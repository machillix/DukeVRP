using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Lamp : MonoBehaviour
{
    [SerializeField] Light lightObject;
    [SerializeField] float lightIntensity = 10f;
    [SerializeField] float materialIntensity = 5f;
    [SerializeField, Range(0f, 1f)] float colorRatio = 1f;
    [SerializeField] Gradient lightColorGradient;

    MeshRenderer mRend;




    void Start ()
    {
        if (null == mRend)
        {
            mRend = GetComponent<MeshRenderer>();
        }

        SetLampIntensity();
        ChangeLampColor(colorRatio);
    }

    void OnValidate ()
    {
        if (!Application.isPlaying)
        {
            if (null == mRend)
            {
                mRend = GetComponent<MeshRenderer>();
            }

            SetLampIntensity();
            ChangeLampColor(colorRatio);
        }     
    }



    public void ToggleLight (bool _on)
    {
        lightObject.intensity = _on ? lightIntensity : 0f;
        mRend.sharedMaterials[0].SetFloat("_Intensity", _on ? materialIntensity : 0.5f);      
    }

    protected void SetLampIntensity ()
    {
        lightObject.intensity = lightIntensity;
        mRend.sharedMaterials[0].SetFloat("_Intensity", materialIntensity);
    }

    public void ChangeLampColor (float _ratio)
    {
        Color col = lightColorGradient.Evaluate(_ratio);        
        colorRatio = _ratio;
        lightObject.color = col;

        mRend.sharedMaterials[0].SetColor("_Color", col);     
    }


}