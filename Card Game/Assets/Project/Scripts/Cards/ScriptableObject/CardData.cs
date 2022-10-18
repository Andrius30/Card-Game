using UnityEngine;

[CreateAssetMenu(menuName = "Cards/new Card")]
public class CardData : BaseScriptableObject
{
    public string cardID;
    public GameObject cardPrefab;
    public GameObject placedCardPrefab;
    public Sprite cardIcon;
    public string cardName;
    [TextArea(2, 5)] public string cardDecription;
    public int cost;
    public int power;
    public int cardHealth;

    protected override void OnEnable()
    {
        base.OnEnable();
        cardID = guiId;
    }
}
