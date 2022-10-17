using Andrius.Core.Debuging;
using Andrius.Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [ReadOnly] public Transform playerCardField;
    [ReadOnly] public Transform oponentCardField;
    [ReadOnly] public int playersJoined = 0;

    GameObject field;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    void OnGameSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if (scene.buildIndex == 2)
        {
            field = GameObject.Find("Field");
            playerCardField = StaticFunctions.FindChild(field.transform, "PlayerCardField");
            oponentCardField = StaticFunctions.FindChild(field.transform, "OponentCardField");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnGameSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnGameSceneLoaded;

    }
}
