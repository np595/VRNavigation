using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will be updated the tagging script to place this script on any sand objects

public class SandMovement : MonoBehaviour
{
    public PlayerMovement movements;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player")) //If the player enters the sand collider then reduce speed
        {
            movements.forwardForce = 6f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            movements.forwardForce = 10f;
        }
    }
}
