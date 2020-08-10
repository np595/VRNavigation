using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElevatorMotion : MonoBehaviour
{
    //ElevatorCommand eCommand;
    bool moveUp;
    bool moveDown;
    float maxY = 0f;
    float minY = 0f;
    ElevatorCommand eCommand;
    // Start is called before the first frame update
    // Update is called once per frame
    void Start()
    {
        moveUp = false;
        moveDown = false;
        var edoors = GameObject.FindGameObjectsWithTag("Elevator");
        foreach (var edoor in edoors)
        {
            if (edoor.transform.position.y - edoor.transform.localScale.y / 2 > maxY)
                maxY = edoor.transform.position.y - edoor.transform.localScale.y / 2 - gameObject.transform.localScale.y / 2;

            if (edoor.transform.position.y - edoor.transform.localScale.y / 2 < minY)
                minY = edoor.transform.position.y - edoor.transform.localScale.y / 2;
        }
    }

    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    { //I'm leaving the old child comments in just in case we move back to trying to make it work as parent and child movement, in case mesh collider ends up having more bugs.
    //Testing shows it is fine as the meshcollider moves the player upwards with it.
        //if (transform.childCount > 0)
        //{
        //ElevatorCommand eCommand = GetComponentInChildren<ElevatorCommand>(); //Gets the player's elevator command script to control the elevator
        
        eCommand = other.GetComponent<ElevatorCommand>();
        moveUp = eCommand.moveUp;
        moveDown = eCommand.moveDown;
        //}

        if (moveUp == true) //Moves elevator up once U key is pressed, press again to stop
        {
            if (transform.position.y >= maxY)
            {
                eCommand.moveUp = false;
                return;
            }
            transform.position += transform.up * Time.deltaTime;
        }
        else if (moveDown == true) //Moves elevator down once J key is pressed, press again to stop
        {
            if (transform.position.y <= minY)
            {
                eCommand.moveDown = false;
                return;
            }
            transform.position -= transform.up * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //other.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider other) //Destroys Elevator when leaving the elevator
    {
        if (other.tag == "Player")
        { //Setting it to null will send the player into space, at least it did when I tested
          //other.transform.parent = null;
            eCommand.moveUp = false;
            eCommand.moveDown = false;
            Destroy(gameObject);
        }
    }
}
