using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAnimator : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        //if ()
        Debug.Log("anim state = " + anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
