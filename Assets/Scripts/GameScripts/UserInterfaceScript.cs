using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterfaceScript : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject gameEssentials;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private Button returnToMainMenuButton;
    private bool isGamePaused = false;

    [Header("Interactions")]
    [SerializeField] private GameObject inspectionWindow;
    [SerializeField] private Image interactionTip;
    [SerializeField] private Sprite[] interactionSprites = new Sprite[5];
    [SerializeField] private RawImage cursor;
    [SerializeField] private Texture[] cursorTextures = new Texture[2];

    [Header("Variables and Events")]
    private bool isInspecting = false;
    private bool isHoldingItem = false;
    private string[] possibleCursorHoverModes = new string[5] { "Inspectable", "Interactable", "Pickable", "Collectable", string.Empty };

    private EventManager eventManagerScript;

    void Start()
    {
        // Pause menu button Listeners
        resumeGameButton.onClick.AddListener(ResumeGameButtonPressed);
        settingsButton.onClick.AddListener(OpenCloseSettings);
        returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Communication with EventManager 
        eventManagerScript = GameObject.Find("EventSystem").GetComponent<EventManager>();

        eventManagerScript.gamePause.AddListener(TogglePauseMenu);
        eventManagerScript.gameResume.AddListener(TogglePauseMenu);
        eventManagerScript.itemInspectionStart.AddListener(OpenInspectWindow);
        eventManagerScript.itemInspectionEnd.AddListener(CloseInspectWindow);
        eventManagerScript.itemPickUp.AddListener(ToggleIsHoldingItem);
        eventManagerScript.itemDrop.AddListener(ToggleIsHoldingItem);
        eventManagerScript.itemInteractionSuccessful.AddListener(ToggleIsHoldingItem);

        eventManagerScript.cursorHoverOnInspectable.AddListener(delegate { ShowInteractionTip(possibleCursorHoverModes[0]); });
        eventManagerScript.cursorHoverOnInteractable.AddListener(delegate { ShowInteractionTip(possibleCursorHoverModes[1]); });
        eventManagerScript.cursorHoverOnPickable.AddListener(delegate { ShowInteractionTip(possibleCursorHoverModes[2]); });
        eventManagerScript.cursorHoverOnCollectable.AddListener(delegate { ShowInteractionTip(possibleCursorHoverModes[3]); });
        eventManagerScript.cursorHoverOnNothing.AddListener(delegate { ShowInteractionTip(possibleCursorHoverModes[4]); });

        eventManagerScript.onTimerTimeDecrement.AddListener(ChangeTimerTime);
    }

    // Method to change UI timers time (https://github.com/ThisIsFix/Unity-UI-Timer/blob/master/UITimer.cs)
    private void ChangeTimerTime(float timeLeft)
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        int milliseconds = Mathf.FloorToInt((timeLeft * 100f) % 100f);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
    }

    // Method that toggles pause menu.
    private void TogglePauseMenu()
    {
        // If player is in the game, but didn't pause it, pause the game
        if (!isGamePaused)
        {
            gameEssentials.SetActive(false); // Hide essentials of the game such as cursor and timer
            pauseMenu.SetActive(true); // Show Pause Menu
        }
        else // Otherwise, un-pause it
        {
            gameEssentials.SetActive(true); // Show game essentials
            pauseMenu.SetActive(false); // Hide Pause Menu

            if (settingsWindow.activeInHierarchy) // If settings window was open
                OpenCloseSettings(); // Close it
        }

        isGamePaused = !isGamePaused; // Change boolean value
    }

    // Method for "Resume Game" button press.
    private void ResumeGameButtonPressed()
    {
        pauseMenu.SetActive(false); // Hide Pause Menu
        eventManagerScript.gameResume?.Invoke(); // Invoke EventManager UnityEvent for game resume action
    }

    // Method to open, close settings window.
    private void OpenCloseSettings()
    {
        if (settingsWindow.activeInHierarchy)
            settingsWindow.SetActive(false);
        else
            settingsWindow.SetActive(true);
    }

    // Method to return to the main menu.
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1.0f; // Dog-nail to prevent freezing each second Game Scene load (time is stopped)
        SceneManager.LoadScene("MainMenuScene"); // Load Main Menu scene.
    }

    // Method to open inspect window.
    private void OpenInspectWindow(GameObject param)
    {
        gameEssentials.SetActive(false); // Hide game essentials
        inspectionWindow.SetActive(true); // Show inspection window
        isInspecting = true; // Change boolean value
    }

    // Method to close inspect window.
    private void CloseInspectWindow()
    {
        if (isInspecting) // If player is inspecting
        {
            gameEssentials.SetActive(true); // Show game essentials
            inspectionWindow.SetActive(false); // Hide inspection window
            isInspecting = false; // Change boolean value
        }
    }

    // Method to show interaction tip, change cursor sprite based on hovered object layer.
    private void ShowInteractionTip(string interactionType)
    {
        if (interactionType == string.Empty) // If hovered nothing
        {
            interactionTip.sprite = interactionSprites[Array.IndexOf(possibleCursorHoverModes, interactionType)];
            cursor.texture = cursorTextures[0];
        }
        else // If hovered something
        {
            if (interactionType == possibleCursorHoverModes[1] && !isHoldingItem) // If hovers interactable, but doesn't have any item in hands
                return; // Do nothing

            interactionTip.sprite = interactionSprites[Array.IndexOf(possibleCursorHoverModes, interactionType)];
            cursor.texture = cursorTextures[1];
        }
    }

    // Method that toggles "isHoldingItem" variable.
    private void ToggleIsHoldingItem()
    {
        isHoldingItem = !isHoldingItem;
    }

    // Method that toggles "isHoldingItem" variable (that requires GameObject parameter).
    private void ToggleIsHoldingItem(GameObject param)
    {
        isHoldingItem = !isHoldingItem;
    }
}
