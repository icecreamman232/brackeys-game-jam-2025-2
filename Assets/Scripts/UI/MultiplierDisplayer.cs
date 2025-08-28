using TMPro;
using UnityEngine;

public class MultiplierDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_multiplierText;

    public void ShowMultiplier(MultiplierType type, float multiplier)
    {
        var multiplierText = "";
        multiplierText = type == MultiplierType.Add ? $"+{multiplier:F2}" : $"x{multiplier:F2}";
        m_multiplierText.text = multiplierText;
        this.gameObject.SetActive(true);
        transform.LeanScale(Vector3.one * 1.2f,0.4f)
            .setOnComplete(()=>
            {
                this.gameObject.SetActive(false);
            });
    }
}
