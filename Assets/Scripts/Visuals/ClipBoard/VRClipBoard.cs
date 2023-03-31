/******************************************************************************
 * File        : VRClipBoard.cs
 * Version     : 0.5 Alpha
 * Author      : Jere Jalonen (jere.jalonen@lapinamk.com)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2023 Lapland University of Applied Sciences
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using FrostBit;
using TMPro;
using System.Reflection;

public class VRClipBoard : MonoBehaviour
{


    public Transform VRHead;

    public float rotSpeed;

    public float boilerPower;

    public TextMeshPro temptext;

    public Transform DotPos;
    public GenericHeater boiler;
    public GenericDevice exit;
    float tempIn;
    public GenericDevice enter;
    float tempOut;

    public Sprite checkOn;
    public Sprite checkOff;

    public SpriteRenderer check1Sprite;
    public SpriteRenderer check2Sprite;
    public SpriteRenderer check3Sprite;

    public bool check1;
    public bool check2;
    public bool check3;

    public CustomPassVolume CRP;

    public List<CustomPass> passes;

    public Quaternion rotOffset;

    public Vector3 posOffset;


    private void Awake()
    {

        passes = CRP.customPasses;


        StartCoroutine("UpdateGizmo");

    }


    IEnumerator UpdateGizmo()
    {
        while (true)
        {
            SlotIn[] slotsIn = enter.GetSlotsIn();
            foreach (SlotIn a in slotsIn)
            {
                var objectType = enter.DeviceData.GetType();
                PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
                foreach (PropertyInfo prop in assa)
                {
                    if (prop.Name == "Temperature")
                        tempIn = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
                }
            }

            SlotIn[] slotsIn2 = exit.GetSlotsIn();
            foreach (SlotIn a in slotsIn2)
            {
                var objectType = exit.DeviceData.GetType();
                PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
                foreach (PropertyInfo prop in assa)
                {
                    if (prop.Name == "Temperature")
                        tempOut = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
                }
            }

            temptext.text = tempIn.ToString("0.00" + "°C") + "\n" + "\n" + tempOut.ToString("0.00" + "°C");

            yield return new WaitForSeconds(0.05f);
        }
    }



    void FixedUpdate()
    {

        Quaternion targetRotation = Quaternion.LookRotation(VRHead.transform.position - (transform.position + posOffset));

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * rotOffset, rotSpeed * Time.deltaTime);

    }
    private void Update()   //debug
    {
        if (Input.GetKeyDown("1"))
            toggle(1);
        else if (Input.GetKeyDown("2"))
            toggle(2);
        else if (Input.GetKeyDown("3"))
            toggle(3);
    }


    public void UpdateBoiler(Vector3 pos)
    {

        DotPos.localPosition = new Vector3((pos.x-0.5f)*1.25f, 2.3283e-10f, -0.0022497f);

        boiler.Power = ((pos.x-1)*-1)*2000;

        //0.6164, -0.6322
    }


    public void toggle(int toToggle)
    {
        SpriteRenderer toCheck;

        if (toToggle == 1)
        {
            toCheck = check1Sprite;
            check1 = !check1;
            if (check1)
                check1Sprite.sprite = checkOn;
            else check1Sprite.sprite = checkOff;


        }
        else if (toToggle == 2)
        {
            toCheck = check2Sprite;
            check2 = !check2;
            if (check2)
                check2Sprite.sprite = checkOn;
            else check2Sprite.sprite = checkOff;
        }
        else if (toToggle == 3)
        {
            toCheck = check3Sprite;
            check3 = !check3;
            if (check3)
                check3Sprite.sprite = checkOn;
            else check3Sprite.sprite = checkOff;
        }

        foreach (CustomPass pass in passes)
        {
            if (pass.name == "Arrows")
                pass.enabled = check1;
            if (pass.name == "PrimaryUnderlay")
                pass.enabled = check2;
            if (pass.name == "SecondaryUnderlay")
                pass.enabled = check3;

        }

    }


}

