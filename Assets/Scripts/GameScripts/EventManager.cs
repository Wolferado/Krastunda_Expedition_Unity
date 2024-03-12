using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    private float timerTime = 180;
    [SerializeField] private bool isTimerEnabled;

    public UnityEvent gamePause;
    public UnityEvent gameResume;
    public UnityEvent<float> onTimerTimeDecrement;

    public UnityEvent<GameObject> itemInspectionStart;
    public UnityEvent itemInspectionEnd;
    public UnityEvent<GameObject> itemInteractionStart;
    public UnityEvent itemInteractionSuccessful;
    public UnityEvent<GameObject> itemPickUp;
    public UnityEvent itemDrop;
    public UnityEvent<GameObject> itemCollect;

    public UnityEvent<RaycastHit> cursorHoverOnInspectable;
    public UnityEvent<RaycastHit> cursorHoverOnInteractable;
    public UnityEvent<RaycastHit> cursorHoverOnPickable;
    public UnityEvent<RaycastHit> cursorHoverOnCollectable;
    public UnityEvent cursorHoverOnNothing;

    private bool isGamePaused = false;
    private bool isInspecting = false;
    private bool isHoldingItem = false;
    private bool isCursorHoverInspectable = false;
    private bool isCursorHoverInteractable = false;
    private bool isCursorHoverPickable = false;
    private bool isCursorHoverCollectable = false;

    private GameObject rayCollidedObject;

    // Start is called before the first frame update
    void Start()
    {
        // Many listeners adding
        gamePause.AddListener(delegate { Time.timeScale = 0.0f; isGamePaused = true; });
        gameResume.AddListener(delegate { Time.timeScale = 1.0f; isGamePaused = false; });

        cursorHoverOnInspectable.AddListener(OnCursorHoverOnInspectable);
        cursorHoverOnInteractable.AddListener(OnCursorHoverOnInteractable);
        cursorHoverOnPickable.AddListener(OnCursorHoverOnPickable);
        cursorHoverOnCollectable.AddListener(OnCursorHoverOnCollectable);
        cursorHoverOnNothing.AddListener(ResetHoverValues);

        itemInspectionEnd.AddListener(delegate { isInspecting = false; });
    }

    // Update is called once per frame
    void Update()
    {
        KeyActions();
        CountDownTimer(isGamePaused);
    }

    // Method to count down time.
    private void CountDownTimer(bool isTimerPaused)
    {
        if (isTimerPaused || !isTimerEnabled) // If game is paused or timer is disabled, don't count down
            return;

        if (timerTime <= 0.0f) // If time is up, load bad ending scene
            SceneManager.LoadScene("BadEndingScene");
        
        timerTime -= Time.deltaTime; // Count down time

        onTimerTimeDecrement.Invoke(timerTime); // Invoke an UnityEvent
    }

    // Method that checks for global key presses.
    private void KeyActions()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // If escape is pressed
        {
            OnEscapeKeyPress();
        }
        else if (Input.GetKeyDown(KeyCode.E)) // If interaction key is pressed
        {
            OnInteractKeyPress();
        }
    }

    // Method that gets called when Escape key is pressed.
    private void OnEscapeKeyPress()
    {
        if(isInspecting) // If player is inspecting
        {
            itemInspectionEnd?.Invoke(); // Invoke an UnityEvent for Inspection to end
            return;
        }

        if (Time.timeScale == 1.0f) // If game isn't paused
        {
            gamePause?.Invoke(); // Invoke an UnityEvent to pause a game
        }
        else // If game is paused
        {
            gameResume?.Invoke(); // Invoke an UnityEvent to resume a game
        }
    }

    // Method that gets called from InteractionEvents, if Interaction with Pickedup item was successful.
    public void OnSuccessfulInteractionChangeBoolean()
    {
        isHoldingItem = false;
        itemInteractionSuccessful?.Invoke();
    }

    // Method that gets called when Interaction key is pressed.
    private void OnInteractKeyPress()
    {
        if (isCursorHoverInspectable && !isInspecting) // If cursor hovers inspectable object and player isn't inspecting
        {
            isInspecting = true; // Change boolean value
            itemInspectionStart?.Invoke(rayCollidedObject); // Invoke an UnityEvent for inspection action
        }
        else if (isCursorHoverInteractable && isHoldingItem) // If cursor hovers interactable object and player is holding an item
        {
            itemInteractionStart?.Invoke(rayCollidedObject); // Invoke an UnityEvent for interaction action
        }
        else if (isCursorHoverPickable && !isHoldingItem) // If cursor hovers pickable object and player isn't holding any item
        {
            isHoldingItem = true; // Change boolean value
            itemPickUp?.Invoke(rayCollidedObject); // Invoke an UnityEvent for pick up action
        }
        else if (isHoldingItem) // If cursor hovers inspectable object and player isn't inspecting
        {
            isHoldingItem = false; // Change boolean value
            itemDrop?.Invoke(); // Invoke an UnityEvent for item drop
        }
        else if (isCursorHoverCollectable) // If cursor hovers collectable
        {
            itemCollect?.Invoke(rayCollidedObject); // Invoke an UnityEvent for item collect
        }
    }

    // Method that gets called when cursor hovers inspectable object.
    private void OnCursorHoverOnInspectable(RaycastHit hit)
    {
        if (isHoldingItem)
            return;

        ResetHoverValues();
        isCursorHoverInspectable = true;

        if (hit.transform.gameObject != null)
            rayCollidedObject = hit.transform.gameObject;
    }
    // Method that gets called when cursor hovers interactable object.
    private void OnCursorHoverOnInteractable(RaycastHit hit)
    {
        if (!isHoldingItem)
            return;
        
        ResetHoverValues();
        isCursorHoverInteractable = true;

        if (hit.transform.gameObject != null)
            rayCollidedObject = hit.transform.gameObject;
    }
    // Method that gets called when cursor hovers pickable object.
    private void OnCursorHoverOnPickable(RaycastHit hit)
    {
        ResetHoverValues();
        isCursorHoverPickable = true;

        if (hit.transform.gameObject != null)
            rayCollidedObject = hit.transform.gameObject;
    }
    // Method that gets called when cursor hovers collectable object.
    private void OnCursorHoverOnCollectable(RaycastHit hit)
    {
        ResetHoverValues();
        isCursorHoverCollectable = true;

        if (hit.transform.gameObject != null)
            rayCollidedObject = hit.transform.gameObject;
    }
    // Method that resets all boolean values for hover events.
    private void ResetHoverValues()
    {
        isCursorHoverInspectable = false;
        isCursorHoverInteractable = false;
        isCursorHoverPickable = false;
        isCursorHoverCollectable = false;
        rayCollidedObject = null;
    }
}
