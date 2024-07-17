using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lobby
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject playModePanel;

        [Header("Settings")]
        [SerializeField] private GameObject settingPanel;
        [SerializeField] private TMP_Dropdown difficultyOption;
        [SerializeField] private TMP_Dropdown mazeSizeOption;

        [Header("Sounds")]
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private Slider masterVolumeSlider;
        private float masterVolume;

        [SerializeField] private Slider bgmVolumeSlider;
        private float bgmVolume;

        [SerializeField] private Slider sfxVolumeSlider;
        private float sfxVolume;

        void Start()
        {
            settingPanel.SetActive(false);
            playModePanel.SetActive(false);

            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0);
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0);

            masterVolumeSlider.value = masterVolume;
            bgmVolumeSlider.value = bgmVolume;
            sfxVolumeSlider.value = sfxVolume;

            audioMixer.SetFloat("MasterVolume", masterVolume);
            audioMixer.SetFloat("BGMVolume", bgmVolume);
            audioMixer.SetFloat("SFXVolume", sfxVolume);

            difficultyOption.value = GameManager.Instance.Difficulty - 1;
            mazeSizeOption.value = GameManager.Instance.MazeSizeOptionValue;
        }


        public void ShowPlayMode()
        {
            // playModePanel.SetActive(true);
            GameManager.Instance.LoadGame();
        }

        public void StartInfiniteMode()
        {
            GameManager.Instance.LoadGame();
        }

        public void StartTimeAttackMode()
        {
            GameManager.Instance.LoadGame();
        }

        public void ShowRank()
        {
            SceneManager.LoadScene("RankScene");
        }

        public void ShowSetting()
        {
            settingPanel.SetActive(true);
        }

        public void CloseSetting()
        {
            settingPanel.SetActive(false);
            PlayerPrefs.Save();
        }

        public void DifficultyChanged()
        {
            GameManager.Instance.SetDifficulty(difficultyOption.value + 1);
        }

        public void MazeSizeChanged()
        {
            switch (mazeSizeOption.value)
            {
                case 0:
                    GameManager.Instance.SetMazeSize(20);
                    break;
                case 1:
                    GameManager.Instance.SetMazeSize(50);
                    break;
                case 2:
                    GameManager.Instance.SetMazeSize(100);
                    break;
            }
            GameManager.Instance.MazeSizeOptionValue = mazeSizeOption.value;
        }

        #region Sound Volume Change

        public void OnMasterVolumeChanged()
        {
            masterVolume = masterVolumeSlider.value;
            audioMixer.SetFloat("MasterVolume", masterVolumeSlider.value);
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }

        public void OnBGMVolumeChanged()
        {
            bgmVolume = bgmVolumeSlider.value;
            audioMixer.SetFloat("BGMVolume", bgmVolumeSlider.value);
            PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        }

        public void OnSFXVolumeChanged()
        {
            sfxVolume = sfxVolumeSlider.value;
            audioMixer.SetFloat("SFXVolume", sfxVolumeSlider.value);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }

        #endregion
    }
}
