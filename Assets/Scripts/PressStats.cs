using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressStats : MonoBehaviour {

    float t, r;
    string s;
    
	// Use this for initialization
	void Start () {
        t = Time.time;
        r = Time.time;
        s = "";
        //Debug.Log("start odata");
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("everytime");
        if (GameObject.Find("Releasing22"))
            GameObject.Find("Releasing22").GetComponent<TextMesh>().text = "NOT CANCELED";
        if (Input.touchCount > 0)
        {
            t = Input.touchCount;
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                s = "just begun";
                r = Input.GetTouch(0).position.y;
            } else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (Input.GetTouch(0).position.y < r)
                {
                    s = "you moved your finger down!";
                } else
                {
                    s = "you moved your finger almost up!";
                }
                r = Input.GetTouch(0).position.y;
            } else if (Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                r = 9;
            } else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                GameObject.Find("Releasing22").GetComponent<TextMesh>().text = "ENDED";
            } else if (Input.GetTouch(0).phase == TouchPhase.Canceled)
            {
                GameObject.Find("Releasing22").GetComponent<TextMesh>().text = "CANCELED";
            }
        }
        if (GameObject.Find("NotYetPressed"))
            GameObject.Find("NotYetPressed").GetComponent<TextMesh>().text = t.ToString();
        if (GameObject.Find("Holding"))
            GameObject.Find("Holding").GetComponent<TextMesh>().text = r.ToString();
        if (GameObject.Find("Pressing"))
            GameObject.Find("Pressing").GetComponent<TextMesh>().text = s;
    }
}
