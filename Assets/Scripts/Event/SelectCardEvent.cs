using SGGames.Scripts.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Select Card Event", menuName = "SGGames/Events/Select Card")]
public class SelectCardEvent : ScriptableEvent<SelectCardEventData> { }

public class SelectCardEventData
{
    public int CardIndex;
    public bool IsSelected;
}
