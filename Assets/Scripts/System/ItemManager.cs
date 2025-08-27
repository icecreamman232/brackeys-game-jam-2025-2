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

   public void TriggerItem(Action<float> onUpdateMultiplierUIAction, Action onFinish)
   {
      StartCoroutine(OnTriggerItemProcess(onUpdateMultiplierUIAction, onFinish));
   }

   private IEnumerator OnTriggerItemProcess(Action<float> onUpdateMultiplierUIAction, Action onFinish)
   {
      var totalMultiplier = 1.0f;
      for (int i = 0; i < m_ownedItems.Count; i++)
      {
         var multiplierInfo = m_ownedItems[i].Use(m_cardManager);
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
      m_ownedItems.Add(item);
      
   }

   private ItemBehavior GetRandomItem()
   {
      return m_itemContainer.CommonItems[Random.Range(0, m_itemContainer.CommonItems.Count)];
   }
}
