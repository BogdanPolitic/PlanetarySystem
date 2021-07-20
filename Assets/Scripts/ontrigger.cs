using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ontrigger : MonoBehaviour {

	void Start () {

	}
	
	void Update () {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //print("i'm collided with mass: " + other.name);
    }
}
