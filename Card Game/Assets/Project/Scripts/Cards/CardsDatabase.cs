using System.Collections.Generic;
using UnityEngine;

public class CardsDatabase : MonoBehaviour
{
    public List<CardData> cards;

    public CardData GetCardbyId(string id)
    {
        foreach (var card in cards)
        {
            if(card.cardID == id)
                return card;
        }
        return null;
    }
}
