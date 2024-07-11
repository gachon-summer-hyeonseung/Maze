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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetMazeSize(int size)
    {
        MazeSize = size > 100 ? 100 : size;
    }

    public void StartPlaying()
    {
        SceneManager.LoadScene("GameScene");
    }
}
