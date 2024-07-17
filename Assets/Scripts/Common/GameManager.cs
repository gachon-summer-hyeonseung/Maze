using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DifficultyType
{
    Easy = 1,
    Normal,
    Hard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private float plusTime = 10.0f;
    [SerializeField]
    private int plusScoreAmount = 50;

    public int MazeSize { get; private set; }
    public bool Playing => playing;
    private bool playing = false;

    public float LeftPlayTime => leftPlayTime;
    private float leftPlayTime = 0.0f;
    private float totalPlayTime = 0.0f;
    private float currPlayTime = 0.0f;

    public int Score => score;
    private int score = 0;
    private int plusScore = 0;

    public int Difficulty => difficulty;
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
            currPlayTime += Time.deltaTime;
            if (leftPlayTime <= 0)
            {
                leftPlayTime = 0;
                OnTimeEnded();
            }
            UpdateScore();
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

    public void AddTime()
    {
        leftPlayTime += plusTime;
    }

    public void AddScore()
    {
        plusScore += plusScoreAmount;
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
        totalPlayTime = 60.0f * 3.0f / difficulty;
        leftPlayTime = totalPlayTime;
        currPlayTime = 0.0f;
        plusScore = 0;

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
        // score = (int)(totalPlayTime - currPlayTime) * difficulty + plusScore;
        score = plusScore;
        SaveScore();
        SceneManager.LoadScene("RankScene");
    }

    void UpdateScore()
    {
        // score = (int)(totalPlayTime - currPlayTime) * difficulty + plusScore;
        score = plusScore;
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
