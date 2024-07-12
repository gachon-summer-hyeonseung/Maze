using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI lastScoreText;

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        highScoreText.text = highScore + " 점";
        lastScoreText.text = lastScore + " 점";
    }

    public void ToLobby()
    {
        GameManager.Instance.ToLobby();
    }
}
