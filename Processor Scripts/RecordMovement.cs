using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordMovement : MonoBehaviour
{
    int fileNum = 0;
    string filePath = "";
    int frame = 0;
    public int frames = 6;

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/Movement Data/movement";
        System.IO.Directory.CreateDirectory(Application.dataPath + "/Movement Data");
        string extension = ".csv";
        while (true)
        {
            if (File.Exists(path + fileNum + extension))
            {
                fileNum += 1;
            }
            else
            {
                filePath = path + fileNum + extension;
                break;
            }
        }

        StreamWriter writer = new StreamWriter(filePath);
        writer.Flush();
        writer.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (frame == frames)
        {
            if (File.Exists(filePath))
            {
                float x = transform.position.x;
                float y = transform.position.y;
                float z = transform.position.z;

                File.AppendAllText(filePath, x + "," + y + "," + z + "," + Environment.NewLine);
            }
            frame = 0;
        }
        else
        {
            frame += 1;
        }
    }
}
