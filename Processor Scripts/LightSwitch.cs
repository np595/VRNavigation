using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    void Start() //Leaves the boxcollider and script enabled so it will always run on trigger when someone enters or leaves the room
    {
        
    }
    // Start is called before the first frame update
    //This will be put on LightRadar
    void OnTriggerEnter(Collider other) //Turn on lights
    {
        if (other.tag == "Player")
        {
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<Light>().enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other) //Turn off lights
    {
        if (other.tag == "Player")
        {
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<Light>().enabled = false;
            }
        }
    }
}
