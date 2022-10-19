using Andrius.Core.Debuging;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.UIElements;

[RequireComponent(typeof(UILineRendererList))]
public class PlacedCard : NetworkBehaviour, ISpawned, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string ID;
    public bool isEnemy = false;
    public CardData cardData;
    UILineRendererList lineRenderer;
    [HideInInspector] public CardSpawner spawner;
    [SerializeField] Vector2 offset; // -850, -370
    Vector2 startingPosition;
    Vector2 lastDragPosition;
    bool hoveringOverEnemyCard = false;
    public int cardHealth;

    void Start()
    {
        cardHealth = cardData.cardHealth;
        lineRenderer = GetComponent<UILineRendererList>();
        try
        {
            if (FusionCallbacks.runner.IsServer)
                RPC_SetCardID(cardData.cardID);
        }
        catch { }
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (hoveringOverEnemyCard)
            {
                hoveringOverEnemyCard = false;
                Debug.Log($"Pointer up. Initiate batle :18:yellow;".Interpolate());
                BatleSystem.onStartBattle?.Invoke();
            }
        }
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
            if (transform.parent.name == GameManager.instance.oponentCardField.name)
            {
                isEnemy = true;
                GameManager.instance.oponentPlaycedCards.Add(this);
            }
            else
                isEnemy = false;
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
        if (!isEnemy)
        {
            BatleSystem.onGetPlayerData?.Invoke(gameObject, cardData);
            var rect = GetComponent<RectTransform>();
            startingPosition = new Vector3(rect.position.x, rect.position.y);
            if (lineRenderer == null)
            {
                Debug.LogError($"Line renderer list null");
                return;
            }
            lineRenderer.AddPoint(startingPosition + offset);
        }
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
        lineRenderer.RemovePoint(startingPosition);
        lineRenderer.RemovePoint(lastDragPosition);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEnemy)
        {
            BatleSystem.onGetOponentData?.Invoke(gameObject, cardData);
            hoveringOverEnemyCard = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnemy)
        {
            hoveringOverEnemyCard = true;
        }
    }

}
