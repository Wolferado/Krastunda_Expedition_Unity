using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BadEndingScript : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private GameDataSO gameDataSO;

    // Start is called before the first frame update
    void Start()
    {
        // Add listeners
        mainMenuButton.onClick.AddListener(delegate () { SceneManager.LoadScene("MainMenuScene"); });
        tryAgainButton.onClick.AddListener(delegate () { gameDataSO.CrystalCollected = false; gameDataSO.GameStarted = false; SceneManager.LoadScene("CaveScene"); });
    }

    void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
