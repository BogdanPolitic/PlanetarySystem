using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<MeshRenderer>();
        Texture tex = Resources.Load("/new_game.png") as Texture;
        renderer.material.SetTexture("_EmissionMap", tex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
