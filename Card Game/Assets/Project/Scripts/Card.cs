using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    public static int ID;

    public int id;
    [SerializeField] RectTransform rect;
    [SerializeField] string cardName;
    [SerializeField] string cardDescription;
    [SerializeField] int cost;
    [SerializeField] int power;
    [SerializeField] GameObject hoverCard;

    static CardSpawner spawner;
    public static GameObject emptyCard;
    int cardSiblingIndex;

    void Awake()
    {
        ID++;
        id = Random.Range(0, 999999);
        spawner = GameObject.FindObjectOfType<CardSpawner>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (emptyCard != null)
            {
                transform.SetSiblingIndex(cardSiblingIndex);
                Destroy(emptyCard);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO: set card info to hover card on enter
        hoverCard.SetActive(true);
        spawner.RPC_HoverCard(id, true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverCard.SetActive(false);
        spawner.RPC_HoverCard(id, false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (emptyCard == null)
        {
            cardSiblingIndex = transform.GetSiblingIndex();
            emptyCard = Instantiate(spawner.emptyCardPrefab, spawner.handLayout);
            emptyCard.transform.SetSiblingIndex(cardSiblingIndex);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // TODO: Create Networked card object with collider
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        //var position = Camera.main.WorldToScreenPoint(Input.mousePosition);
        //transform.parent = null;
        //transform.position = position;
    }

}
