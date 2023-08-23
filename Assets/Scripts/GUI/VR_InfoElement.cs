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
using FrostBit;
using System.Reflection;
using TMPro;
using DUKE.Controls;

public class VR_InfoElement : MonoBehaviour
{

    //public GenericDevice curdevice;

    public string iText;

    public VRInput LHand;
    public VRInput RHand;


    float Temp;

    //yeeeasd

    public Transform VRHead;
    public Transform DesktopHead;

    GameObject Visuals;
    public TextMeshPro InfoText;

    private void Start()
    {
        VRHead = GameObject.Find("/Func/Player/Player (VR)/Head").transform;
        DesktopHead = GameObject.Find("/Func/Player/Player (Desktop)/Desktop Camera").transform;
        //InfoText = GameObject.Find("/InfoBall/Visuals/InfoLine/Root/End/Length/InfoText").GetComponent<TextMeshPro>();


        //Find all text objects
        foreach (TextMeshPro TMP in FindObjectsOfType<TextMeshPro>())
            if (TMP.transform.root == this.transform)
                InfoText = TMP;

        
    //Find hidable visuals of the object
        foreach (Transform child in transform)
            if (child.name == "Visuals")
                Visuals = child.gameObject;

        Visuals.SetActive(false);
    }


    public void enableGizmo(GenericDevice deviceInfo)
    {
        Visuals.SetActive(true);
        StartCoroutine("UpdateGizmo", deviceInfo);
    }

    public void disableGizmo()
    {
        Visuals.SetActive(false);
        StopCoroutine("UpdateGizmo");
    }



    IEnumerator UpdateGizmo(GenericDevice deviceInfo)
    {
        while (true)
        {
            SlotIn[] slotsIn = deviceInfo.GetSlotsIn();
            foreach (SlotIn a in slotsIn)
            {
                var objectType = deviceInfo.DeviceData.GetType();
                PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
                foreach (PropertyInfo prop in assa)
                {
                    if (prop.Name == "Temperature")
                        Temp = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
                }
            }
            UpdatePosition(deviceInfo.transform);

            yield return new WaitForSeconds(0.025f);
        }  
    }



    Transform Lookat;

    float dl;
    float dr;

    void UpdatePosition(Transform curdevice)
    {
        //Check if the player is on VR or not
        if (VRHead.gameObject.activeSelf)
            Lookat = VRHead;
        else Lookat = DesktopHead;
        //Turn the object to face the camera
        transform.LookAt(Lookat);

        //Change the text on the textmeshpro component
        InfoText.text = Temp.ToString("0.00"+ "°C");

        //Check if controllers hit device, then compare distance to device between left and right controllers, closest is where we draw the gizmo
        if (LHand.Hit.transform == curdevice)
            dl = Vector3.Distance(LHand.RayPointPosition, curdevice.position);
        else dl = 100;
        if (RHand.Hit.transform == curdevice)
            dr = Vector3.Distance(RHand.RayPointPosition, curdevice.position);
        else dr = 100;


        if (dl <= dr)
            transform.position = LHand.RayPointPosition;
        else transform.position = RHand.RayPointPosition;
    }

}
