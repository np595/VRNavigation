using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;

public class ElevatorSpawn : MonoBehaviour
{
    GameObject cube;

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other) //Spawns elevators when players ender the elevator door trigger
    {
        var elevators = GameObject.FindGameObjectsWithTag("Elevator Floor");
        if (elevators == null || elevators.Length == 0)
        {
            if (other.tag == "Player")
            {
                GameObject child = gameObject.transform.GetChild(0).gameObject;
                Vector3 size = child.GetComponent<Renderer>().bounds.size;
                Vector3 ElevatorPosition = new Vector3(0, 0, 0);
                Vector3 playerPosition = other.transform.position;
                bool flag = true;
                if (size.x > size.z)
                    flag = false;

               

                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Elevator";
                cube.transform.localScale = new Vector3(2.5f, 1f, 2.5f);
                BoxCollider box = cube.GetComponent<BoxCollider>();
                if (!flag)
                {
                    if (playerPosition.z < child.transform.position.z)
                    {
                        ElevatorPosition = new Vector3(child.transform.position.x - child.transform.localScale.x / 2, child.transform.position.y - (child.transform.localScale.y / 2 + cube.transform.localScale.y / 2), child.transform.position.z + 1);
                    }
                    else
                    {
                        ElevatorPosition = new Vector3(child.transform.position.x - child.transform.localScale.x / 2, child.transform.position.y - (child.transform.localScale.y / 2 + cube.transform.localScale.y / 2), child.transform.position.z - 1);
                    }
                    box.size = new Vector3(1f, 15, 2f); //The box is slightly larger to allow the player to get off the elevator completely first before it despawns
                    box.isTrigger = true;
                }
                else
                {
                    if (playerPosition.x < child.transform.position.x)
                    {
                        ElevatorPosition = new Vector3(child.transform.position.x + 1, child.transform.position.y - (child.transform.localScale.y / 2 + cube.transform.localScale.y / 2), child.transform.position.z - child.transform.localScale.z / 2);
                    }
                    else
                    {
                        ElevatorPosition = new Vector3(child.transform.position.x - 1, child.transform.position.y - (child.transform.localScale.y / 2 + cube.transform.localScale.y / 2), child.transform.position.z - child.transform.localScale.z / 2);
                    }
                    box.size = new Vector3(2f, 15, 1f); //The box is slightly larger to allow the player to get off the elevator completely first before it despawns
                    box.isTrigger = true;
                }
                cube.transform.position = ElevatorPosition;
                cube.tag = "Elevator Floor";

                

                if (cube.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = cube.AddComponent<MeshCollider>() as MeshCollider;
                }

                if (cube.GetComponent<ElevatorMotion>() == null) //Adds the motion of the elevator on the elevator cube
                {
                    ElevatorMotion eMotion = cube.AddComponent<ElevatorMotion>() as ElevatorMotion;
                }
            }
        }
    }
}
