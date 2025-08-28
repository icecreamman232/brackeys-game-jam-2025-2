using SGGames.Scripts.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Energy HUD Event", menuName = "SGGames/Events/Energy HUD")]
public class EnergyHUDEvent : ScriptableEvent<EnergyHUDInfo> { }

public class EnergyHUDInfo
{
    public int CurrentEnergy;
    public int MaxEnergy;
}
