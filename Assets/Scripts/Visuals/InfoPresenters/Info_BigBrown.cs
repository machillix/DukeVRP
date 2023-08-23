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

public class Info_BigBrown : MonoBehaviour
{
    public GenericDevice deviceInfo;

    //Internal
    public TextMeshPro primaryText;
    float TempPrim1;
    float TempPrim2;
    public GenericDevice PrimIn;
    public GenericDevice PrimOut;

    //External
    public TextMeshPro secondaryText;
    float TempSec1;
    float TempSec2;
    public GenericDevice SecIn;
    public GenericDevice SecOut;


    void Update()
    {
        SlotIn[] slotsIn = PrimIn.GetSlotsIn();
        foreach (SlotIn a in slotsIn)
        {
            var objectType = deviceInfo.DeviceData.GetType();
            PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
            foreach (PropertyInfo prop in assa)
            {
                if (prop.Name == "Temperature")
                    TempPrim1 = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
            }
        }

        SlotIn[] slotsIn2 = PrimOut.GetSlotsIn();
        foreach (SlotIn a in slotsIn2)
        {
            var objectType = deviceInfo.DeviceData.GetType();
            PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
            foreach (PropertyInfo prop in assa)
            {
                if (prop.Name == "Temperature")
                    TempPrim2 = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
            }
        }

        primaryText.text = "Primary\n" + TempPrim1.ToString("0.00" + "°C") + "\n" + TempPrim2.ToString("0.00" + "°C");
      

        SlotIn[] slotsIn3 = SecIn.GetSlotsIn();
        foreach (SlotIn a in slotsIn3)
        {
            var objectType = deviceInfo.DeviceData.GetType();
            PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
            foreach (PropertyInfo prop in assa)
            {
                if (prop.Name == "Temperature")
                    TempSec1 = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
            }
        }

        SlotIn[] slotsIn4 = SecOut.GetSlotsIn();
        foreach (SlotIn a in slotsIn4)
        {
            var objectType = deviceInfo.DeviceData.GetType();
            PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
            foreach (PropertyInfo prop in assa)
            {
                if (prop.Name == "Temperature")
                    TempSec2 = float.Parse(prop.GetValue(a.LastDeviceData, null).ToString());
            }
        }

        secondaryText.text = "Secondary\n" + TempSec1.ToString("0.00" + "°C") + "\n" + TempSec2.ToString("0.00" + "°C");

    }
}
