using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SGGames.Scripts.Core;
using SGGames.Scripts.System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SGGames.Scripts.Item
{
   public class ItemManager : MonoBehaviour, IBootStrap, IGameService
   {
      [SerializeField] private PlaySFXEvent m_playSFXEvent;
      [SerializeField] private CardManager m_cardManager;
      [SerializeField] private ScoreManager m_scoreManager;
      [SerializeField] private ItemContainer m_itemContainer;
      [SerializeField] private Transform[] m_itemPositions;
      [SerializeField] private MultiplierDisplayer[] m_multiplierDisplayers;
      [SerializeField] private ItemDescriptionDisplayer[] m_itemDescriptionDisplayers;
      [SerializeField] private List<ItemBehavior> m_ownedItems = new List<ItemBehavior>();
      
      private BanhMiItem m_banhMiItemRef;
      private const int k_DefaultNumberItem = 1;
      
      public List<ItemBehavior> OwnedItems => m_ownedItems;
      
      public void Install()
      {
         ServiceLocator.RegisterService<ItemManager>(this);
         var listItem = GetRandomItemsWithoutDuplicates(k_DefaultNumberItem);
         for (int i = 0; i < listItem.Count; i++)
         {
            CreateItem(listItem[i].ItemPrefab);
         }
      }

      public void Uninstall()
      {
         ServiceLocator.UnregisterService<ItemManager>();
      }

      public void AddItem(ItemData itemData)
      {
         CreateItem(itemData.ItemPrefab);
         if (itemData.ItemID == ItemID.RedPaper)
         {
            m_cardManager.AddDiscardNumber(1);
         }
      }

      public bool HasItem(ItemID id)
      {
         return (m_ownedItems.FirstOrDefault(item => item.ItemData.ItemID == id) != null);
      }

      public void TriggerItem(Action<float, float> onUpdateMultiplierCounterAction, Action onFinish)
      {
         StartCoroutine(OnTriggerItemProcess(onUpdateMultiplierCounterAction, onFinish));
      }
      
      public List<ItemData> GetRandomItemsWithoutDuplicates(int number)
      {
          // Create a HashSet of owned item IDs for O(1) lookup
          var ownedItemIDs = new HashSet<ItemID>();
          for (int i = 0; i < m_ownedItems.Count; i++)
          {
              ownedItemIDs.Add(m_ownedItems[i].ItemData.ItemID);
          }
          
          // Create array of available items (only those not owned)
          var availableItems = new ItemData[
              m_itemContainer.CommonItems.Count + 
              m_itemContainer.UncommonItems.Count + 
              m_itemContainer.RareItems.Count];
          
          int availableCount = 0;
          
          // Add common items that aren't owned
          for (int i = 0; i < m_itemContainer.CommonItems.Count; i++)
          {
              if (!ownedItemIDs.Contains(m_itemContainer.CommonItems[i].ItemID))
              {
                  availableItems[availableCount++] = m_itemContainer.CommonItems[i];
              }
          }
          
          // Add uncommon items that aren't owned
          for (int i = 0; i < m_itemContainer.UncommonItems.Count; i++)
          {
              if (!ownedItemIDs.Contains(m_itemContainer.UncommonItems[i].ItemID))
              {
                  availableItems[availableCount++] = m_itemContainer.UncommonItems[i];
              }
          }
          
          // Add rare items that aren't owned
          for (int i = 0; i < m_itemContainer.RareItems.Count; i++)
          {
              if (!ownedItemIDs.Contains(m_itemContainer.RareItems[i].ItemID))
              {
                  availableItems[availableCount++] = m_itemContainer.RareItems[i];
              }
          }
          
          // Clamp the requested number to available items
          var itemsToSelect = Mathf.Min(number, availableCount);
          var result = new List<ItemData>(itemsToSelect);
          
          // Use Fisher-Yates shuffle to select random items without duplicates
          for (int i = 0; i < itemsToSelect; i++)
          {
              var randomIndex = Random.Range(i, availableCount);
              
              // Add the selected item to result
              result.Add(availableItems[randomIndex]);
              
              // Swap the selected item to the front (already processed area)
              (availableItems[i], availableItems[randomIndex]) = (availableItems[randomIndex], availableItems[i]);
          }
          
          return result;
      }


      public void ShowItemDescription(ItemBehavior item)
      {
         if (item is BanhMiItem)
         {
            var banhMiDesc = GetDescForBanhMi((BanhMiItem)item);
            m_itemDescriptionDisplayers[item.ItemIndex].ShowDescription(item.ItemData.Name, banhMiDesc, item.ItemData.Rarity);
         }
         else
         {
            m_itemDescriptionDisplayers[item.ItemIndex].ShowDescription(item.ItemData.Name, item.ItemData.Description, item.ItemData.Rarity);
         }
         
      }

      public void HideItemDescription(ItemBehavior item)
      {
         m_itemDescriptionDisplayers[item.ItemIndex].HideDescription();
      }

      public void RandomNewBanhMiCondition()
      {
         if (m_banhMiItemRef == null) return;
         m_banhMiItemRef.RandomNewConditionType();
      }

      private IEnumerator OnTriggerItemProcess(Action<float, float> onUpdateMultiplierUIAction, Action onFinish)
      {
         var totalMultiplier = 0.0f;
         for (int i = 0; i < m_ownedItems.Count; i++)
         {
            if (m_ownedItems[i].ItemData.ItemID == ItemID.MegaSpeaker)
            {
               //Re-trigger first card in the hand
               m_cardManager.CountScoreForCardAtIndex(0);
               m_playSFXEvent.Raise(SFX.ScoreCounting);
               yield return new WaitForSeconds(CardManager.k_ShowScoreTime + 0.2f + 0.2f);
               continue;
            }
            
            var multiplierInfo = m_ownedItems[i].Use(m_cardManager);
            
            if (!(multiplierInfo.type == MultiplierType.Multiply && multiplierInfo.value <= 1.0f)
                && !(multiplierInfo.type == MultiplierType.Add && multiplierInfo.value <= 0.0f))
            {
               m_ownedItems[i].PlayTriggerAnimation();
               m_multiplierDisplayers[i].ShowMultiplier(multiplierInfo.type, multiplierInfo.value);
               m_playSFXEvent.Raise(SFX.MulCounting);
            }

            
            if (multiplierInfo.type == MultiplierType.Add)
            {
               totalMultiplier += multiplierInfo.value;
            }
            else
            {
               //If multiplier is 0, set it to 1.0f for multiplication
               if (totalMultiplier == 0)
               {
                  totalMultiplier = 1.0f;
               }
               totalMultiplier *= multiplierInfo.value;
            }

            //If total multiplier is smaller than 1, we add 1 so it wont be a reduce to player score
            if (totalMultiplier <= 1.0f)
            {
               totalMultiplier += 1.0f;
            }

            if (!(multiplierInfo.type == MultiplierType.Multiply && multiplierInfo.value <= 1.0f)
                && !(multiplierInfo.type == MultiplierType.Add && multiplierInfo.value <= 0.0f))
            {
               var mulForAnimation = totalMultiplier - 1;
               if (mulForAnimation <= 1.0f)
               {
                  mulForAnimation = 1.0f;
               }
            
               onUpdateMultiplierUIAction?.Invoke(mulForAnimation, totalMultiplier);
               yield return new WaitForSeconds(CardManager.k_ShowScoreTime + 0.2f + 0.2f);
            }
         }
         
         m_scoreManager.AddMultiplier(totalMultiplier);
         onFinish?.Invoke();
      }
      
      private void CreateItem(ItemBehavior itemPrefab)
      {
         var currentIndex = m_ownedItems.Count;
         var item = Instantiate(itemPrefab);
         item.ItemIndex = currentIndex;
         item.transform.position = m_itemPositions[currentIndex].position;
         item.SetItemPosition(m_itemPositions[currentIndex].position);
         item.ShowItemDescriptionAction = ShowItemDescription;
         item.HideItemDescriptionAction = HideItemDescription;
         
         // Hook up the drag and drop functionality
         item.IsOverlappedOnItem = IsItemOverlapping;
         item.SwapItemsAction = SwapItems;

         if (item.ItemData.ItemID == ItemID.BanhMi)
         {
            m_banhMiItemRef = (BanhMiItem)item;
         }
         m_ownedItems.Add(item);
      }


      /// <summary>
      /// Check if this item is overlapping any other item.
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      private ItemBehavior IsItemOverlapping(ItemBehavior item)
      {
         Collider2D[] results = new Collider2D[3]; // Adjust size as needed
         ContactFilter2D filter = new ContactFilter2D();
         int count = Physics2D.OverlapBox(item.ItemCollider.bounds.center, item.ItemCollider.size, 0, filter, results);

         for (int i = 0; i < count; i++)
         {
            // Skip if it's the same collider as the item we're checking
            if (results[i] == item.ItemCollider) continue;
      
            if (results[i].gameObject.TryGetComponent(out ItemBehavior otherItem))
            {
               return otherItem;
            }
         }

         return null;
      }

      private void SwapItems(ItemBehavior item1, ItemBehavior item2)
      {
         var prevItem1Pos = m_itemPositions[item1.ItemIndex].position;
         var prevItem2Pos = m_itemPositions[item2.ItemIndex].position;
         
         var item1Index = item1.ItemIndex;
         var item2Index = item2.ItemIndex;
         
         // Update the list to reflect the swap
         m_ownedItems[item1Index] = item2;
         m_ownedItems[item2Index] = item1;
         
         item1.TweenItemToPosition(prevItem2Pos, () =>
         {
            item1.transform.position = prevItem2Pos;
            item1.SetItemPosition(prevItem2Pos);
            item1.ItemIndex = item2Index;
         });
         
         item2.TweenItemToPosition(prevItem1Pos, () =>
         {
            item2.transform.position = prevItem1Pos;
            item2.SetItemPosition(prevItem1Pos);
            item2.ItemIndex = item1Index;
         });
      }

      private string GetDescForBanhMi(BanhMiItem banhMiItem)
      {
         var desc = banhMiItem.ItemData.Description;
         var customDesc = ((BanhMiItemData)banhMiItem.ItemData).RandomConditionDescription[(int)banhMiItem.CurrentCondition.conditionType];
         desc = desc.Replace("[random_condition]", $"<color=orange>{banhMiItem.CurrentCondition.numberRequired}</color> {customDesc}");
         return desc;
      }
   }
}

