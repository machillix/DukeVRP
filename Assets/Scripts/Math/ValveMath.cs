/******************************************************************************
 * File        : ValveMath.cs
 * Version     : 0.9
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
    /// ValveMath
    /// </summary>
    public class ValveMath : DeviceMath
    {



        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// DeviceData
        /// </summary>
        private DeviceData data1;

        /// <summary>
        /// DeviceData for Slot 1
        /// </summary>
        public DeviceData Data1 { set => data1 = value; get => data1; }


        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /// <summary>
        /// Compute()
        /// </summary>
        public override void Compute(SimulationState aState = SimulationState.EPressureLoss)
        {

            if (null != data1)
            {
                GenericValveData data = (GenericValveData)deviceInterface.GetDeviceData();

                FluidData inData = (FluidData)data1;
                FluidData outData = new FluidData(inData);

                // Compute Pressure Loss
                if (aState == SimulationState.EPressureLoss)
                {
                    // Valve Pressure Loss = p1 * (Q/Kv)^2 * 1/1000
                    // Get kv from curve
                    float kv = data.Curve.Evaluate((data.ValvePosition));
                    // Compute Pressure Lost
                    float pressureLoss = inData.Mass * (Mathf.Pow((inData.Q / kv), 2)) * (1f / 1000f);


                    // Add valve pressure lost to total lost
                    outData.PressureLoss = pressureLoss + inData.PressureLoss;
                }
                data1 = null;
                // Forward incoming data
                deviceInterface.Send(outData, 1, aState);

            }
        }
        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ValveMath()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public ValveMath(IDevice device) : base(device)
        {

        }
        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/

    }
}