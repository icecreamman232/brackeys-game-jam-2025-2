using TMPro;
using UnityEngine;

public class ItemDescriptionDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_descriptionText;

    public void ShowDescription(string description)
    {
        this.gameObject.SetActive(true);
        m_descriptionText.text = description;
    }
    
    public void HideDescription()
    {
        this.gameObject.SetActive(false);
    }
}
