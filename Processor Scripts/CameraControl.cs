using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //can be modified in unity for comfort
    public float mouseSensitivity = 100f;

    //public Transform player;

    float xRotate = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //grab position of mouse coordinates
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //rotate y axis as well as limit how far the model can turn
        xRotate -= mouseY;
        xRotate = Mathf.Clamp(xRotate, -90f, 90f);

        //rotate x axis 
        transform.localRotation = Quaternion.Euler(xRotate, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);


        transform.LookAt(transform.parent);
    }
}
