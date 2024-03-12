using Assets.Scripts.GameScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    Rigidbody playerRb;
    Camera playerCamera;

    private float speed = 2.0f;
    private float maxSpeed = 6.0f;
    public float verticalInput = 0.0f;
    public float horizontalInput = 0.0f;

    public bool isGamePaused = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>(); // Getting rigidbody
        playerCamera = Camera.main; // Getting main camera

        // Communication with EventManager
        EventManager eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();
        eventManager.gamePause.AddListener(ToggleIsGamePaused);
        eventManager.gameResume.AddListener(ToggleIsGamePaused);
        eventManager.itemInspectionStart.AddListener(ToggleIsGamePaused);
        eventManager.itemInspectionEnd.AddListener(ToggleIsGamePaused);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMove();
    }

    // Method that gets called each frame if game isn't paused (player movement).
    private void PlayerMove()
    {
        if (isGamePaused)
            return;

        // Get vectors of the main camera (where it looks).
        Vector3 forwardCamera = playerCamera.transform.forward;
        Vector3 rightCamera = playerCamera.transform.right;
        forwardCamera.y = 0f;
        rightCamera.y = 0f;

        // Get keyboard input.
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Vectors relative to the camera
        Vector3 forwardRelativeToCamera = verticalInput * forwardCamera;
        Vector3 rightRelativeToCamera = horizontalInput * rightCamera;
        Vector3 moveDirection = forwardRelativeToCamera + rightRelativeToCamera;

        if (playerRb.velocity.magnitude < maxSpeed) // If player hasn't reached his maximum speed
        {
            playerRb.AddForce(moveDirection * speed, ForceMode.Force); // Add more force to him
        }
    }

    // Method to toggle isGamePaused variable.
    private void ToggleIsGamePaused()
    {
        isGamePaused = !isGamePaused;
    }

    // Method to toggle isGamePaused variable (for methods that need GameObject as parameter).
    private void ToggleIsGamePaused(GameObject gameObject)
    {
        isGamePaused = !isGamePaused;
    }
}
