using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour {

    public bool collidedWithPlanet, collidedWithHalfsphere;
    public GameObject colliderObj;  // NECESAR pentru a nu pune la coliziuni chiar checker-ul aferent planetei!! colliderObj e de fapt argumentul other.

    private void Start()
    {
        collidedWithPlanet = false;
        collidedWithHalfsphere = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.position == transform.position)
            return;
        
        if (other.gameObject.name[0] == 'P') // planet 
        {
            collidedWithPlanet = true;
            collidedWithHalfsphere = false;
        }
        else if (other.gameObject.name[0] == 'H') // halfsphere
        {
            collidedWithHalfsphere = true;
            collidedWithPlanet = false;
        }
        else return;

        
        
        colliderObj = other.gameObject.name[1] != 'u' ? other.gameObject : null;
    }
}
