using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovement : MonoBehaviour
{

    public float forwardForce = 3f;
    public CharacterController controller;
    public Vector3 spawn = new Vector3(0, 0, 0);

    private Vector3 moveDirection;
    public float gravity = 10f;

    public Vector2 inputAxis;

    private XRRig rig;
    public GameObject kbPlayer;
    public GameObject vrPlayer;

    public XRNode inputSource;

    // Start is called before the first frame update
    void Start()
    {
        kbPlayer = GameObject.Find("Player");
        vrPlayer = GameObject.Find("VrRig");
        rig = GetComponent<XRRig>();

        //assigns character to whichever player is enabled
        if (kbPlayer == null){
            controller = vrPlayer.GetComponent<CharacterController>();
            
        }     
        else{
            controller = kbPlayer.GetComponent<CharacterController>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //if the player is disabled, aka the use VR option was selected in the model processor, use VR controls
        if (kbPlayer == null){
            InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

            Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
            Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

            controller.Move(direction * Time.fixedDeltaTime * forwardForce);

            //gravity
            direction.y = direction.y + (Physics.gravity.y * gravity * Time.deltaTime);
            controller.Move(direction * Time.deltaTime * forwardForce);
        }
        //if the player is enabled, aka the use VR option was not selected in the model processor, use keyboard controls
        else{

            moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
            moveDirection.y = moveDirection.y + (Physics.gravity.y * gravity * Time.deltaTime);
            controller.Move(moveDirection * Time.deltaTime * forwardForce);

        }
      
        if(transform.position.y < 0)
        {
            transform.position = spawn;
        }
    }
}