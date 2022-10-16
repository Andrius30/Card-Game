using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int playersJoined = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}
