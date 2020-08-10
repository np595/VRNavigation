using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Animator _animator;
    public bool triggers = false;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (triggers == true && _animator.GetBool("isOpen") == false)
        {
            _animator.SetBool("isOpen", true);
        }

        else if (triggers == false && _animator.GetBool("isOpen") == true)
        {
            _animator.SetBool("isOpen", false);
        }
    }
}
