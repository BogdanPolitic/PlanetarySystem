using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeOnPause : MonoBehaviour
{
    Vector3 originalVelocity = Vector3.zero;
    Vector3 originalAngularVelocity = Vector3.zero;

    Vector3 velocity = Vector3.zero;
    Vector3 angularVelocity = Vector3.zero;

    public bool zeroVelocity = false;

    //static GameObject thisGameObject = null;

    void Start()
    {
        //thisGameObject = gameObject;
    }

    
    void Update()
    {
        velocity = gameObject.GetComponent<Rigidbody>().velocity;
        angularVelocity = gameObject.GetComponent<Rigidbody>().angularVelocity;

        if (!zeroVelocity)
            originalVelocity = velocity;
        if (!zeroVelocity)
            originalAngularVelocity = angularVelocity;
    }

    public void Freeze()
    {
        if (!zeroVelocity)
        {
            originalVelocity = velocity;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            zeroVelocity = true;
        }
    }

    public void Unfreeze()
    {
        if (zeroVelocity)
        {
            gameObject.GetComponent<Rigidbody>().velocity = originalVelocity;
            gameObject.GetComponent<Rigidbody>().angularVelocity = originalAngularVelocity;
            zeroVelocity = false;
        }
    }
}
