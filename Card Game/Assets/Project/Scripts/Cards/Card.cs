using Andrius.Core.Utils;
using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform rect;
    public CardData cardData;
    [SerializeField] GameObject hoverCard;

    public static GameObject emptyCard;
    static CardSpawner spawner;

    int cardSiblingIndex;
    bool clickedOnCard = false;
    bool isHoveringOverPlayerField;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        spawner = GameObject.FindObjectOfType<CardSpawner>();
        // TODO: Initialize Card UI from card data
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (emptyCard != null)
            {
                transform.SetParent(spawner.handLayout);
                Destroy(emptyCard);
                transform.SetSiblingIndex(cardSiblingIndex);
                clickedOnCard = false;
            }
        }
        if (!clickedOnCard) return;
        if (hoverCard.activeInHierarchy)
            ToggeleHoverCard(cardData.cardID, false);

        CardFallowMouse();

        isHoveringOverPlayerField = IsHoveringOverPlayerField();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!clickedOnCard)
        {
            ToggeleHoverCard(cardData.cardID, true);
            Image img;
            TextMeshProUGUI nameText, descriptionText, cardPowerText, cardHealthText;
            spawner.FindCardComponents(hoverCard.transform, out img, out nameText, out descriptionText, out cardPowerText, out cardHealthText);
            spawner.InitializeCard(cardData, img, nameText, descriptionText, cardPowerText, cardHealthText);
        }
    }
    public void OnPointerExit(PointerEventData eventData) => ToggeleHoverCard(cardData.cardID, false);
    public void OnPointerDown(PointerEventData eventData)
    {
        if (emptyCard == null)
        {
            clickedOnCard = true;
            cardSiblingIndex = transform.GetSiblingIndex();
            emptyCard = Instantiate(spawner.emptyCardPrefab, spawner.handLayout);
            emptyCard.transform.SetSiblingIndex(cardSiblingIndex);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isHoveringOverPlayerField)
        {
            clickedOnCard = false;
            if (FusionCallbacks.runner.IsServer)
            {
                spawner.SpawnPlacedCard(cardData);
            }
            else
            {
                spawner.RPC_SendRequestToServerSpawnCard(cardData.cardID);
            }
            Destroy(gameObject);
            Destroy(emptyCard);
            spawner.inHandsList.Remove(this);
        }
    }

    void ToggeleHoverCard(string id, bool enable)
    {
        hoverCard.SetActive(enable);
        spawner.RPC_HoverCard(id, enable);
    }
    void CardFallowMouse()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(spawner.canvasTransform as RectTransform, Input.mousePosition, spawner.canvas.worldCamera, out pos);
        transform.position = Vector2.Lerp(transform.position, (Vector2)spawner.canvasTransform.TransformPoint(pos) + spawner.cardOffset, Time.deltaTime * spawner.cardFallowMouseSpeed);
        if (transform.parent != spawner.canvasTransform)
            transform.parent = spawner.canvasTransform;
    }
    bool IsHoveringOverPlayerField()
    {
        PointerEventData data = new PointerEventData(spawner.eventSystem);
        List<RaycastResult> result = new List<RaycastResult>();
        data.position = Input.mousePosition;
        spawner.raycaster.Raycast(data, result);
        foreach (var item in result)
        {
            if (item.gameObject.layer == 3)
            {
                return true;
            }
        }
        return false;
    }

}
