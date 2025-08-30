using System;
using SGGames.Scripts.Core;
using UnityEngine;

public class BootStrapHandler : MonoBehaviour, IGameService
{
    [SerializeField] private MonoBehaviour[] m_bootStrap;

    private void Awake()
    {
        ServiceLocator.RegisterService<BootStrapHandler>(this);
    }

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

    public void UninstallBootStrap()
    {
        foreach (var component in m_bootStrap)
        {
            if (component is IBootStrap bootStrap)
            {
                bootStrap.Uninstall();
            }
        }
    }
}
