/******************************************************************************
 * File        : GenericHeater.cs
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
    /// Heater (Device)
    /// </summary>
    public class GenericHeater : GenericDevice
    {
        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /// <summary>
        /// Heating Power 
        /// </summary>
        [SerializeField]private float power = 0;

        /// <summary>
        /// Heating Power
        /// </summary>
        public float Power
        {
            set
            {
                this.power = value;
                if (power > 2000f)
                {
                    power = 2000f;
                }
                else if (power < 0f)
                {
                    power = 0f;
                }
            }
            get
            {
                return this.power;
            }
        }

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /// <summary>
        /// GetDeviceData
        /// </summary>
        /// <returns></returns>
        public override GenericDeviceData GetDeviceData()
        {
            GenericHeaterData newData = (GenericHeaterData)ScriptableObject.CreateInstance("GenericHeaterData");
            GenericHeaterData data = (GenericHeaterData)deviceData;
            newData.DeviceModel = data.DeviceModel;
            newData.DeviceName = data.DeviceName;
            newData.Manufactor = data.Manufactor;
            newData.PowerCurve = data.PowerCurve;
            newData.DeadTime = data.DeadTime;
            newData.Power = Power;
            return newData;
        }
        /// <summary>
        /// GetParams
        /// </summary>
        /// <returns></returns>
        public override DeviceParam GetParams()
        {
            UnityAction<float> ua = delegate (float s) { power = s; };
            DeviceParam deviceParam = new();
            deviceParam.paramType = DeviceParamType.EFloat;
            deviceParam.value = power;
            deviceParam.max = ((GenericHeaterData)deviceData).MaxPower;
            deviceParam.min = ((GenericHeaterData)deviceData).MinPower;
            deviceParam.name = "Power KW";
            deviceParam.target = ua;
            return deviceParam;
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