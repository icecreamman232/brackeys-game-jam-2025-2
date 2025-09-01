using UnityEngine;

namespace SGGames.Scripts.Item
{
    public class ItemVisualHelper : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_itemIcon;

        public void SetItemVisual(ItemData itemData)
        {
            m_itemIcon.sprite = itemData.Icon;
        }
    }
}
