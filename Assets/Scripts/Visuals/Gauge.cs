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
using UnityEngine.EventSystems;
using System.Reflection;

public class Gauge : MonoBehaviour
{
    Transform pointer;

    public GenericDevice device;

    public float mypressure;

    public bool TempNotBar = false;

    void Start()
    { 
        foreach (Transform child in transform)
            if (child.transform.name.Contains("Pointer"))
                pointer = child;
    }


    void Update()
    {
        UpdatePointer(mypressure);
    }

    //public float wantpressure;

    public float Exponential = 1;

    void UpdatePointer(float pressure)
    {


        SlotIn[] slotsIn = device.GetSlotsIn();

        foreach (SlotIn a in slotsIn)
        {
            var objectType = device.DeviceData.GetType();

            PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
            foreach (PropertyInfo prop in assa)
            {
                //print(prop.Name + " - " + prop.GetValue(a.LastDeviceData, null).ToString());
                //if (prop.Name == "Q")
                //pressure = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());

                if (TempNotBar) {
                    if (prop.Name == "Q")
                        pressure = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
                }
                else
                    if (prop.Name == "Temperature")
                    pressure = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
            }
        }
        float wantpressure;

        if (!TempNotBar)
        {
            pressure = pressure * 0.1f;
            wantpressure = (pressure * 33.75f);     //270 jaettu 8
        }
        else wantpressure = (pressure * Exponential);
        //else wantpressure = (pressure * 1.733f);

        if (pointer)
        pointer.localEulerAngles = new Vector3(wantpressure-135, 0, 0);
    }

}
