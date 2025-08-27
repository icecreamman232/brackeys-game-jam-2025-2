using System;
using System.Collections;
using System.Collections.Generic;
using SGGames.Scripts.System;
using UnityEngine;
using Random = UnityEngine.Random;


public class ItemManager : MonoBehaviour, IBootStrap
{
   [SerializeField] private CardManager m_cardManager;
   [SerializeField] private ScoreManager m_scoreManager;
   [SerializeField] private ItemContainer m_itemContainer;
   [SerializeField] private Transform[] m_itemPositions;
   [SerializeField] private MultiplierDisplayer[] m_multiplierDisplayers;
    [SerializeField] private List<ItemBehavior> m_ownedItems = new List<ItemBehavior>();
   
   public void Install()
   {
      //FOR TESTING
      foreach (var item in m_itemContainer.CommonItems)
      {
         AddItem(item);
      }
   }

   public void Uninstall()
   {
     
   }

   public void TriggerItem(Action<float> onUpdateMultiplierCounterAction, Action onFinish)
   {
      StartCoroutine(OnTriggerItemProcess(onUpdateMultiplierCounterAction, onFinish));
   }

   private IEnumerator OnTriggerItemProcess(Action<float> onUpdateMultiplierUIAction, Action onFinish)
   {
      var totalMultiplier = 1.0f;
      for (int i = 0; i < m_ownedItems.Count; i++)
      {
         var multiplierInfo = m_ownedItems[i].Use(m_cardManager);
         
         if (multiplierInfo.value > 0)
         {
            m_ownedItems[i].PlayTriggerAnimation();
            m_multiplierDisplayers[i].ShowMultiplier(multiplierInfo.type, multiplierInfo.value);
            yield return new WaitForSeconds(0.4f);
         }
         
         if (multiplierInfo.type == MultiplierType.Add)
         {
            totalMultiplier += multiplierInfo.value;
         }
         else
         {
            totalMultiplier *= multiplierInfo.value;
         }
         Debug.Log($"Total Multiplier: {totalMultiplier}");
         onUpdateMultiplierUIAction?.Invoke(totalMultiplier);
         yield return new WaitForSeconds(0.2f);
      }
      
      m_scoreManager.AddMultiplier(totalMultiplier);
      onFinish?.Invoke();
   }
   
   private void AddItem(ItemBehavior itemPrefab)
   {
      var currentIndex = m_ownedItems.Count;
      var item = Instantiate(itemPrefab);
      item.ItemIndex = currentIndex;
      item.transform.position = m_itemPositions[currentIndex].position;
      item.SetItemPosition(m_itemPositions[currentIndex].position);
      
      // Hook up the drag and drop functionality
      item.IsOverlappedOnItem = IsItemOverlapping;
      item.SwapItemsAction = SwapItems;
      
      m_ownedItems.Add(item);
   }

   private ItemBehavior GetRandomItem()
   {
      return m_itemContainer.CommonItems[Random.Range(0, m_itemContainer.CommonItems.Count)];
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
}
