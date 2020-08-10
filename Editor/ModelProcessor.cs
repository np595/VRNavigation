using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SpatialTracking;
using UnityEditor.Animations;
using UnityEditor;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System;
using System.Runtime.Versioning;

public class ModelProcessor : EditorWindow
{
    public GameObject model = null;
    List<string> tagList = new List<string>{
        "Door",
        "Double Door",
        "Wall",
        "Floor",
        "Roof",
        "Elevator",
        "Stair",
        "Window",
        "Water",
        "Sand",
        "Light"
    };
    string doorString = "";
    string doubleDoorString = "";
    string wallString = "";
    string floorString = "";
    string roofString = "";
    string elevatorString = "";
    string stairString = "";
    string windowString = "";
    string waterString = "";
    string sandString = "";
    string lightString = "";
    GameObject player;
    GameObject VRplayer;
    bool doubleDoorTgl = false;
    bool csvTgl = true;
    bool positionTgl = false;
    bool recordTgl = false;
    bool fallThroughTgl = false;
    string frameString = "";
    int frames = 0;
    bool vrOption = false;
    bool lightOption = false;
    Vector2 scrollPos;
    Dictionary<string, string> tagStrings = new Dictionary<string, string>();

    // Add menu named "Model Processor" to the Window menu
    [MenuItem("Tools/Model Processor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ModelProcessor window = (ModelProcessor)EditorWindow.GetWindow(typeof(ModelProcessor));
        window.Show();
    }

    void OnGUI()
    {
        // Enable Scroll Bar
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Create input fields
        GUILayout.Label("Model", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        model = (GameObject)EditorGUILayout.ObjectField("Model", model, typeof(GameObject), true);
        EditorGUILayout.Space();
        EditorGUI.indentLevel--;

        GUILayout.Label("Tag Search (Use RegEx)", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        doorString = EditorGUILayout.TextField("Door", doorString == string.Empty ? "" : doorString);
        doubleDoorTgl = EditorGUILayout.Toggle("Replace Double Doors", doubleDoorTgl);
        if (doubleDoorTgl)
            doubleDoorString = EditorGUILayout.TextField("Double Door", doubleDoorString == string.Empty ? "" : doubleDoorString);
        else
            doubleDoorString = "";
        wallString = EditorGUILayout.TextField("Wall", wallString == string.Empty ? "" : wallString);
        floorString = EditorGUILayout.TextField("Floor", floorString == string.Empty ? "" : floorString);
        roofString = EditorGUILayout.TextField("Roof", roofString == string.Empty ? "" : roofString);
        elevatorString = EditorGUILayout.TextField("Elevator", elevatorString == string.Empty ? "" : elevatorString);
        stairString = EditorGUILayout.TextField("Stair", stairString == string.Empty ? "" : stairString);
        windowString = EditorGUILayout.TextField("Window", windowString == string.Empty ? "" : windowString);
        waterString = EditorGUILayout.TextField("Water", waterString == string.Empty ? "" : waterString);
        sandString = EditorGUILayout.TextField("Sand", sandString == string.Empty ? "" : sandString);
        lightString = EditorGUILayout.TextField("Light", lightString == string.Empty ? "" : lightString);
        fallThroughTgl = EditorGUILayout.Toggle("Enable Fall Through", fallThroughTgl);
        EditorGUILayout.Space();
        EditorGUI.indentLevel--;

        // CSV Options
        csvTgl = EditorGUILayout.Toggle("Display CSV Options", csvTgl);
        if (csvTgl)
        {
            EditorGUI.indentLevel++;
            GUILayout.Label("CSV", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            // Create Template File
            if (GUILayout.Button("Create Template CSV file"))
            {
                string path = EditorUtility.SaveFilePanel("Save textures to folder", "", "tags.csv", "csv");
                if (path.Length != 0)
                {
                    StreamWriter writer = new StreamWriter(path);
                    writer.WriteLine("Door,");
                    writer.WriteLine("Double Door,");
                    writer.WriteLine("Wall,");
                    writer.WriteLine("Floor,");
                    writer.WriteLine("Roof,");
                    writer.WriteLine("Elevator,");
                    writer.WriteLine("Stair,");
                    writer.WriteLine("Window,");
                    writer.WriteLine("Water,");
                    writer.WriteLine("Sand,");
                    writer.WriteLine("Light,");
                    writer.WriteLine("Fall Through,");
                    writer.Flush();
                    writer.Close();
                }
                RefreshEditorProjectWindow();
            }

            // Import CSV Data
            if (GUILayout.Button("Import from CSV file"))
            {
                string path = EditorUtility.OpenFilePanel("Choose csv", "", "csv");
                if (path.Length != 0 && path.Contains(".csv"))
                {
                    StreamReader reader = new StreamReader(path);
                    string fileData = reader.ReadToEnd();
                    string[] lines = fileData.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] parts = lines[i].Split(',');
                        switch (parts[0])
                        {
                            case "Door":
                                doorString = parts[1].Trim();
                                break;
                            case "Double Door":
                                doubleDoorTgl = true;
                                doubleDoorString = parts[1].Trim();
                                break;
                            case "Wall":
                                wallString = parts[1].Trim();
                                break;
                            case "Floor":
                                floorString = parts[1].Trim();
                                break;
                            case "Roof":
                                roofString = parts[1].Trim();
                                break;
                            case "Elevator":
                                elevatorString = parts[1].Trim();
                                break;
                            case "Stair":
                                stairString = parts[1].Trim();
                                break;
                            case "Window":
                                windowString = parts[1].Trim();
                                break;
                            case "Water":
                                waterString = parts[1].Trim();
                                break;
                            case "Sand":
                                sandString = parts[1].Trim();
                                break;
                            case "Light":
                                lightString = parts[1].Trim();
                                break;
                            case "Fall Through":
                                fallThroughTgl = Convert.ToBoolean(parts[1].Trim());
                                break;
                        }
                    }
                    reader.Close();
                }
                RefreshEditorProjectWindow();
            }

            // Export CSV Data
            if (GUILayout.Button("Export as tags.csv file"))
            {
                string path = EditorUtility.SaveFilePanel("Choose Folder", "", "tags.csv", "csv");
                if (path.Length != 0)
                {
                    StreamWriter writer = new StreamWriter(path);
                    writer.WriteLine("Door," + doorString.Trim());
                    if (doubleDoorTgl)
                    {
                        writer.WriteLine("Double Door," + doubleDoorString.Trim());
                    }
                    writer.WriteLine("Wall," + wallString.Trim());
                    writer.WriteLine("Floor," + floorString.Trim());
                    writer.WriteLine("Roof," + roofString.Trim());
                    writer.WriteLine("Elevator," + elevatorString.Trim());
                    writer.WriteLine("Stair," + stairString.Trim());
                    writer.WriteLine("Window," + windowString.Trim());
                    writer.WriteLine("Water," + waterString.Trim());
                    writer.WriteLine("Sand," + sandString.Trim());
                    writer.WriteLine("Light," + lightString.Trim());
                    writer.WriteLine("Fall Through," + fallThroughTgl.ToString());
                    writer.Flush();
                    writer.Close();
                }
                RefreshEditorProjectWindow();
            }
            GUILayout.Label("Close tag.csv before using any of the above functions", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        // Custom Start Positions
        positionTgl = EditorGUILayout.Toggle("Use Custom Start Position", positionTgl);
        if (positionTgl)
        {
            EditorGUI.indentLevel++;
            if (GUILayout.Button("Import from CSV file"))
            {
                string fileData = "";
                string path = EditorUtility.OpenFilePanel("Choose csv", "", "csv");
                if (path.Length != 0 && path.Contains(".csv"))
                {
                    StreamReader reader = new StreamReader(path);
                    fileData = reader.ReadToEnd();
                    reader.Close();
                }
                path = Application.dataPath + "/Editor/positions.csv";
                StreamWriter writer = new StreamWriter(path);
                writer.WriteLine(fileData);
                writer.Flush();
                writer.Close();
                RefreshEditorProjectWindow();
            }
            GUILayout.Label("Close position.csv before using the above function", EditorStyles.centeredGreyMiniLabel);
            EditorGUI.indentLevel--;
        }

        // Record Movement
        recordTgl = EditorGUILayout.Toggle("Record Movement", recordTgl);
        if (recordTgl)
        {
            EditorGUI.indentLevel++;
            frameString = EditorGUILayout.TextField("Record every X frames", frameString);
            EditorGUI.indentLevel--;
        }
        else
            frameString = "";

        // Additional Options
        vrOption = EditorGUILayout.Toggle("Use VR", vrOption);
        lightOption = EditorGUILayout.Toggle("Add Light to Player", lightOption);

        // Process the Model
        EditorGUILayout.Space(20);
        if (GUILayout.Button("Process Model"))
        {
            try
            {
                RefreshEditorProjectWindow();
                string path = Application.dataPath + "/Editor/positions";

                // If custom spawning is disabled, delete the positions.csv file
                if (!positionTgl && File.Exists(path + ".csv"))
                    File.Delete(path + ".csv");
                File.Delete(path + ".meta");

                tagStrings["Door"] = doorString;
                tagStrings["Double Door"] = doubleDoorString;
                tagStrings["Wall"] = wallString;
                tagStrings["Floor"] = floorString;
                tagStrings["Roof"] = roofString;
                tagStrings["Elevator"] = elevatorString;
                tagStrings["Stair"] = stairString;
                tagStrings["Window"] = windowString;
                tagStrings["Water"] = waterString;
                tagStrings["Sand"] = sandString;
                tagStrings["Light"] = lightString;

                if (!GameObject.FindWithTag("Player"))
                {
                    disableCameras();
                    createPlayer();
                    createVRPlayer();
                }

                // Creates all needed tags
                foreach (String tag in tagList)
                {
                    createTag(tag);
                }
                createTag("Elevator Floor");

                // Adds ObjectComponentProvider script to the model
                if (model.GetComponent(typeof(ObjectComponentProvider)) == null)
                {
                    ObjectComponentProvider OCP = model.AddComponent<ObjectComponentProvider>();
                    OCP.tagList = tagList;
                }

                if (model.transform.childCount > 0)
                    for (int i = 0; i < model.transform.childCount; i++)
                    {
                        assignTag(model.transform.GetChild(i).gameObject);
                        iterateChild(model.transform.GetChild(i).gameObject);
                    }

                prepareAnimations();
                RefreshEditorProjectWindow();
                EditorUtility.DisplayDialog("Model Processor Tool", "Processing Successful", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Model Processor Tool", "Processing Failed", "OK");
                UnityEngine.Debug.LogException(e, this);
                DestroyImmediate(player);
                DestroyImmediate(VRplayer);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    // Refreshes Project Editor Windows
    void RefreshEditorProjectWindow()
    {
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    // Disables all camera in the scene
    void disableCameras()
    {
        Camera[] cameras = Camera.allCameras;
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
        }
    }

    // Prepares animations for doors
    void prepareAnimations()
    {
        AnimatorController doorAnimator = Resources.Load("DoorLeft") as AnimatorController;
        doorAnimator.layers[0].stateMachine.states[1].state.motion = Resources.Load("OpenLeft") as Motion;
        doorAnimator.layers[0].stateMachine.states[2].state.motion = Resources.Load("CloseLeft") as Motion;
        doorAnimator = Resources.Load("DoorRight") as AnimatorController;
        doorAnimator.layers[0].stateMachine.states[1].state.motion = Resources.Load("OpenRight") as Motion;
        doorAnimator.layers[0].stateMachine.states[2].state.motion = Resources.Load("CloseRight") as Motion;
    }

    // Creates player object
    void createPlayer()
    {
        player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.localScale = new Vector3(.3f, .7f, .3f);

        CharacterController controller = player.AddComponent(typeof(CharacterController)) as CharacterController;
        PlayerMovement movementScript = player.AddComponent(typeof(PlayerMovement)) as PlayerMovement;

        // Adds recording script if enabled
        if (recordTgl)
        {
            RecordMovement recordScript = player.AddComponent(typeof(RecordMovement)) as RecordMovement;
            bool flag = int.TryParse(frameString, out frames);
            if (!string.IsNullOrWhiteSpace(frameString) && flag)
                recordScript.frames = frames;
        }

        // Enables light if option is selected
        Light lightComp = new Light();
        if (player.GetComponent<Light>() == null)
            lightComp = player.AddComponent<Light>() as Light;
        if (lightOption)
            lightComp.enabled = true;
        else
            lightComp.enabled = false;
        lightComp.range = 30;

        createTag("Player");
        player.tag = "Player";


        // Creates Camera object
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent(typeof(Camera)) as Camera;
        CameraControl cameraScript = cameraObject.AddComponent(typeof(CameraControl)) as CameraControl;
        cameraObject.transform.parent = player.transform;
        createTag("MainCamera");
        cameraObject.tag = "MainCamera";

        // Creates Pivot object
         GameObject pivotObject = new GameObject("Pivot");
         pivotObject.transform.parent = player.transform;

        // If VR is selected, makes player object inactive
        if (vrOption)
            player.SetActive(false);
    }

    // Creates VR player object
    void createVRPlayer(){
        VRplayer = new GameObject("VrRig");
        createTag("Player");
        VRplayer.tag = "Player";

        // create XRrig, which is the VRplayer, and add all necessary components
        XRRig rig = VRplayer.AddComponent(typeof(XRRig)) as XRRig;
        Vector3 size = new Vector3(.3f,.7f,.3f);
        VRplayer.transform.localScale = size;
        CharacterController controller = VRplayer.AddComponent(typeof(CharacterController)) as CharacterController;
        LocomotionSystem loco = VRplayer.AddComponent(typeof(LocomotionSystem)) as LocomotionSystem;
        TeleportationProvider tele = VRplayer.AddComponent(typeof(TeleportationProvider)) as TeleportationProvider;
        PlayerMovement movement = VRplayer.AddComponent(typeof(PlayerMovement)) as PlayerMovement;
        movement.inputSource = UnityEngine.XR.XRNode.LeftHand;
        rig.cameraYOffset = 1f;

        // create cameraoffset, child of VRrig
        GameObject cameraOffset = new GameObject("CameraOffset");
        cameraOffset.tag = "MainCamera";
        cameraOffset.transform.parent = VRplayer.transform;
        
        // create all children of camera offset
        GameObject leftHand = new GameObject("leftHand");
        GameObject rightHand = new GameObject("rightHand");
        GameObject rightRay = new GameObject("Right Teleport Ray");
        GameObject camera = new GameObject("Camera");

        // add necessary components to the camera, rightHand, leftHand, and rightRay objects

        Camera cam = camera.AddComponent(typeof(Camera)) as Camera;
        XRController leftController = leftHand.AddComponent(typeof(XRController)) as XRController;
        XRController rightController = rightHand.AddComponent(typeof(XRController)) as XRController;
        TrackedPoseDriver track = camera.AddComponent(typeof(TrackedPoseDriver)) as TrackedPoseDriver;
        CameraFix camF = camera.AddComponent(typeof(CameraFix)) as CameraFix;
        camF.playa = rig.transform;
        XRController cont = rightRay.AddComponent(typeof(XRController)) as XRController;
        XRRayInteractor ray = rightRay.AddComponent(typeof(XRRayInteractor)) as XRRayInteractor;
        LineRenderer line = rightRay.AddComponent(typeof(LineRenderer)) as LineRenderer;
        XRInteractorLineVisual vis = rightRay.AddComponent(typeof(XRInteractorLineVisual)) as XRInteractorLineVisual;

        // make cameraoffset the parent of the aforementioned objects
        leftHand.transform.parent = cameraOffset.transform;
        rightHand.transform.parent = cameraOffset.transform;
        camera.transform.parent = cameraOffset.transform;
        rightRay.transform.parent = cameraOffset.transform;

        // set component details for controller and teleport ray
        rig.cameraGameObject = camera;
        rig.cameraFloorOffsetObject = cameraOffset;
        leftController.controllerNode = UnityEngine.XR.XRNode.LeftHand;
        rightController.controllerNode = UnityEngine.XR.XRNode.RightHand;
        
        rightRay.GetComponent<XRController>().controllerNode = UnityEngine.XR.XRNode.RightHand;
        // to fix teleport camera, try looking into locking the rotation when you teleport

        // Adds recording script if enabled
        if (recordTgl)
        {
            RecordMovement recordScript = VRplayer.AddComponent(typeof(RecordMovement)) as RecordMovement;
            bool flag = int.TryParse(frameString, out frames);
            if (!string.IsNullOrWhiteSpace(frameString) && flag)
                recordScript.frames = frames;
        }

        // Enables light if option is selected
        Light lightComp = new Light();
        if (VRplayer.GetComponent<Light>() == null)
            lightComp = VRplayer.AddComponent<Light>() as Light;
        if (lightOption)
            lightComp.enabled = true;
        else
            lightComp.enabled = false;
        lightComp.range = 30;

        // If VR is not selected, makes VR VRplayer object inactive
        if (!vrOption)
            VRplayer.SetActive(false);

    }

    // Assigns Tags to each object in the model
    void assignTag(GameObject child)
    {
        string objectName = child.name;

        // Checks the object name against all possible regex
        foreach (KeyValuePair<string, string> entry in tagStrings)
        {
            if (string.IsNullOrEmpty(entry.Value))
                continue;

            if (Regex.Match(objectName, entry.Value).Success)
            {
                child.tag = entry.Key;

                if (!fallThroughTgl)
                    break;
            }
        }
    }

    // Iterates through all of an objects children and assigns tags
    void iterateChild(GameObject child)
    {
        if (child.transform.childCount > 0)
        {
            for (int i = 0; i < child.transform.childCount; i++)
            {
                assignTag(child.transform.GetChild(i).gameObject);
                iterateChild(child.transform.GetChild(i).gameObject);
            }
        }
    }

    // Creates tag
    void createTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));
        SerializedProperty tags = tagManager.FindProperty("tags");
        int numTags = tags.arraySize;
        for (int i = 0; i < numTags; i++)
        {
            var ptr = tags.GetArrayElementAtIndex(i);
            if (ptr.stringValue == tag)
            {
                return;
            }
        }
        tags.InsertArrayElementAtIndex(numTags);
        tags.GetArrayElementAtIndex(numTags).stringValue = tag;
        tagManager.ApplyModifiedProperties();
        tagManager.Update();
    }
}
