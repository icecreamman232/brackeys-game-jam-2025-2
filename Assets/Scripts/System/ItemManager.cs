using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ItemManager : MonoBehaviour
{
   [SerializeField] private ScoreManager m_scoreManager;
   [SerializeField] private ItemContainer m_itemContainer;
   [SerializeField] private List<ItemBehavior> m_ownedItems = new List<ItemBehavior>();

   private void Start()
   {
      var item = GetRandomItem();
      AddItem(item);
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
         var multiplierInfo = m_ownedItems[i].Use();
         if (multiplierInfo.type == MultiplierType.Add)
         {
            totalMultiplier += multiplierInfo.value;
         }
         else
         {
            totalMultiplier *= multiplierInfo.value;
         }
         onUpdateMultiplierUIAction?.Invoke(totalMultiplier);
         
         yield return null;
      }
      
      m_scoreManager.AddMultiplier(totalMultiplier);
      onFinish?.Invoke();
   }
   
   private void AddItem(ItemBehavior item)
   {
      m_ownedItems.Add(item);
   }

   private ItemBehavior GetRandomItem()
   {
      return m_itemContainer.CommonItems[Random.Range(0, m_itemContainer.CommonItems.Count)];
   }
}
