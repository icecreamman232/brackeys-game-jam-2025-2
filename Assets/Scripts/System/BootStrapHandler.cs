using UnityEngine;

public class BootStrapHandler : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] m_bootStrap;

    private void Start()
    {
        foreach (var component in m_bootStrap)
        {
            if (component is IBootStrap bootStrap)
            {
                bootStrap.Install();
            }
        }
    }
}
