using System.Collections.Generic;
using SGGames.Scripts.Card;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    private List<CardBehavior> m_cardInDiscard = new List<CardBehavior>();


    public void AddCardToDiscard(CardBehavior card)
    {
        m_cardInDiscard.Add(card);
        card.ChangeCardState(CardState.InDiscard);
        card.SetCardIndex(-1);
        card.ResetSelection();
    }
    
    public void AddCardsToDiscard(List<CardBehavior> cards)
    {
        foreach (var card in cards)
        {
            AddCardToDiscard(card);
        }
    }
    
    public List<CardBehavior> RemoveAllCards()
    {
        var allCards = new List<CardBehavior>(m_cardInDiscard);
        
        foreach (var card in allCards)
        {
            card.ChangeCardState(CardState.InPile);
        }
        m_cardInDiscard.Clear();
        return allCards;
    }
    
    public void PositionCardAtDiscard(CardBehavior card)
    {
        card.transform.position = this.transform.position;
        card.gameObject.SetActive(false);
    }

}
