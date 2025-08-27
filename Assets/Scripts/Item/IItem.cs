using SGGames.Scripts.System;
using UnityEngine;

public interface IItem
{
    (MultiplierType type, float value) Use(CardManager cardManager);
}
