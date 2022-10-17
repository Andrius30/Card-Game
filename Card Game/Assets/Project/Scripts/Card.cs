using Fusion;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public static int ID;

    public int id;
    [SerializeField] RectTransform rect;
    [SerializeField] string cardName;
    [SerializeField] string cardDescription;
    [SerializeField] int cost;
    [SerializeField] int power;
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
        ID++;
        id = Random.Range(0, 999999); // FIXME: When cards database will be ready each card should have already pre-defined id's
        spawner = GameObject.FindObjectOfType<CardSpawner>();
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
            ToggeleHoverCard(id, false);

        CardFallowMouse();

        isHoveringOverPlayerField = IsHoveringOverPlayerField();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!clickedOnCard)
        {
            ToggeleHoverCard(id, true);
        }
    }
    public void OnPointerExit(PointerEventData eventData) => ToggeleHoverCard(id, false);
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
            Instantiate(spawner.placedCardPrefab, spawner.playerField);
            Destroy(gameObject);
            Destroy(emptyCard);
            spawner.inHandsList.Remove(this);
        }
    }

    void ToggeleHoverCard(int id, bool enable)
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
