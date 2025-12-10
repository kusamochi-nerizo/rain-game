using TMPro;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    public TextMeshProUGUI scoreText; // TextMeshProのTextオブジェクトをInspectorで関連付ける
    private int score = 0; // スコアの初期値
    
    public int Score => score;

    protected override void doAwake()
    {
    }

    void Start()
    {
        UpdateScoreText(); // 初期状態でのスコアを表示
    }

    // スコアを増やすメソッド
    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText(); // スコアが変更されたら表示を更新
    }

    // スコアを減らすメソッド
    public void DecreaseScore(int amount)
    {
        score = Mathf.Max(0, score - amount); // スコアが負にならないようにする
        UpdateScoreText(); // スコアが変更されたら表示を更新
    }

    // スコアを表示するメソッド
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}