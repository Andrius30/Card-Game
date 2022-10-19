using Andrius.Core.Debuging;
using Fusion;
using System;
using UnityEngine;

public class BatleSystem : NetworkBehaviour
{
    public static Action<GameObject, CardData> onGetPlayerData; // called when player clicks on placed card
    public static Action<GameObject, CardData> onGetOponentData; // called when player release button on oponent card entered with mouse
    public static Action onStartBattle;

    [SerializeField] CardData playerCardData;
    [SerializeField] CardData oponentCardData;

    GameObject playerCardObject;
    GameObject oponentCardObject;

    void SetPlayerData(GameObject gm, CardData data)
    {
        playerCardObject = gm;
        playerCardData = data;
    }
    void SetOponentData(GameObject gm, CardData data)
    {
        oponentCardObject = gm;
        oponentCardData = data;
    }

    public void StartBattle()
    {
        var oponenPlacedCard = oponentCardObject.GetComponent<PlacedCard>();
        var playerPlacedCard = playerCardObject.GetComponent<PlacedCard>();

        var attackerResult = oponenPlacedCard.cardHealth - playerCardData.power;
        //Debug.Log($"oponenPlacedCard.cardHealth {oponenPlacedCard.cardHealth} -- playerCardData.power {playerCardData.power}-- oponenPlacedCard.cardHealth - playerCardData.power {oponenPlacedCard.cardHealth - playerCardData.power}");
        var defenderResponseResult = playerPlacedCard.cardHealth - oponentCardData.power;
        //Debug.Log($"playerPlacedCard.cardHealth {playerPlacedCard.cardHealth} -- oponentCardData.power {oponentCardData.power}-- playerPlacedCard.cardHealth - oponentCardData.power {playerPlacedCard.cardHealth - oponentCardData.power}");

        Debug.Log($"Start batlle! attacker res {attackerResult} -- defender res {defenderResponseResult} :20:red;".Interpolate());
        if (attackerResult <= 0)
        {
            Destroy(oponentCardObject);
            RPC_DestroyPlayerCard(playerCardData.cardID);
            playerPlacedCard.cardHealth -= oponentCardData.power;
        }
        else
        {
            playerPlacedCard.cardHealth -= oponentCardData.power;
        }
        if (defenderResponseResult <= 0)
        {
            Destroy(playerCardObject);
            RPC_DestroyOponentCard(oponentCardData.cardID);
            oponenPlacedCard.cardHealth -= playerCardData.power;
        }
        else
        {
            oponenPlacedCard.cardHealth -= playerCardData.power;
        }
    }
    [Rpc(InvokeLocal = false)]
    public void RPC_DestroyPlayerCard(string ID)
    {
        foreach (var card in GameManager.instance.playerPlaycedCards)
        {
            if (card.cardData.cardID == ID)
            {
                Destroy(card.gameObject);
                GameManager.instance.playerPlaycedCards.Remove(card);
                return;
            }
        }
    }
    [Rpc(InvokeLocal = false)]
    public void RPC_DestroyOponentCard(string ID)
    {
        foreach (var card in GameManager.instance.oponentPlaycedCards)
        {
            if (card.cardData.cardID == ID)
            {
                Destroy(card.gameObject);
                GameManager.instance.oponentPlaycedCards.Remove(card);
                return;
            }
        }
    }

    void OnEnable()
    {
        onGetPlayerData += SetPlayerData;
        onGetOponentData += SetOponentData;
        onStartBattle += StartBattle;
    }
    void OnDisable()
    {
        onGetPlayerData -= SetPlayerData;
        onGetOponentData -= SetOponentData;
        onStartBattle -= StartBattle;

    }
}
