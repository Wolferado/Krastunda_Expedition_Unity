using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadOnCollisionScript : MonoBehaviour
{
    private GameObject player;
    private void Start()
    {
        player = GameObject.Find("Player"); // Get reference to player
    }
    
    // Method that gets called when Player enters the collision.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player) // If collision is one of players
            FunctionOnCollision(); // Activate needed functionality
    }

    private void FunctionOnCollision()
    {
        // Collision for an Arch in CaveScene (loads main game scene)
        if(this.gameObject.name == "ArchGateway")
        {
            SceneManager.LoadScene("MainScene");
        }
        else if (this.gameObject.name == "PlazaHelipadEntry") // Collision for plaza entry (teleports player to helipad)
        {
            player.transform.position = new Vector3(4, 12.85f, 153);
        }
        else if (this.gameObject.name == "PlazaHelipadExit") // Collision for plaza exit (teleports player outside of plaza to the entry)
        {
            player.transform.position = new Vector3(41, 1, 187);
        }
    }
}
