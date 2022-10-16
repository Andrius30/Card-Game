using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorBox : MonoBehaviour
{
    public static Action<string> onError;

    [SerializeField] GameObject backGround;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] Button okBtn;

    void Start() => okBtn.onClick.AddListener(() => ToggleErrorBox(false));

    void Show(string msg)
    {
        ToggleErrorBox(true);
        errorText.text = msg;
    }
    void ToggleErrorBox(bool open) => backGround.SetActive(open);

    void OnEnable()
    {
        onError += Show;
    }
    void OnDisable()
    {
        onError -= Show;
    }
}
