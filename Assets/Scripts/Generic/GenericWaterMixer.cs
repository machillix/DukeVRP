/******************************************************************************
 * File        : GenericWaterPipe.cs
 * Version     : 0.9 Alpha
 * Author      : Toni Westerlund (toni.westerlund@lapinamk.com)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2022 Lapland University of Applied Sciences
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

namespace FrostBit
{
    /// <summary>
    /// GenericWaterPipe
    /// </summary>
    public class GenericWaterMixer : GenericDevice
    {


        /// <summary>
        /// Valve position, 0 = closed, 1 = Full open
        /// </summary>
        [SerializeField]
        private float valvePos = 0;


        /// <summary>
        /// Get Device Data
        /// </summary>
        /// <returns></returns>
        public override GenericDeviceData GetDeviceData()
        {
            Debug.Log("GenericWaterValve() - GetDeviceData()");
            GenericWaterPipeData newData = new GenericWaterPipeData();
            GenericWaterPipeData data = (GenericWaterPipeData)deviceData;

            newData.DeviceModel = data.DeviceModel;
            newData.DeviceName = data.DeviceName;
            newData.Diameter = data.Diameter;
            newData.Manufactor = data.Manufactor;
            newData.Diameter = newData.Diameter * valvePos;
            return newData;
        }

        /// <summary>
        /// ForwardData()
        /// </summary>
        /// <param name="aDeviceData"></param>
        public override void ForwardData(DeviceData aDeviceData, int aSlotId, SimulationState aState = SimulationState.EPressureLoss)
        {
            if (aDeviceData.GetType() == typeof(WaterPipeData))
            {
                WaterPipeData data = (WaterPipeData)aDeviceData;
                data.diameter = data.diameter * valvePos;
                aDeviceData = data;
            }
            base.ForwardData(aDeviceData, aSlotId);
        }


    }
}