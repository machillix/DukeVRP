/******************************************************************************
 * File        : GenericValve.cs
 * Version     : 0.9 Alpha
 * Author      : Toni Westerlund (toni.westerlund@lapinamk.com)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2020 Lapland University of Applied Sciences
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
using UnityEngine.Events;

namespace FrostBit
{
    /// <summary>
    /// Valve (Device)
    /// </summary>
    public class GenericValve : GenericDevice
    {
        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// Valve position, 0 = closed, 1 = Full open
        /// </summary>
        [SerializeField] private float valvePos = 0;

        /// <summary>
        /// ValvePos
        /// </summary>
        public float ValvePos
        {
            set
            {
                this.valvePos = value;
                if (valvePos > 100f)
                {
                    valvePos = 100f;
                }
                else if (valvePos < 0f)
                {
                    valvePos = 0f;
                }
            }
            get
            {
                return this.valvePos;
            }
        }


        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /// <summary>
        /// GetDeviceData
        /// </summary>
        /// <returns></returns>
        public override GenericDeviceData GetDeviceData()
        {

            GenericValveData newData = (GenericValveData)GenericValveData.CreateInstance("GenericValveData");
            GenericValveData data = (GenericValveData)deviceData;
            newData.MaxValvePosition = data.MaxValvePosition;
            newData.DeviceModel = data.DeviceModel;
            newData.DeviceName = data.DeviceName;
            newData.Diameter = data.Diameter;
            newData.Manufactor = data.Manufactor;
            newData.Curve = data.Curve;
            newData.ValvePosition = ValvePos;
            return newData;
        }

        /// <summary>
        /// GetParams
        /// </summary>
        /// <returns></returns>
        public override DeviceParam GetParams()
        {

            UnityAction<float> ua = delegate (float s) { valvePos = s; };

            DeviceParam deviceParam = new DeviceParam();
            deviceParam.paramType = DeviceParamType.EFloat;
            deviceParam.value = (float)valvePos;
            deviceParam.max = (float)((GenericValveData)deviceData).MaxValvePosition;
            deviceParam.min = (float)((GenericValveData)deviceData).MinValvePosition;
            deviceParam.name = "Valve Position";
            deviceParam.target = ua;



            return deviceParam;
        }


        /// <summary>
        /// Simulation Start Point
        /// </summary>
        protected override void RunSimulation()
        {
            // Create initial data
            FluidData fData = new FluidData();
            fData.Q = 0;
            fData.PressureLoss = pressureLoss;
            fData.Mass = 1000f;

            if (GetComponent<SlotIn>().LastDeviceData != null)
                fData.Temperature = ((FluidData)GetComponent<SlotIn>().LastDeviceData).Temperature;
            else
            {
                fData.Temperature = 40f;
            }

            if (null != GetComponent<SimMath>())
            {
                base.ForwardData(fData, 1);
            }
            else
            {
                Debug.LogError("GenericPump - RunSimulation() -> No Math Component (" + gameObject.name + ")");
            }

        }

        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}