using TMPro;
using UniRx;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float totalTime = 60f; // 制限時間（秒）
    private float currentTime; // 現在の経過時間
    public TextMeshProUGUI timeText; // UIテキストオブジェクト
    public Subject<Unit> GameOverSubject = new Subject<Unit>();
    private bool isGameOver = false;
    
    void Start()
    {
        currentTime = totalTime;
    }

    public void Init()
    {
        currentTime = totalTime;
        isGameOver = false;
    }

    void FixedUpdate()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // 経過時間を減算
            if (currentTime < 0)
            {
                currentTime = 0;
            }

            // 残り時間をテキストで表示
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
        else if (isGameOver == false)
        {
            isGameOver = true;
            // 制限時間が終了した場合の処理をここに追加
            timeText.text = "Time's up!";
            GameOverSubject.OnNext(Unit.Default);
        }
    }
}