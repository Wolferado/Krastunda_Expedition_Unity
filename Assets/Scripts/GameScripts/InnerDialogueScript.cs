using Assets.Scripts.GameScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class InnerDialogueScript : MonoBehaviour
{
    [SerializeField] private GameObject innerDialogueWindow;
    [SerializeField] private TextMeshProUGUI dialogueText;
    public AudioManager audioManagerScript;

    private Dictionary<string, string> dialogueLines = new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        audioManagerScript = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        // Adding story lines for inner dialogue
        dialogueLines.Add("CrystalCaveTrigger", "Should I pickup the Crystal?");
        dialogueLines.Add("BrokenShelterTrigger", "I doubt I will be able to open this door.");
        dialogueLines.Add("GameObjectiveTrigger", "I need to get out of here and fast! Aliens won't tolerate me being here, especially after I visited their tomb.");
        dialogueLines.Add("KeyCardTrigger", "A keycard? Maybe I can use it to access one of the shelters...");
        dialogueLines.Add("StarshipKeysTrigger", "Keys for a starship? I can get out of here.");
    }

    // Method that gets called, when Player enters trigger
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.name == "Player") // If it player
        {
            audioManagerScript.PlaySound("NotificationSound");
            StartCoroutine(ShowDialogue(this.gameObject.name)); // Show a dialogue
        }
    }

    private IEnumerator ShowDialogue(string colliderName)
    {
        float timeForDialogue = dialogueLines[colliderName].Length * 0.05f + 1.5f; // Get the time for a dialogue to show
        dialogueText.SetText(dialogueLines[colliderName]); // Get the text based on the trigger name
        this.gameObject.GetComponent<BoxCollider>().enabled = false; // Disable trigger's collider (to allow picking up items, if they are inside the trigger)
        innerDialogueWindow.SetActive(true); // Show inner dialogue window 
        yield return new WaitForSeconds(timeForDialogue); // Wait for a time
        innerDialogueWindow.SetActive(false); // Hide inner dialogue window
        Destroy(this.gameObject); // Destroy trigger
    }
}
