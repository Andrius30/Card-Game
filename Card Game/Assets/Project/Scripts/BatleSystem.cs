using Andrius.Core.Debuging;
using Fusion;
using System;
using UnityEngine;

public class BatleSystem : NetworkBehaviour
{
    public static Action<GameObject, CardData> onGetPlayerData; // called when player clicks on placed card
    public static Action<GameObject, CardData> onGetOponentData; // called when player release button on oponent card entered with mouse
    public static Action onStartBattle;

    CardData playerCardData;
    CardData oponentCardData;

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

    public void StartBatlle()
    {
        var oponenPlacedCard = oponentCardObject.GetComponent<PlacedCard>();
        var playerPlacedCard = playerCardObject.GetComponent<PlacedCard>();

        var attackerResult = oponenPlacedCard.cardHealth - playerCardData.power;
        var defenderResponseResult = playerPlacedCard.cardHealth - oponentCardData.power;

        Debug.Log($"Start batlle! attacker res {attackerResult} -- defender res {defenderResponseResult} :20:red;".Interpolate());
        if (attackerResult <= 0)
        {
            Destroy(oponentCardObject);
            RPC_DestroyPlayerCard(playerCardData.cardID);
            playerPlacedCard.cardHealth -= defenderResponseResult;
        }
        else
        {
            oponenPlacedCard.cardHealth -= attackerResult;
        }
        if (defenderResponseResult <= 0)
        {
            Destroy(playerCardObject);
            RPC_DestroyOponentCard(oponentCardData.cardID);
            oponenPlacedCard.cardHealth -= defenderResponseResult;
        }
        else
        {
            playerPlacedCard.cardHealth -= defenderResponseResult;
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
        onGetPlayerData += SetOponentData;
        onStartBattle += StartBatlle;
    }
    void OnDisable()
    {
        onGetPlayerData -= SetPlayerData;
        onGetPlayerData -= SetOponentData;
        onStartBattle -= StartBatlle;

    }
}
