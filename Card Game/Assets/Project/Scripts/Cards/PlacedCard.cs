using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(UILineRendererList))]
public class PlacedCard : NetworkBehaviour, ISpawned, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public string ID;
    public CardData cardData;
    UILineRendererList lineRenderer;
    [HideInInspector] public CardSpawner spawner;
    [SerializeField] Vector2 offset; // -850, -370
    Vector2 startingPosition;
    Vector2 lastDragPosition;

    void Start()
    {
        lineRenderer = GetComponent<UILineRendererList>();
        try
        {
            if (FusionCallbacks.runner.IsServer)
                RPC_SetCardID(cardData.cardID);
        }
        catch { }
    }
    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsClient)
        {
            if (spawner == null)
            {
                spawner = FindObjectOfType<CardSpawner>();
            }
            transform.SetParent(GameManager.instance.oponentCardField);
        }
    }

    [Rpc(InvokeLocal = false)]
    public void RPC_SetCardID(string id)
    {
        ID = id;
        cardData = spawner.cardsDatabase.GetCardbyId(id);
        spawner.InitializePlacedCard(this, cardData);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Pointer down");
        var rect = GetComponent<RectTransform>();
        startingPosition = new Vector3(rect.position.x, rect.position.y);
        if (lineRenderer == null)
        {
            Debug.LogError($"Line renderer list null");
            return;
        }
        lineRenderer.AddPoint(startingPosition + offset);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = new Vector3(eventData.position.x, eventData.position.y);

        if (lastDragPosition != pos)
        {
            lineRenderer.RemovePoint(lastDragPosition);
        }
        lineRenderer.AddPoint(pos + offset);
        lastDragPosition = pos + offset;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"Pointer up. Initiate batle");
        lineRenderer.RemovePoint(startingPosition);
        lineRenderer.RemovePoint(lastDragPosition);

    }

}
