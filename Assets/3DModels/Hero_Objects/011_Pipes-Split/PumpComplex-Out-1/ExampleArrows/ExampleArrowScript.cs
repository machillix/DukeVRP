using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleArrowScript : MonoBehaviour
{

    public MeshRenderer mesh;
    public Material mat;


    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mat = GetComponent<Renderer>().material;

    }

    void Update()
    {

    }

    void Enable()
    {
        mesh.enabled = true;
    }

    void Disable()
    {
        mesh.enabled = false;
    }


    public void ChangeColor(float colour)
    {
        mat.SetFloat("_Difference", colour);
    }


}
