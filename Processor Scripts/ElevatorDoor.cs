using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    public bool triggers = false;
    Vector3 moveLeftx;
    Vector3 moveRightx;
    Vector3 moveLeftz;
    Vector3 moveRightz;
    Vector3 origin;

    //public OpenDoor oDoor;
    // Start is called before the first frame update
    void Start()
    {
        moveLeftx = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        moveRightx = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        moveLeftz = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        moveRightz = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        origin = transform.position;
    }

    void Update()
    {
        //oDoor = GetComponentInChildren<OpenDoor>();
        Vector3 objSize = gameObject.GetComponent<Renderer>().bounds.size;
        if (triggers == true)
        {//Move door left
                    
                    if(objSize.x >= objSize.z)
                    {
                        if (Regex.IsMatch(gameObject.name, "_1", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _1 in it then it puts doorleft
                        {
                            transform.position = Vector3.MoveTowards(transform.position, moveRightx, 0.1f);
                        }
                        if (Regex.IsMatch(gameObject.name, "_2", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _2 in it then it puts doorright
                        {
                            transform.position = Vector3.MoveTowards(transform.position, moveLeftx, 0.1f);
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(gameObject.name, "_1", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _1 in it then it puts doorleft
                        {
                            transform.position = Vector3.MoveTowards(transform.position, moveRightz, 0.1f);
                        }
                        if (Regex.IsMatch(gameObject.name, "_2", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _2 in it then it puts doorright
                        {
                            transform.position = Vector3.MoveTowards(transform.position, moveLeftz, 0.1f);
                        }
                    }
        }

        else if (triggers == false)
        {//Move close door left
            transform.position = Vector3.MoveTowards(transform.position, origin, 0.1f);
            /*if (objSize.x >= objSize.z)
            {
                if (Regex.IsMatch(gameObject.name, "_1", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _1 in it then it puts doorleft
                {
                    transform.position = Vector3.MoveTowards(transform.position, origin, 0.1f);
                }
                if (Regex.IsMatch(gameObject.name, "_2", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _2 in it then closes it
                {
                    transform.position = Vector3.MoveTowards(transform.position, origin, 0.1f);
                }
            }
            else
            {
                if (Regex.IsMatch(gameObject.name, "_1", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _1 in it then it puts doorleft
                {
                    transform.position = Vector3.MoveTowards(transform.position, origin, 0.1f);
                }
                if (Regex.IsMatch(gameObject.name, "_2", RegexOptions.IgnoreCase)) //Reads the end of the name and if it has _1 in it then it puts doorleft
                {
                    transform.position = Vector3.MoveTowards(transform.position, origin, 0.1f);
                }
            }*/
        }
    }
}
