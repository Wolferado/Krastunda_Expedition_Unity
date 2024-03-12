using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CameraBehaviour : MonoBehaviour
{
    private GameObject player;

    private EventManager eventManagerScript;

    [SerializeField] private GameDataSO gameDataSO;

    private float sensitivityX;
    private float sensitivityY;
    private float rotationX;
    private float rotationY;

    private Ray cameraRay;

    void Start()
    {
        player = GameObject.Find("Player"); // Getting Player GameObject

        // Set cameras rotationX and rotationY to the players rotation values.
        rotationX = player.transform.rotation.eulerAngles.x;
        rotationY = player.transform.rotation.eulerAngles.y;

        // Communication with EventManager
        eventManagerScript = GameObject.Find("EventSystem").GetComponent<EventManager>();
        eventManagerScript.gamePause.AddListener(ToggleCursorModeAndVisibility);
        eventManagerScript.gameResume.AddListener(ToggleCursorModeAndVisibility);
        eventManagerScript.itemInspectionStart.AddListener(ToggleCursorModeAndVisibility);
        eventManagerScript.itemInspectionEnd.AddListener(ToggleCursorModeAndVisibility);

        // Communication with SettingsScript (to update sensitivity if changed)
        SettingsScript settingsScript = GameObject.Find("SettingsWindow").GetComponent<SettingsScript>();
        GameObject.Find("SettingsWindow").SetActive(false); // Dog-nail to hide SettingsWindow (active to get its reference)
        settingsScript.onSensitivityXChange.AddListener(OnSensitivityXChange);
        settingsScript.onSensitivityYChange.AddListener(OnSensitivityYChange);

        settingsScript.ApplySettings();
    }

    // Method to call, when game is started
    void Awake()
    {
        if (gameDataSO.GameStarted == false) // When game started and boolean is false
        {
            ToggleCursorModeAndVisibility(); // Change cursor visibility and its mode
            gameDataSO.GameStarted = true; // Set boolean to true
        }

        // Set sensitivity with values from Player Preferences
        if (!PlayerPrefs.HasKey("SensX") || !PlayerPrefs.HasKey("SensY"))
        {
            rotationX = 3.0f;
            rotationY = 2.0f;
        }
        else
        {
            sensitivityX = (float)Math.Round(PlayerPrefs.GetFloat("SensX"), 2);
            sensitivityY = (float)Math.Round(PlayerPrefs.GetFloat("SensY"), 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked) // If cursor is locked, move camera freely.
        {
            MoveCamera();
        }
    }

    private void FixedUpdate()
    {
        CastRay(); // Cast a ray each fixed frame
    }

    private void MoveCamera()
    {
        // Set position of the camera to the position of the player.
        gameObject.transform.position = player.transform.position + new Vector3(0, 0.75f, 0);

        // Receive mouse inputs on both axis multiplied by the time and sensitivity.
        float verticalMouse = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivityX * 100;
        float horizontalMouse = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivityY * 100;

        // Add values to the rotation variables.
        rotationY += verticalMouse;
        rotationX -= horizontalMouse;

        // Limit rotation.
        rotationX = Mathf.Clamp(rotationX, -60f, 60f);

        // Assign rotation of the camera.
        transform.rotation = Quaternion.Euler(rotationX + 0.0f, rotationY, 0.0f);
    }

    // Method to cast a Ray to detect on what kind of object Player is looking at.
    private RaycastHit CastRay()
    {
        // Create a new Ray object that has position of the player and casted forward.
        cameraRay = new Ray(transform.position, transform.forward);
        // Create a new RayHit object that detects, when Ray has collided with something.
        RaycastHit hit;
        // Draw a Ray for more understandable logic.
        Debug.DrawRay(transform.position, transform.forward * 3.0f, Color.red);

        // Ray hit logic that detects, what kind of object collision occurred.
        if (Physics.Raycast(cameraRay, out hit, 3f)) // If there is collision
        {
            if (hit.transform.gameObject.layer == 6) // Inspectable
                eventManagerScript.cursorHoverOnInspectable?.Invoke(hit);
            else if (hit.transform.gameObject.layer == 7) // Interactable
                eventManagerScript.cursorHoverOnInteractable?.Invoke(hit);
            else if (hit.transform.gameObject.layer == 8) // Pickable
                eventManagerScript.cursorHoverOnPickable?.Invoke(hit);
            else if (hit.transform.gameObject.layer == 10) // Collectable
                eventManagerScript.cursorHoverOnCollectable?.Invoke(hit);
            else // If there is no collision
                eventManagerScript.cursorHoverOnNothing?.Invoke();
        }
        else
        {
            eventManagerScript.cursorHoverOnNothing?.Invoke();
        }

        return hit;
    }

    // Method to change X axis sensitivity with parameter.
    private void OnSensitivityXChange(float newSensitivityX)
    {
        sensitivityX = newSensitivityX;
    }
    // Method to change Y axis sensitivity with parameter.
    private void OnSensitivityYChange(float newSensitivityY)
    {
        sensitivityY = newSensitivityY;
    }

    // Method to change cursor mode and its visibility.
    private void ToggleCursorModeAndVisibility()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Method to change cursor mode and its visibility (for GameObject).
    private void ToggleCursorModeAndVisibility(GameObject param)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
