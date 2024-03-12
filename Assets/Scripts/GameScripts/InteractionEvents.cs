using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InteractionEvents : MonoBehaviour
{
    // Overall variables for action setup
    private GameObject player;
    private GameObject objectInAction;
    private GameObject inspectableObject;
    private GameObject pickedUpObject;

    // Values for objects that are being affected by actions
    private Vector3 originObjectPosition;
    private Quaternion originObjectRotation;
    private Vector3 originObjectScale;

    // Variables for Inspect action
    private Vector3 inspectedObjectPosition;
    private bool isInspecting = false;
    
    // Variables for Pickup action
    private bool isHolding = false;
    Vector3 offsetHoldingPos = new Vector3(0, -0.30f, 0);

    [SerializeField] private GameDataSO gameDataSO;
    EventManager eventManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get player
        player = GameObject.Find("Player");

        eventManagerScript = GameObject.Find("EventSystem").GetComponent<EventManager>();

        // Set up listeners for Inspect action
        eventManagerScript.itemInspectionStart.AddListener(OnInspect);
        eventManagerScript.itemInspectionEnd.AddListener(OnInspectRelease);

        // Set up listeners for Interact action (Key-Lock system)
        eventManagerScript.itemInteractionStart.AddListener(OnInteract);

        // Set up listeners for Pickup action
        eventManagerScript.itemPickUp.AddListener(OnPickup);
        eventManagerScript.itemDrop.AddListener(OnPickupRelease);

        // Set up listeners for Collect action
        eventManagerScript.itemCollect.AddListener(OnCollect);
    }

    void Update()
    {
        if (isInspecting) // If player is inspecting an item, allow to rotate it and change camera FOV
        {
            ChangeCameraFOV();
            AllowItemRotationInInspectView();
        }
        else if (!isInspecting) // If player isn't inspecting, return normal camera FOV
        {
            ChangeCameraFOV();
        }
    }

    void LateUpdate()
    {
        if (isHolding) // If player is holding an item, move it along with the player
        {
            MovePickedUpItem();
        }
    }

    // Method that activates on start of Inspect action.
    private void OnInspect(GameObject objectToInspect)
    {
        objectInAction = objectToInspect; // Get object that is in action (for later purposes)
        inspectableObject = Instantiate(objectToInspect, objectInAction.transform.position, objectInAction.transform.rotation); // Create a copy of inspectable object.
        
        inspectableObject.GetComponent<ParticleSystem>().Stop(); // Stop Particle System for copy
        objectInAction.GetComponent<ParticleSystem>().Stop(); // Stop Particle System for original
        objectInAction.SetActive(false);
        //objectInAction.GetComponent<Renderer>().enabled = false; // Make original object invisible, but active.

        inspectableObject.transform.position = objectInAction.transform.position; // WTF, it just teleported outside of the map

        // Get its data about position, rotation and scale.
        originObjectPosition = objectToInspect.transform.position;
        originObjectRotation = objectToInspect.transform.rotation;
        originObjectScale = objectToInspect.transform.localScale;

        // Destroy rigidbody of the copy to prevent unexpected movements.
        Destroy(inspectableObject.GetComponent<Rigidbody>());

        // Create a position in front of the main camera.
        inspectedObjectPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.4f;

        // Create a mini scale for the object that is going to be inspected.
        Vector3 newScale = new Vector3(originObjectScale.x * 0.25f, originObjectScale.y * 0.25f, originObjectScale.z * 0.25f);

        // Update inspectable object position, scale.
        LeanTween.scale(inspectableObject, newScale, 0.25f);
        LeanTween.move(inspectableObject, inspectedObjectPosition, 0.25f);

        // Set boolean to true.
        isInspecting = true;
    }

    // Method that activates on end of Inspect action.
    private void OnInspectRelease()
    {
        // Move copy of the object to its original place and scale it to original size.
        LeanTween.move(inspectableObject, originObjectPosition, 0.25f);
        LeanTween.rotate(inspectableObject, originObjectRotation.eulerAngles, 0.25f);
        LeanTween.scale(inspectableObject, originObjectScale, 0.25f).setOnComplete(delegate () {
            // Destroy the copy.
            Destroy(inspectableObject);
            // Turn original object visible.
            objectInAction.SetActive(true);
            //objectInAction.GetComponent<MeshRenderer>().enabled = true;
            // Enable Particle System.
            objectInAction.GetComponent<ParticleSystem>().Play(); 
            // Clear the handle for the original.
            objectInAction = null;
            // Set boolean to false.
            isInspecting = false;
        });
    }
    // Method that changes Camera Field of View when inspecting an item.
    private void ChangeCameraFOV()
    {
        if(isInspecting)
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 30, 5.0f * Time.deltaTime);
        else if (!isInspecting && Camera.main.fieldOfView < 60)
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, 5.0f * Time.deltaTime);
    }

    // Method to rotate object in Inspect view.
    private void AllowItemRotationInInspectView()
    {
        // If mouse is pressed down and being hold
        if (Input.GetMouseButton(0)) { 
            // Get rotations based on mouse input
            float rotationX = Input.GetAxis("Mouse Y") * 5.0f;
            float rotationY = Input.GetAxis("Mouse X") * 7.5f;

            // Rotate the object on both axis
            inspectableObject.transform.Rotate(Vector3.left, rotationY, Space.World);
            inspectableObject.transform.Rotate(Vector3.forward, rotationX, Space.World);
        }
    }
    // Method that allows interaction with "Key -> Lock" principle.
    private void OnInteract(GameObject objectLock)
    {
        // Creating a variable
        string tag = String.Empty;

        if (pickedUpObject.CompareTag(objectLock.tag)) // Check if Key -> Lock pair is valid
        {
            tag = objectLock.tag; // Assign a tag
            isHolding = false; // Change boolean value
            eventManagerScript.OnSuccessfulInteractionChangeBoolean(); // Tell EventManager that action is successful
            Destroy(pickedUpObject); // Destroy picked up object
            Destroy(objectLock); // Destroy object that has paired with the previous object
        }

        if (tag != String.Empty) // If tag isn't empty
        {
            switch (tag) {
                case "Key-Ship": // If it were keys and starship
                    SceneManager.LoadScene("GoodEndingScene"); // Load good ending scene
                    break;
            }
        }
    }

    // Method that activates on start of Pickup action.
    private void OnPickup(GameObject objectToPickup)
    {
        pickedUpObject = objectToPickup; // Save item
        pickedUpObject.GetComponent<Rigidbody>().useGravity = false; // Disable gravity to stop it from lagging in the air
        pickedUpObject.layer = 9; // Set it to layer "PickedPickable" to prevent various collisions with the player

        originObjectRotation = pickedUpObject.transform.rotation; // Save origin rotation
        
        // Animate it to the picked up position
        LeanTween.move(pickedUpObject, Camera.main.transform.position + (Camera.main.transform.forward * 0.6f) + offsetHoldingPos + Camera.main.transform.right * 0.3f, 0.5f).setOnComplete(delegate () { isHolding = true; }); 
        
        // Disable particle system
        pickedUpObject.GetComponent<ParticleSystem>().Stop();
    }

    // Method that activates on end of Pickup action.
    private void OnPickupRelease()
    {
        pickedUpObject.GetComponent<Rigidbody>().useGravity = true; // Enable the gravity of the object.
        pickedUpObject.layer = 8; // Set its layer back to "Pickable"
        pickedUpObject.transform.position = player.transform.position + (Camera.main.transform.forward * 1.0f); // Put it in front of the player
        pickedUpObject.transform.rotation = originObjectRotation; // Assign its origin rotation
        pickedUpObject.GetComponent<ParticleSystem>().Play(); // Enable particle system
        pickedUpObject = null; // Make variable a null
        isHolding = false; // Change boolean value
    }

    // Method to move item that is being held along with the player.
    private void MovePickedUpItem()
    {
        pickedUpObject.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 0.6f) + offsetHoldingPos + Camera.main.transform.right * 0.3f;
        pickedUpObject.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(90, 1, 180);
    }

    private void OnCollect(GameObject objectToCollect)
    {
        // If it is a crystal, assign to scriptable object that information.
        if(objectToCollect.name == "Crystal")
        {
            gameDataSO.CrystalCollected = true;   
        }

        Vector3 offset = new Vector3(0, -1, 0); // Offset for the position of collection
        Destroy(objectToCollect.GetComponent<CapsuleCollider>()); // Destroy collider so item will go inside the player
        
        // Animate the object being picked up and destroy it once animation ends.
        LeanTween.move(objectToCollect, player.transform.position + offset, 0.25f).setOnComplete(delegate () { Destroy(objectToCollect); });
    }
}
