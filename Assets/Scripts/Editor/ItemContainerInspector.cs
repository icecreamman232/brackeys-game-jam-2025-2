using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemContainer))]
public class ItemContainerInspector : Editor
{
    private ItemContainer m_itemContainer;

    private void OnEnable()
    {
        m_itemContainer = (ItemContainer) target;
    }

    public override  void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Find common item"))
        {
            m_itemContainer.CommonItems.Clear();
            var list = FindItemByRarity(ItemRarity.Common);
            m_itemContainer.CommonItems.AddRange(list);
            EditorUtility.SetDirty((ItemContainer)target);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        
        if (GUILayout.Button("Find uncommon item"))
        {
            m_itemContainer.UncommonItems.Clear();
            var list = FindItemByRarity(ItemRarity.Uncommon);
            m_itemContainer.UncommonItems.AddRange(list);
            EditorUtility.SetDirty((ItemContainer)target);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        
        if (GUILayout.Button("Find rare item"))
        {
            m_itemContainer.RareItems.Clear();
            var list = FindItemByRarity(ItemRarity.Rare);
            m_itemContainer.RareItems.AddRange(list);
            EditorUtility.SetDirty((ItemContainer)target);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
    
    private List<ItemData> FindItemByRarity(ItemRarity rarity)
    {
        var guidList = AssetDatabase.FindAssets("t:ItemData");
        var commonList = new List<ItemData>();
        foreach(var guid in guidList)
        {
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guid));
            if (item.Rarity == rarity)
            {
                commonList.Add(item);
            }
        }

        return commonList;
    }
}
