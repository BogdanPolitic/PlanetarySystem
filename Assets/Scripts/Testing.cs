using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    GameObject obj1;
    GameObject obj2;
    public Vector3 velocity;
    public float dst;

    float AngleAdapted(GameObject g)
    {
        Vector3 v = g.GetComponent<Rigidbody>().velocity;
        float unsignedAngle = Vector3.Angle(v, Vector3.right);
        if (v.z >= 0)
        {
            return 180 - unsignedAngle;
        }
        else
        {
            return 180 + unsignedAngle;
        }
    }

    void RotationAlongVelocitySide1(GameObject g)
    {
        g.transform.rotation = Quaternion.Euler(g.transform.rotation.x, AngleAdapted(g), g.transform.rotation.z);
    }

    void RotationAlongVelocitySide2(GameObject g)
    {
        g.transform.rotation = Quaternion.Euler(g.transform.rotation.x, AngleAdapted(g) + 180, g.transform.rotation.z);
    }

    Vector3 hsCoords(Vector3 instPos, Vector3 velocity, bool upwards, bool firstObj)
    {
        float m = -velocity.x / velocity.z; // panta perpendicularei la traiectorie
        float root = Mathf.Sqrt(dst / (2 * (1 + Mathf.Pow(m, 2)))); // radicalul din ecuatia din care aflam x' (xc)
        float xc = (upwards && firstObj) || (!upwards && !firstObj) ? instPos.x - root : instPos.x + root;  // coord x la care se afla semisfera (prima sau ultima)
        float zc = instPos.z + m * (xc - instPos.x);    // coord z la care se afla semisfera (prima sau ultima)

        return new Vector3(xc, instPos.y, zc);
    }

	void Start () {
        if (gameObject.GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
	}
	
	void Update () {
        if (Input.GetKeyDown("t"))
        {
            obj1 = Instantiate(GameObject.Find("Cube3"));
            obj2 = Instantiate(GameObject.Find("Cube3"));

            obj1.transform.position = new Vector3(obj1.transform.position.x, 1, obj1.transform.position.z);
            obj1.GetComponent<Rigidbody>().velocity = velocity;

            obj2.transform.position = new Vector3(obj1.transform.position.x, 1, obj1.transform.position.z);
            obj2.GetComponent<Rigidbody>().velocity = velocity;

            RotationAlongVelocitySide1(obj1);
            RotationAlongVelocitySide2(obj2);
            
            obj1.transform.position = hsCoords(gameObject.transform.position, velocity, velocity.z > 0, true);
            obj2.transform.position = hsCoords(gameObject.transform.position, velocity, velocity.z > 0, false);
            gameObject.SetActive(false);
        }
	}
}
