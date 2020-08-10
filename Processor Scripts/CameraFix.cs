using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SpatialTracking;

public class CameraFix : MonoBehaviour
{
    public Transform playa;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var xRot = playa.transform.rotation.eulerAngles;
        xRot.x = 0f;
        Vector3 playerPos = new Vector3(playa.transform.position.x, playa.transform.position.y , playa.transform.position.z);
        transform.position = playerPos;
        playa.rotation = Quaternion.Euler(xRot);
    }
}
