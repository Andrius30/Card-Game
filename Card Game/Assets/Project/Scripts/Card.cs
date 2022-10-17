using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static int ID;

    public int id;
    [SerializeField] string cardName;
    [SerializeField] string cardDescription;
    [SerializeField] int cost;
    [SerializeField] int power;
    [SerializeField] GameObject hoverCard;

    static CardSpawner spawner;

    void Awake()
    {
        ID++;
        id = Random.Range(0, 999999);
        spawner = GameObject.FindObjectOfType<CardSpawner>();
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

}
