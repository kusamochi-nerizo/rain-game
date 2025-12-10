using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    private Action closeAction;

    void Start()
    {
        startButton.onClick.AddListener(OnOk);
    }

    public void SetView(Action onClose)
    {
        closeAction = onClose;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    void OnOk()
    {
        this.gameObject.SetActive(false);
        closeAction?.Invoke();
    }
}