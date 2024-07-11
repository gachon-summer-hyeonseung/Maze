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

            difficultyOption.value = PlayerPrefs.GetInt("Difficulty", 0);
        }


        public void ShowPlayMode()
        {
            playModePanel.SetActive(true);
        }

        public void StartInfiniteMode()
        {
            GameManager.Instance.StartPlaying();
        }

        public void StartTimeAttackMode()
        {
            GameManager.Instance.StartPlaying();
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
            switch (difficultyOption.value)
            {
                case 0:
                    PlayerPrefs.SetInt("Difficulty", 0);
                    break;
                case 1:
                    PlayerPrefs.SetInt("Difficulty", 1);
                    break;
                case 2:
                    PlayerPrefs.SetInt("Difficulty", 2);
                    break;
            }
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
