/******************************************************************************
 * File        : GenericWaterPipe.cs
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

namespace FrostBit
{
    /// <summary>
    /// Water Pipe (Device)
    /// </summary>
    public class GenericWaterPipe : GenericDevice
    {

        /// <summary>
        /// RunSimulation
        /// </summary>
        protected override void RunSimulation()
        {
            base.RunSimulation();

            GenericWaterPipeData pumpData = (GenericWaterPipeData)deviceData;

            // Create initial data
            FluidData fData = new FluidData();
            fData.PressureLoss = 0;
            fData.Mass = 1000f;

            fData.FlowID = gameObject.GetInstanceID();
            if (false == SimulationSettings.Instance.Circuits.ContainsKey(fData.FlowID))
            {
                CircuitData circuitData = new CircuitData();
                circuitData.PressureLost = 0;

                SimulationSettings.Instance.Circuits.Add(fData.FlowID, circuitData);
            }

            if (GetComponent<SlotIn>().LastDeviceData != null)
            {
                fData.Temperature = ((FluidData)GetComponent<SlotIn>().LastDeviceData).Temperature;
                fData.Q = ((FluidData)GetComponent<SlotIn>().LastDeviceData).Q;
            }
            else
            {
                fData.Temperature = 40f;
            }

            if (null != GetComponent<SimMath>())
            {
                base.ForwardData(fData, 1, SimulationState.EPumpRound);
            }
            else
            {
                Debug.LogError("GenericPump - RunSimulation() -> No Math Component (" + gameObject.name + ")");
            }

        }
    }
}