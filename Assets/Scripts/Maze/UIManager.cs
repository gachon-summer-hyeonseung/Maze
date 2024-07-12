using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Maze
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI scoreText;

        // Update is called once per frame
        void Update()
        {
            UpdateTime();
            UpdateScore();
        }

        void UpdateTime()
        {
            timeText.text = "시간 : " + GameManager.Instance.LeftPlayTime.ToString("F2");
        }

        void UpdateScore()
        {
            scoreText.text = "점수 : " + GameManager.Instance.Score.ToString();
        }
    }
}
