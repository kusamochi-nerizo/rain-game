using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RainGameManager : SingletonMonoBehaviour<RainGameManager>
{
    [SerializeField] private Timer timer;
    [SerializeField] private ResultController resultController;
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private TargetFactory targetFactory;
    [SerializeField] private TargetFactory targetFactory2;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TutorialController tutorialController;
    [SerializeField] private TextAnimationController StartTextAnimationController;
    [SerializeField] private TextAnimationController EndTextAnimationController;

    protected override void doAwake()
    {
        DOTween.Init();
        timer.GameOverSubject.Subscribe(_ => OnGameover()).AddTo(this);
    }

    private void Start()
    {
        Pose();
        timer.Init();
        itemFactory.Init();
        targetFactory.Init();
        targetFactory2.Init();
       
        ShowTutorial();
        SoundManager.Instance.StopBgm();
    }

    void Pose()
    {
        Time.timeScale = 0f;
    }

    void Resume()
    {
        Time.timeScale = 1f;
    }

    void ShowTutorial()
    {
        tutorialController.SetView(ShowStarAnimation);
        tutorialController.Show();
    }

    void ShowStarAnimation()
    {
        StartTextAnimationController.PlayAnimation(GameStart);
    }

    void GameStart()
    {
        Resume();
        playerController.Init();
        SoundManager.Instance.PlayBgm("Game");
    }


    // ゲームが終了したときに実行される処理
    void OnGameover()
    {
        // Time.timeScale = 0f;
        itemFactory.Dispose();
        targetFactory.Dispose();
        targetFactory2.Dispose();
        playerController.Dispose();
        ShowEnsAnimation();
    }

    void ShowEnsAnimation()
    {
        EndTextAnimationController.PlayAnimation(ShowResult);
    }

    void ShowResult()
    {
        resultController.SetView(ScoreManager.Instance.Score, OnResultAction);
        resultController.Show();
    }

    void OnResultAction(ResultController.DialogResult result)
    {
        if (result == ResultController.DialogResult.OK)
        {
            SceneManager.LoadScene("Game");
            // Initiate.Fade("Game", Color.black, 0.5f);
        }
        else
        {
            SceneManager.LoadScene("Title");
            // Initiate.Fade("Title", Color.black, 0.5f);
        }
    }
}