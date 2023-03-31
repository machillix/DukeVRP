/******************************************************************************
 * File        : ConsumerMath.cs
 * Version     : 0.9 (Alpha Version)
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
    /// ConsumerMath, This Math Component "Simulates" Consumers. 
    /// Simplified, the component takes some of the energy and simulates consumers
    /// in this way. The component does not take into account external factors such
    /// as temperature. It only reduces a predetermined amount of energy
    /// </summary>
    public class ConsumerMath : DeviceMath
    {

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// DeviceData
        /// </summary>
        private DeviceData data1;

        /// <summary>
        /// DeviceData
        /// </summary>
        public DeviceData Data1 { get => data1; set => data1 = value; }

        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /// <summary>
        /// This method is responsible for the math of the component
        /// </summary>
        public override void Compute(SimulationState aState = 
            SimulationState.EPressureLoss)
        {
            if (null != data1)
            {

                FluidData inData = (FluidData)data1;
                FluidData outData = new FluidData(inData);

                if (aState == SimulationState.EPressureLoss)
                {
                    float pipePressureLost = 0.01f;
                    outData.PressureLoss = pipePressureLost + inData.PressureLoss;
                }
                else if (aState == SimulationState.EParallel)
                {

                    // TODO : Hardcoded value, please change. In this case:
                    // Consumers uses 1.2MW for heating 
                    float power = 1200000f;

                    // Compute Delta Temparature
                    float dt = (power / (4.19f * inData.Q * 1000f))
                        / (3600f / (SimulationSettings.Instance.SimulationInterval 
                        * SimulationSettings.Instance.TimeMultiplier));

                    // Decrease temperature
                    outData.Temperature = inData.Temperature - dt;

                    
                    if (inData.Temperature > 40)
                        outData.Temperature = 45;
                    else
                        outData.Temperature = inData.Temperature;
                }
                data1 = null;
                deviceInterface.Send(outData, 1, aState);
            }
        }


        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/
        /***************************************************************************
         *                          PUBLIC METHODS
         **************************************************************************/

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConsumerMath()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public ConsumerMath(IDevice device) : base(device)
        {

        }

        /***************************************************************************
         *                          PRIVATE METHODS
         **************************************************************************/
    }
}