using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class ElevatorCommand : MonoBehaviour //When trying to do this as a child of the elevator then back, you need to get the last location the player was
{                                            //in relation to the model, else the player gets sent flying either to 0,0,0 or into space.
    
    public bool moveUp = false;
    public bool moveDown = false;
   
    
    
    // Update is called once per frame
    private InputDevice targetDevice1;
    
    void Start(){

        List<InputDevice> devices1 = new List<InputDevice>();
        InputDeviceCharacteristics rightC = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightC, devices1);

        
        if (devices1.Count > 0){
            targetDevice1 = devices1[0];
        }
        
    }

    void Update()
    {
        
        targetDevice1.TryGetFeatureValue(CommonUsages.secondaryButton, out bool aButton);
        targetDevice1.TryGetFeatureValue(CommonUsages.primaryButton, out bool bButton);
        
        


        if (Input.GetKeyDown(KeyCode.U) | aButton) //Moves elevator up while Q key is pressed
        {
            moveDown = false;
            if (moveUp == true)
            {
                Debug.Log("Move Up Deactivated");
            }
            if (moveUp == false)
            {
                Debug.Log("Move Up Activated");
            }
            moveUp = !moveUp;
        }
        else if (Input.GetKeyDown(KeyCode.J) | bButton) //Moves elevator down while J key is pressed
        {
            moveUp = false;
            if (moveDown == true) //Needs to hit the U or J button once to move up or down, and once again to stop the motion
            {
                Debug.Log("Move Down Deactivated");
            }
            if (moveDown == false)
            {
                Debug.Log("Move Down Activated");
            }
            moveDown = !moveDown;
        }
    }
}
