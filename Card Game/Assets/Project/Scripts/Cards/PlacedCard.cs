using Fusion;
using UnityEngine;

public class PlacedCard : NetworkBehaviour, ISpawned
{
    public string ID;
    public CardData cardData;

    [HideInInspector] public CardSpawner spawner;

    void Start()
    {
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
}
