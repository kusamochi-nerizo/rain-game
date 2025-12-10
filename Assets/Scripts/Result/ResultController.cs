using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    private Action<DialogResult> FixDialog { get; set; }

    public enum DialogResult
    {
        OK,
        Cancel,
    }

    void Start()
    {
        retryButton.onClick.AddListener(OnOk);
        titleButton.onClick.AddListener(OnCancel);
    }


    public void SetView(int score, Action<DialogResult> onClose)
    {
        FixDialog = onClose;
        scoreText.text = "Score：" + score.ToString() + "点";
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    private void OnOk()
    {
        this.FixDialog?.Invoke(DialogResult.OK);
        Destroy(this.gameObject);
    }

    private void OnCancel()
    {
        // イベント通知先があれば通知してダイアログを破棄してしまう
        this.FixDialog?.Invoke(DialogResult.Cancel);
        Destroy(this.gameObject);
    }
}