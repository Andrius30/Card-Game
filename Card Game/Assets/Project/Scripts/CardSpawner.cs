using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Timer = Andrius.Core.Timers.Timer;
using TMPro;

public class CardSpawner : NetworkBehaviour
{
    public static Action onSceneLoadedDone;

    public List<InvisibleCard> oponentInHandsList = new List<InvisibleCard>();
    public InvisibleCard invisibleCard;
    public Transform oponentHandLayout;

    [SerializeField] List<Card> cardDec = new List<Card>();
    [SerializeField] List<Card> inHandsList = new List<Card>();
    [SerializeField] int startingCardInHandsCount = 3;
    [SerializeField] Transform handLayout;
    [SerializeField] TextMeshProUGUI countdownText;

    const float time = 2f;
    Timer countDownTimer;

    void Awake()
    {
        countDownTimer = new Timer(time, OnDoneSpawn, false, false);
        countdownText.gameObject.SetActive(true);
    }
    void Update()
    {
        if (countDownTimer.IsDone()) return;
        countDownTimer.StartTimer();
        countdownText.text = $"{countDownTimer.GetCurrentTime():00:00}";
    }

    void SpawnCards()
    {
        for (int i = 0; i < startingCardInHandsCount; i++)
        {
            int randomCard = Random.Range(0, cardDec.Count);
            Card card = Instantiate(cardDec[randomCard], handLayout);
            inHandsList.Add(card);

            RPC_SpawnInvisibleCards(card.id);
        }
    }

    #region RPC's
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false, Channel = RpcChannel.Reliable, InvokeResim = true)]
    public void RPC_SpawnInvisibleCards(int id)  // Spawn oponent invisible cards back sided
    {
        InvisibleCard invisCard = Instantiate(invisibleCard, oponentHandLayout);
        oponentInHandsList.Add(invisCard);
        invisCard.ID = id;
    }
    [Rpc]
    public void RPC_HoverCard(int id, bool enable) // on real player hovering the card sync oponent back sided cards at real time, to make an iliusion that oponent is at other side of the table
    {
        var invisCard = GetInvisibleCardById(id);
        EnableInvisibleCard(invisCard, enable);
    } 
    #endregion

    public InvisibleCard GetInvisibleCardById(int id)
    {
        foreach (var card in oponentInHandsList)
        {
            if (card.ID == id)
                return card;
        }
        return null;
    }

    void OnDoneSpawn()
    {
        SpawnCards();
        countdownText.gameObject.SetActive(false);
    }
    void EnableInvisibleCard(InvisibleCard invisCard, bool enable)
    {
        if (invisCard == null) return;
        invisCard.hoverCard.SetActive(enable);
    }

}
