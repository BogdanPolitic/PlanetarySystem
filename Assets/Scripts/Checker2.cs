using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker2 : MonoBehaviour {

    public float idx; // which is the WEIGHT of the object!!

    private void OnTriggerEnter(Collider other)
    {
        idx = other.gameObject.GetComponent<Rigidbody>().mass;
    }
}
