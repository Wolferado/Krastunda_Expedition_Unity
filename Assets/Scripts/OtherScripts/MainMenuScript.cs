using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button aboutUsButton;
    [SerializeField] private Button exitGameButton;

    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject aboutWindow;

    [SerializeField] private GameDataSO gameDataSO;

    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        aboutUsButton.onClick.AddListener(OpenAboutUs);
        exitGameButton.onClick.AddListener(ExitGame);

        mainCamera = Camera.main;

        gameDataSO = ScriptableObject.CreateInstance<GameDataSO>();
    }

    void Awake()
    {
        gameDataSO.CrystalCollected = false;
        gameDataSO.GameStarted = false;
    }

    void LateUpdate()
    {
        RotateCamera();
    }

    // Method for "Start Game" button.
    private void StartGame()
    {
        SceneManager.LoadScene("CaveScene"); // Load CaveScene.
    }

    // Method for "Open Settings" button.
    private void OpenSettings()
    {
        if(aboutWindow.activeInHierarchy) // If about us is opened, hide it
            aboutWindow.SetActive(false);

        if(settingsWindow.activeInHierarchy == true) // If settings are opened, hide it. Otherwise open it.
            settingsWindow.SetActive(false);
        else
            settingsWindow.SetActive(true);
    }

    // Method for "About Us" button (simillar to OpenSettings method).
    private void OpenAboutUs()
    {
        if(settingsWindow.activeInHierarchy)
            settingsWindow.SetActive(false);

        if (aboutWindow.activeInHierarchy == true)
            aboutWindow.SetActive(false);
        else
            aboutWindow.SetActive(true);
    }

    // Method for "Exit Game" button.
    private void ExitGame()
    {
        Application.Quit();
    }

    // Method to constantly rotate the camera for a pleasant visual effect.
    private void RotateCamera()
    {
        mainCamera.transform.Rotate(0, 0.001f, 0, 0);
    }
}
