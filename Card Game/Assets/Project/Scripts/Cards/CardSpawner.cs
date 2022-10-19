using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Timer = Andrius.Core.Timers.Timer;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Andrius.Core.Utils;
using System.Collections;

public class CardSpawner : NetworkBehaviour
{
    public static Action onSceneLoadedDone;

    public Transform canvasTransform;
    public List<InvisibleCard> oponentInHandsList = new List<InvisibleCard>();
    public InvisibleCard invisibleCard;
    public GameObject placedCardPrefab;
    public GameObject emptyCardPrefab;
    public Transform oponentHandLayout;
    public Transform handLayout;
    public Transform playerField;
    public float cardFallowMouseSpeed = 5;
    public Vector2 cardOffset;
    public EventSystem eventSystem;
    public GraphicRaycaster raycaster;
    public CardsDatabase cardsDatabase;
    public List<Card> inHandsList;

    [SerializeField] float cardsDrawDelay = .5f;
    [SerializeField] List<CardData> playerDeck;
    [SerializeField] int startingCardInHandsCount = 3;
    [SerializeField] TextMeshProUGUI countdownText;

    [HideInInspector] public Canvas canvas;

    const float time = 2f;
    Timer countDownTimer;
    WaitForSeconds delayWait;

    void Awake()
    {
        countDownTimer = new Timer(time, OnDoneSpawn, false, false);
        countdownText.gameObject.SetActive(true);
        delayWait = new WaitForSeconds(cardsDrawDelay);
        canvas = canvasTransform.GetComponent<Canvas>();
        playerDeck.AddRange(cardsDatabase.cards); // later on can be inserted extra earned cards
    }
    void Update()
    {
        if (countDownTimer.IsDone()) return;
        countDownTimer.StartTimer();
        countdownText.text = $"{countDownTimer.GetCurrentTime():0}";
    }

    public IEnumerator DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomCard = Random.Range(0, playerDeck.Count);
            var randomCrd = playerDeck[randomCard];
            GameObject gm = Instantiate(randomCrd.cardPrefab, handLayout);
            Card card = gm.GetComponent<Card>();
            card.cardData = playerDeck[randomCard];
            int infinityLoopError = 0;
            while (CheckIfCardDuplicated(card.cardData.cardID))
            {
                if (gm != null)
                {
                    Destroy(gm);
                }
                randomCard = Random.Range(0, playerDeck.Count);
                randomCrd = playerDeck[randomCard];
                gm = Instantiate(randomCrd.cardPrefab, handLayout);
                card = gm.GetComponent<Card>();
                card.cardData = playerDeck[randomCard];
                infinityLoopError++;
                if (infinityLoopError > 9999)
                {
                    break;
                }
                yield return null;
            }
            InitializeCard(card);
            inHandsList.Add(card);
            RPC_SpawnInvisibleCards(card.cardData.cardID);
            yield return delayWait;
        }
    }
    bool CheckIfCardDuplicated(string id)
    {
        foreach (var card in inHandsList)
        {
            if (card.cardData.cardID == id)
                return true;
        }
        return false;
    }

    #region RPC's
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false, Channel = RpcChannel.Reliable, InvokeResim = true)]
    public void RPC_SpawnInvisibleCards(string id)  // Spawn oponent invisible cards back sided
    {
        InvisibleCard invisCard = Instantiate(invisibleCard, oponentHandLayout);
        oponentInHandsList.Add(invisCard);
        invisCard.ID = id;
    }
    [Rpc(InvokeLocal = false)]
    public void RPC_HoverCard(string id, bool enable) // on real player hovering the card sync oponent back sided cards at real time, to make an iliusion that oponent is at other side of the table
    {
        var invisCard = GetInvisibleCardById(id);
        EnableInvisibleCard(invisCard, enable);
    }
    [Rpc]
    public void RPC_SendRequestToServerSpawnCard(string id)
    {
        if (Runner.IsServer)
        {
            RPC_ResponceFromServerSpawnCard(id);
        }
    }
    [Rpc]
    public void RPC_ResponceFromServerSpawnCard(string id)
    {
        GameObject obj = null;
        CardData cardData;
        if (!Runner.IsServer)
        {
            cardData = cardsDatabase.GetCardbyId(id);
            obj = Instantiate(cardData.placedCardPrefab, playerField);
            var crd = obj.GetComponent<PlacedCard>();
            GameManager.instance.playerPlaycedCards.Add(crd);
        }
        else
        {
            cardData = cardsDatabase.GetCardbyId(id);
            obj = Instantiate(cardData.placedCardPrefab, GameManager.instance.oponentCardField);
            var crd = obj.GetComponent<PlacedCard>();
            GameManager.instance.oponentPlaycedCards.Add(crd);
            crd.isEnemy = true;
        }
        PlacedCard card = obj.GetComponent<PlacedCard>();
        InitializePlacedCard(card, cardData);
    }
    #endregion

    public void InitializeCard(Card card)
    {
        Image img;
        TextMeshProUGUI nameText, descriptionText, cardPowerText, cardHealthText;
        FindCardComponents(card.transform, out img, out nameText, out descriptionText, out cardPowerText, out cardHealthText);
        InitializeCard(card.cardData, img, nameText, descriptionText, cardPowerText, cardHealthText);
    }
    public void InitializePlacedCard(PlacedCard card, CardData cardData)
    {
        card.cardData = cardData;
        Image img;
        TextMeshProUGUI nameText, descriptionText, cardPowerText, cardHealthText;
        FindCardComponents(card.transform, out img, out nameText, out descriptionText, out cardPowerText, out cardHealthText);
        InitializeCard(cardData, img, nameText, descriptionText, cardPowerText, cardHealthText);
    }

    public void InitializeCard(CardData cardData, Image img, TextMeshProUGUI nameText, TextMeshProUGUI descriptionText, TextMeshProUGUI cardPowerText, TextMeshProUGUI cardHealthText)
    {
        img.sprite = cardData.cardIcon;
        nameText.text = cardData.cardName;
        descriptionText.text = cardData.cardDecription;
        cardPowerText.text = $"{cardData.power}";
        cardHealthText.text = $"{cardData.cardHealth}";
    }

    public void FindCardComponents(Transform card, out Image img, out TextMeshProUGUI nameText, out TextMeshProUGUI descriptionText, out TextMeshProUGUI cardPowerText, out TextMeshProUGUI cardHealthText)
    {
        img = StaticFunctions.FindChild(card, "CardIcon").GetComponent<Image>();
        nameText = StaticFunctions.FindChild(card, "CardName").GetComponent<TextMeshProUGUI>();
        descriptionText = StaticFunctions.FindChild(card, "CardDescriptionText").GetComponent<TextMeshProUGUI>();
        cardPowerText = StaticFunctions.FindChild(card, "CardPowerText").GetComponent<TextMeshProUGUI>();
        cardHealthText = StaticFunctions.FindChild(card, "CardHealthText").GetComponent<TextMeshProUGUI>();
    }

    public void SpawnPlacedCard(CardData cardData)
    {
        if (Runner.IsServer)
        {
            var obj = Runner.Spawn(placedCardPrefab, Vector2.zero, Quaternion.identity, Object.InputAuthority);
            obj.transform.SetParent(playerField);
            PlacedCard card = obj.GetComponent<PlacedCard>();
            card.cardData = cardData;
            GameManager.instance.playerPlaycedCards.Add(card);
            Image img;
            TextMeshProUGUI nameText, descriptionText, cardPowerText, cardHealthText;
            FindCardComponents(card.transform, out img, out nameText, out descriptionText, out cardPowerText, out cardHealthText);
            InitializeCard(cardData, img, nameText, descriptionText, cardPowerText, cardHealthText);
        }
    }

    public InvisibleCard GetInvisibleCardById(string id)
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
        StartCoroutine(DrawCards(5));
        countdownText.gameObject.SetActive(false);
    }
    void EnableInvisibleCard(InvisibleCard invisCard, bool enable)
    {
        if (invisCard == null) return;
        invisCard.hoverCard.SetActive(enable);
    }

}
