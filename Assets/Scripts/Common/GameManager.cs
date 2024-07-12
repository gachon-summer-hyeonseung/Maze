using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DifficultyType
{
    Easy,
    Normal,
    Hard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int MazeSize { get; private set; }
    public bool Playing => playing;
    private bool playing = false;

    public float LeftPlayTime => leftPlayTime;
    private float leftPlayTime = 0.0f;
    private float totalPlayTime = 0.0f;

    public int Score => score;
    private int score = 0;

    private int difficulty = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (playing)
        {
            leftPlayTime -= Time.deltaTime;
            if (leftPlayTime <= 0)
            {
                leftPlayTime = 0;
                OnTimeEnded();
            }
        }
    }

    public void SetMazeSize(int size)
    {
        MazeSize = size > 100 ? 100 : size;
    }

    public void SetDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void OnMazeGenerated()
    {
        leftPlayTime = 60.0f * 10.0f / difficulty;
        playing = true;
    }

    void OnTimeEnded()
    {
        playing = false;
        // score = (int)(totalPlayTime - leftPlayTime);
        // SaveScore();
        SceneManager.LoadScene("LobbyScene");
    }

    public void OnClear()
    {
        playing = false;
        score = (int)(totalPlayTime - leftPlayTime);
        SaveScore();
        SceneManager.LoadScene("RankScene");
    }

    void SaveScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
    }
}
