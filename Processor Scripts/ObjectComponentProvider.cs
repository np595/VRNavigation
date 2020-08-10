using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectComponentProvider : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 spawn = new Vector3(0, 0, 0);
    public List<String> tagList = new List<String>{ };
    void Start()
    {
        var ogDoors = GameObject.FindGameObjectsWithTag("Door");
        foreach (var ogDoor in ogDoors)
            if (!GameObject.Find(ogDoor.name + "_1"))
                replaceDoor(ogDoor);

        var ogDdoors = GameObject.FindGameObjectsWithTag("Double Door");
        foreach (var ogDdoor in ogDdoors)
            if (!GameObject.Find(ogDdoor.name + "_1"))
                replaceDoubleDoor(ogDdoor);

        var ogEdoors = GameObject.FindGameObjectsWithTag("Elevator");
        foreach (var ogEdoor in ogEdoors)
            if (!GameObject.Find(ogEdoor.name + "_1"))
                replaceElevatorDoor(ogEdoor);

        int totalObj = GameObject.FindGameObjectsWithTag("Door").Length;
        Vector3 doorHolderPosition = transform.position;
        Vector3[] walkways = new Vector3[totalObj];
        int x = 0;

        if (_tagChecker("Door"))
        {
            var doors = GameObject.FindGameObjectsWithTag("Door"); //Only checks tags and not assets
            foreach (var door in doors) //Reads through each object as Door to add the components in it
            {
                if (door.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider box = door.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                }

                if (door.GetComponent<Renderer>() == null) //Renderer will allow for size to be found
                {
                    Renderer render = door.AddComponent<Renderer>() as Renderer;
                }

                if (door.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = door.AddComponent<MeshCollider>() as MeshCollider; //Sets a Box Collider in the object
                }

                Vector3 doorLength = door.GetComponent<Renderer>().bounds.size;
                Vector3 doorPosition = door.transform.position;
                float halfDoor;

                GameObject doorHolder = new GameObject();
                doorHolder.name = "doorHolder";
                Vector3 objSize = door.GetComponent<Renderer>().bounds.size;
                if (objSize.x >= objSize.z) //If the x axis is larger, then put the hinge on the x axis
                {
                    halfDoor = doorLength.x / 2; //Gets half the object size
                    if (Regex.IsMatch(door.name, "_1", RegexOptions.IgnoreCase))
                        doorHolderPosition.Set(doorPosition.x + halfDoor, doorPosition.y, doorPosition.z);
                    else if (Regex.IsMatch(door.name, "_2", RegexOptions.IgnoreCase))
                        doorHolderPosition.Set(doorPosition.x - halfDoor, doorPosition.y, doorPosition.z);

                    doorHolder.transform.position = doorHolderPosition;
                }
                else //If z axis is larger, then put it on the z axis
                {
                    halfDoor = doorLength.z / 2; //Gets half the object size
                    if (Regex.IsMatch(door.name, "_1", RegexOptions.IgnoreCase))
                        doorHolderPosition.Set(doorPosition.x, doorPosition.y, doorPosition.z + halfDoor);
                    else if (Regex.IsMatch(door.name, "_2", RegexOptions.IgnoreCase))
                        doorHolderPosition.Set(doorPosition.x, doorPosition.y, doorPosition.z - halfDoor);
                    doorHolder.transform.position = doorHolderPosition;
                }

                Animator animator = new Animator();

                if (doorHolder.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider boxes = doorHolder.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                    boxes.isTrigger = true;
                }

                if (doorHolder.GetComponent<Animator>() == null)
                {
                    animator = doorHolder.AddComponent<Animator>() as Animator;
                    if (Regex.IsMatch(door.name, "_1", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _1 in it then it puts doorleft
                    {
                        animator.runtimeAnimatorController = Resources.Load("DoorLeft") as RuntimeAnimatorController; //Sets the controller as the Animation Controller for each door
                    }
                    else if (Regex.IsMatch(door.name, "_2", RegexOptions.IgnoreCase))
                    {
                        animator.runtimeAnimatorController = Resources.Load("DoorRight") as RuntimeAnimatorController; //Sets the controller as the Animation Controller for each door
                    }
                }

                if (x < totalObj)
                {
                    if (objSize.x >= objSize.z) //If the x axis is larger, then put the hinge on the x axis
                    {
                        walkways[x++].Set(doorPosition.x, doorPosition.y + 1, doorPosition.z - halfDoor);
                    }
                    else
                    {
                        walkways[x++].Set(doorPosition.x - halfDoor, doorPosition.y + 1, doorPosition.z);

                    }
                }

                door.transform.parent = doorHolder.transform; //door is now the child of doorHolder. Might need to delete the old Door if this doesn't move the original gameObject to be a child of the doorHolder

                if (doorHolder.GetComponent<DoorScript>() == null)
                {
                    DoorScript dScript = doorHolder.AddComponent<DoorScript>() as DoorScript;
                }

                //Creates the object to trigger the rotation of the door
                GameObject doorTrigger = new GameObject(); //Spawns object where the door is
                doorTrigger.name = "doorTrigger";
                doorTrigger.transform.position = door.transform.position;

                doorHolder.transform.parent = doorTrigger.transform;

                if (doorTrigger.GetComponent<SphereCollider>() == null)
                {
                    SphereCollider sCol = doorTrigger.AddComponent<SphereCollider>() as SphereCollider;
                    sCol.radius = 2.5f;
                    sCol.isTrigger = true;
                }

                if (doorTrigger.GetComponent<OpenDoor>() == null)
                {
                    OpenDoor oDoor = doorTrigger.AddComponent<OpenDoor>() as OpenDoor;
                }
            }       //You will need to change the path to the controller if you save it under Resources in the Assets or under another folder
        }

        if (_tagChecker("Wall"))
        {
            var walls = GameObject.FindGameObjectsWithTag("Wall"); //Only checks tags and not assets
            foreach (var wall in walls)
            {

                if (wall.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = wall.AddComponent<MeshCollider>() as MeshCollider; //Sets a Box Collider in the object
                }

            }
        }

        if (_tagChecker("Elevator"))
        {
            var edoors = GameObject.FindGameObjectsWithTag("Elevator"); //Only checks tags and not assets
            foreach (var edoor in edoors)
            {
                if (edoor.GetComponent<BoxCollider>() == null)
                {
                    Vector3 size = edoor.GetComponent<Renderer>().bounds.size;
                    bool flag = true;
                    if (size.x > size.z)
                    {
                        flag = false;
                    }
                    BoxCollider box = edoor.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                    if (flag) {
                        box.size = new Vector3(12, 1, 2);
                        box.center = new Vector3(0, 0, -.5f);
                    }
                    else
                    {
                        box.size = new Vector3(2, 1, 12);
                        box.center = new Vector3(-.5f, 0, 0);
                    }

                    box.isTrigger = true;
                }

                if (edoor.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = edoor.AddComponent<MeshCollider>() as MeshCollider;
                }
                
                GameObject elevatorSlider = new GameObject();
                elevatorSlider.name = "elevatorSlider";
                elevatorSlider.transform.position = edoor.transform.position;
                edoor.transform.parent = elevatorSlider.transform;

                if (Regex.IsMatch(edoor.name, "_1", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _1 in it then it puts doorleft
                {
                    if (elevatorSlider.GetComponent<ElevatorSpawn>() == null)
                    {
                        ElevatorSpawn eSpawn = elevatorSlider.AddComponent<ElevatorSpawn>() as ElevatorSpawn;
                    }
                }

                if (elevatorSlider.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider box = elevatorSlider.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                    box.size = new Vector3(2, 1, 2);
                    box.isTrigger = true;
                }
                
                if (edoor.GetComponent<ElevatorDoor>() == null)
                {
                    ElevatorDoor dScript = edoor.AddComponent<ElevatorDoor>() as ElevatorDoor;
                }

                if (elevatorSlider.GetComponent<ElevatorDoorOpen>() == null)
                {
                    ElevatorDoorOpen oDoor = elevatorSlider.AddComponent<ElevatorDoorOpen>() as ElevatorDoorOpen;
                }
            }
        }

        if (_tagChecker("Water"))
        {
            var waters = GameObject.FindGameObjectsWithTag("Water"); //Only checks tags and not assets
            foreach (var water in waters)
            {
                if (water.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider box = water.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                    box.isTrigger = true;
                }

            }
        }

        if (_tagChecker("Sand"))
        {
            var sands = GameObject.FindGameObjectsWithTag("Sand"); //Only checks tags and not assets
            foreach (var sand in sands)
            {
                if (sand.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider box = sand.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                    box.isTrigger = true;
                }

                if (sand.GetComponent<SandMovement>() == null)
                {
                    SandMovement sMove = sand.AddComponent<SandMovement>() as SandMovement;
                }

                if (sand.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = sand.AddComponent<MeshCollider>() as MeshCollider; //Sets a Box Collider in the object
                }

            }
        }

        if (_tagChecker("Window"))
        {
            var windows = GameObject.FindGameObjectsWithTag("Window"); //Only checks tags and not assets
            foreach (var window in windows)
            {
                if (window.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider box = window.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                    box.isTrigger = true;
                }

                if (window.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = window.AddComponent<MeshCollider>() as MeshCollider; //Sets a Box Collider in the object
                }

            }
        }

        if (_tagChecker("Stair"))
        {
            var stairs = GameObject.FindGameObjectsWithTag("Stair"); //Only checks tags and not assets
            foreach (var stair in stairs)
            {
                if (stair.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = stair.AddComponent<MeshCollider>() as MeshCollider; //Sets a Mesh Collider in the object
                }

            }
        }

        if (_tagChecker("Floor"))
        {
            var floors = GameObject.FindGameObjectsWithTag("Floor"); //Only checks tags and not assets
            foreach (var floor in floors)
            {
                if (floor.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = floor.AddComponent<MeshCollider>() as MeshCollider; //Sets a Mesh Collider in the object
                }
                if (floor.GetComponent<TeleportationArea>() == null)
                {
                    TeleportationArea tele = floor.AddComponent<TeleportationArea>() as TeleportationArea; //adds teleportationArea script to object
                }
            }
        }

        if (_tagChecker("Roof"))
        {
            var roofs = GameObject.FindGameObjectsWithTag("Roof"); //Only checks tags and not assets
            foreach (var roof in roofs)
            {
                if (roof.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mesh = roof.AddComponent<MeshCollider>() as MeshCollider; //Sets a Mesh Collider in the object
                }
            }
        }

        if (_tagChecker("Light"))
        {
            var lights = GameObject.FindGameObjectsWithTag("Light"); //Only checks tags and not assets
            foreach (var light in lights)
            {
                GameObject lightBulb = new GameObject("Light");
                Light lightComp = lightBulb.AddComponent<Light>();
                lightComp.color = Color.white;
                lightComp.enabled = false;
                lightBulb.transform.position = light.transform.position;

                GameObject lightSwitch = new GameObject();
                lightSwitch.name = "lightSwitch";
                lightSwitch.transform.position = light.transform.position;

                if (lightSwitch.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider box = lightSwitch.AddComponent<BoxCollider>() as BoxCollider; //Sets a Box Collider in the object
                    box.size = new Vector3(5, 5, 5); //Will be changed to fit size of the room provided
                    box.isTrigger = true;
                }

                if (lightBulb.GetComponent<MeshRenderer>() == null)
                {
                    MeshRenderer mesh = lightBulb.AddComponent<MeshRenderer>() as MeshRenderer; //Sets a Mesh Collider in the object
                }

                lightBulb.transform.parent = lightSwitch.transform;

                if (lightSwitch.GetComponent<LightSwitch>() == null)
                {
                    LightSwitch lSwitch = lightSwitch.AddComponent<LightSwitch>() as LightSwitch;
                }
            }
        }


        var players = GameObject.FindGameObjectsWithTag("Player"); //Only checks tags and not assets
        foreach (var player in players)
        {
            if (player.GetComponent<CharacterController>() == null)
            {
                CharacterController cControl = player.AddComponent<CharacterController>() as CharacterController;
            }

            string path = Application.dataPath + "/Editor/positions.csv";
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                string fileData = reader.ReadToEnd();
                string[] lines = fileData.Split('\n');
                List<Vector3> positions = new List<Vector3>();
                foreach (string coordinates in lines)
                {
                    if (String.IsNullOrWhiteSpace(coordinates))
                    {
                        break;
                    }
                    string[] values = coordinates.Split(',');
                    Vector3 vec = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                    positions.Add(vec);
                }
                spawn = positions[UnityEngine.Random.Range(0, positions.Count)];
                player.transform.position = spawn;
                reader.Close();

            }
            else
            {
                spawn = walkways[UnityEngine.Random.Range(0, walkways.Length)]; //Randomly spawns player in front of a door
                player.transform.position = spawn;
            }

            if (player.GetComponent<PlayerMovement>() != null)
            {
                PlayerMovement movementScript = player.GetComponent<PlayerMovement>();
                movementScript.spawn = spawn;
            }

            if (player.GetComponent<ElevatorCommand>() == null)
            {
                ElevatorCommand eComm = player.AddComponent<ElevatorCommand>() as ElevatorCommand;
            }
        }

    }

    bool _tagChecker(string tag)
    {
        int numTags = tagList.Count;
        for (int i = 0; i < numTags; i++)
        {
            var ptr = tagList[i];
            if (ptr == tag)
            {
                return true;
            }
        }
        return false;
    }


    void replaceDoor(GameObject door)
    {
        Vector3 size = door.GetComponent<Renderer>().bounds.size;
        bool flag = false;
        if (size.x > size.z)
            flag = true;

        GameObject door1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door1.transform.localScale = size;
        door1.name = door.name + "_1";
        BoxCollider box1 = door1.GetComponent<BoxCollider>();
        DestroyImmediate(box1);

        door1.tag = "Door";

        if (flag)
            door1.transform.position = new Vector3(door.transform.position.x, door.transform.position.y + size.y / 2, door.transform.position.z);
        else
            door1.transform.position = new Vector3(door.transform.position.x, door.transform.position.y + size.y / 2, door.transform.position.z);

        door.SetActive(false);
    }

    void replaceDoubleDoor(GameObject doubleDoor)
    {
        Vector3 size = doubleDoor.GetComponent<Renderer>().bounds.size;
        bool flag = true;
        if (size.x > size.z)
            size.x /= 2;
        else
        {
            flag = false;
            size.z /= 2;
        }

        GameObject door1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door1.transform.localScale = size;
        door1.name = doubleDoor.name + "_1";
        BoxCollider box1 = door1.GetComponent<BoxCollider>();
        DestroyImmediate(box1);


        GameObject door2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door2.transform.localScale = size;
        door2.name = doubleDoor.name + "_2";
        BoxCollider box2 = door2.GetComponent<BoxCollider>();
        DestroyImmediate(box2);

        door1.tag = door2.tag = "Door";

        if (flag)
        {
            door1.transform.position = new Vector3(doubleDoor.transform.position.x + size.x / 2, doubleDoor.transform.position.y + size.y / 2, doubleDoor.transform.position.z);
            door2.transform.position = new Vector3(doubleDoor.transform.position.x - size.x / 2, doubleDoor.transform.position.y + size.y / 2, doubleDoor.transform.position.z);
        }
        else
        {
            door1.transform.position = new Vector3(doubleDoor.transform.position.x, doubleDoor.transform.position.y + size.y / 2, doubleDoor.transform.position.z + size.z / 2);
            door2.transform.position = new Vector3(doubleDoor.transform.position.x, doubleDoor.transform.position.y + size.y / 2, doubleDoor.transform.position.z - size.z / 2);
        }
        doubleDoor.SetActive(false);
    }

    void replaceElevatorDoor(GameObject elevatorDoor)
    {
        Vector3 size = elevatorDoor.GetComponent<Renderer>().bounds.size;
        bool flag = true;
        if (size.x > size.z)
            size.x /= 2;
        else
        {
            flag = false;
            size.z /= 2;
        }

        GameObject door1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door1.transform.localScale = size;
        door1.name = elevatorDoor.name + "_1";
        BoxCollider box1 = door1.GetComponent<BoxCollider>();
        DestroyImmediate(box1);


        GameObject door2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door2.transform.localScale = size;
        door2.name = elevatorDoor.name + "_2";
        BoxCollider box2 = door2.GetComponent<BoxCollider>();
        DestroyImmediate(box2);

        door1.tag = door2.tag = "Elevator";

        if (flag)
        {
            door1.transform.position = new Vector3(elevatorDoor.transform.position.x + size.x / 2, elevatorDoor.transform.position.y + size.y / 2, elevatorDoor.transform.position.z);
            door2.transform.position = new Vector3(elevatorDoor.transform.position.x - size.x / 2, elevatorDoor.transform.position.y + size.y / 2, elevatorDoor.transform.position.z);
        }
        else
        {
            door1.transform.position = new Vector3(elevatorDoor.transform.position.x, elevatorDoor.transform.position.y + size.y / 2, elevatorDoor.transform.position.z + size.z / 2);
            door2.transform.position = new Vector3(elevatorDoor.transform.position.x, elevatorDoor.transform.position.y + size.y / 2, elevatorDoor.transform.position.z - size.z / 2);
        }
        elevatorDoor.SetActive(false);
    }
}
