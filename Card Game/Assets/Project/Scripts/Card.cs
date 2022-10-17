using Fusion;
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

    void Awake()
    {
        ID++;
        id = Random.Range(0, 999999);
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

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(spawner.canvasTransform as RectTransform, Input.mousePosition, spawner.canvasTransform.GetComponent<Canvas>().worldCamera, out pos);
        transform.position = Vector2.Lerp(transform.position, spawner.canvasTransform.TransformPoint(pos), Time.deltaTime * spawner.cardFallowMouseSpeed);
        if (transform.parent != spawner.canvasTransform)
            transform.parent = spawner.canvasTransform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO: set card info to hover card on enter
        if (!clickedOnCard)
        {
            ToggeleHoverCard(id, true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ToggeleHoverCard(id, false);
    }
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
        // TODO: Create Networked card object with collider
    }

    void ToggeleHoverCard(int id, bool enable)
    {
        hoverCard.SetActive(enable);
        spawner.RPC_HoverCard(id, enable);
    }
}
