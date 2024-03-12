using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private Button resetSettingsButton;

    [Header("Music Volume UI")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeValueLabel;
    public UnityEvent<float> onMusicVolumeChanged;

    [Header("Sound Volume UI")]
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private TextMeshProUGUI soundVolumeValueLabel;
    public UnityEvent<float> onMasterVolumeChanged;

    [Header("Sensitivity X UI")]
    [SerializeField] private Slider sensitivityXSlider;
    [SerializeField] private TextMeshProUGUI sensitivityXValueLabel;
    public UnityEvent<float> onSensitivityXChange;

    [Header("Sensitivity Y UI")]
    [SerializeField] private Slider sensitivityYSlider;
    [SerializeField] private TextMeshProUGUI sensitivityYValueLabel;
    public UnityEvent<float> onSensitivityYChange;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("SensX"))
            CreatePlayerPrefs();

        // Listeners
        resetSettingsButton.onClick.AddListener(ResetSettings);
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnChangeMusicVolume(); });
        soundVolumeSlider.onValueChanged.AddListener(delegate { OnChangeSoundVolume(); });
        sensitivityXSlider.onValueChanged.AddListener(delegate { OnChangeSensitivityX(); });
        sensitivityYSlider.onValueChanged.AddListener(delegate { OnChangeSensitivityY(); });
    }

    void Awake()
    {
        ApplySettings();
    }

    private void CreatePlayerPrefs()
    {
        PlayerPrefs.SetInt("MusicVolume", 80);
        PlayerPrefs.SetInt("SoundVolume", 100);
        PlayerPrefs.SetFloat("SensX", (float)Math.Round(3.0f, 2));
        PlayerPrefs.SetFloat("SensY", (float)Math.Round(2.0f, 2));
    }

    // Method to apply settings and change needed labels and values.
    public void ApplySettings()
    {
        musicVolumeSlider.value = PlayerPrefs.GetInt("MusicVolume");
        musicVolumeValueLabel.text = PlayerPrefs.GetInt("MusicVolume").ToString() + " %";

        soundVolumeSlider.value = PlayerPrefs.GetInt("SoundVolume");
        soundVolumeValueLabel.text = PlayerPrefs.GetInt("SoundVolume").ToString() + " %";
        sensitivityXSlider.value = (float)Math.Round(PlayerPrefs.GetFloat("SensX"), 2);
        sensitivityXValueLabel.text = Math.Round(PlayerPrefs.GetFloat("SensX"), 2).ToString();

        sensitivityXSlider.value = 3.0f;
        sensitivityXValueLabel.text = "3"; 

        sensitivityYSlider.value = (float)Math.Round(PlayerPrefs.GetFloat("SensY"), 2);
        sensitivityYValueLabel.text = Math.Round(PlayerPrefs.GetFloat("SensY"), 2).ToString();

        sensitivityYSlider.value = 2.0f;
        sensitivityYValueLabel.text = "2";
    }

    // Method that changes Music Volume on slider change.
    public void OnChangeMusicVolume()
    {
        PlayerPrefs.SetInt("MusicVolume", (int)musicVolumeSlider.value);
        musicVolumeValueLabel.SetText(musicVolumeSlider.value.ToString() + " %");
        onMusicVolumeChanged.Invoke(musicVolumeSlider.value / 100);
    }
    // Method that changes Sound Volume on slider change.
    public void OnChangeSoundVolume()
    {
        PlayerPrefs.SetInt("SoundVolume", (int)soundVolumeSlider.value);
        soundVolumeValueLabel.SetText(soundVolumeSlider.value.ToString() + " %");
        onMasterVolumeChanged.Invoke(soundVolumeSlider.value / 100);
    }
    // Method that changes Sensitivity X on slider change.
    public void OnChangeSensitivityX()
    {
        PlayerPrefs.SetFloat("SensX", (float)Math.Round(sensitivityXSlider.value, 2));
        sensitivityXValueLabel.SetText(Math.Round(sensitivityXSlider.value, 2).ToString());
        onSensitivityXChange?.Invoke(sensitivityXSlider.value);
    }
    // Method that changes Sensitivity Y on slider change.
    public void OnChangeSensitivityY()
    {
        PlayerPrefs.SetFloat("SensY", (float)Math.Round(sensitivityYSlider.value, 2));
        sensitivityYValueLabel.SetText(Math.Round(sensitivityYSlider.value, 2).ToString());
        onSensitivityYChange?.Invoke(sensitivityYSlider.value);
    }
    // Method that resets settings to theirs original values.
    public void ResetSettings()
    {
        musicVolumeSlider.value = 100;
        musicVolumeValueLabel.text = "100 %";
        soundVolumeSlider.value = 80;
        soundVolumeValueLabel.text = "80 %";
        sensitivityXSlider.value = 3.0f;
        sensitivityXValueLabel.text = "3";
        sensitivityYSlider.value = 2.0f;
        sensitivityYValueLabel.text = "2";

        onMasterVolumeChanged?.Invoke(soundVolumeSlider.value / 100);
        onMusicVolumeChanged?.Invoke(musicVolumeSlider.value / 100);
    }
}
