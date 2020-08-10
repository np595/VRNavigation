using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public DoorScript oDoor;
    // Start is called before the first frame update
    void Start()
    {
        oDoor = GetComponentInChildren<DoorScript>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(oDoor != null)
                oDoor.triggers = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if(oDoor != null)
                oDoor.triggers = false;
        }
    }
}