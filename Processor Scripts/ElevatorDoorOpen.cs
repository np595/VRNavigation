using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorOpen : MonoBehaviour
{
    //public bool triggers;
    public ElevatorDoor oDoor;
    // Start is called before the first frame update
    void Update()
    {
        oDoor = GetComponentInChildren<ElevatorDoor>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (oDoor != null)
                oDoor.triggers = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (oDoor != null)
                oDoor.triggers = false;
        }
    }
}