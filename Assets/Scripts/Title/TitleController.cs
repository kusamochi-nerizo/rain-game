using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [SerializeField] private Button startButton;

    void Start()
    {
        SoundManager.Instance.PlayBgm("title");
        startButton.onClick.AddListener(OnClickStart);
    }

    void OnClickStart()
    {
        SceneManager.LoadScene("Game");
        // Initiate.Fade("Game", Color.black, 0.5f);
    }
}
