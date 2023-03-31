/******************************************************************************
 * File        : FlowRateMath.cs
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
    /// Math Class for Pipe
    /// </summary>
    public class PipeMath : DeviceMath
    {

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/


        /// <summary>
        /// DeviceData
        /// </summary>
        private DeviceData data1;



        /// <summary>
        /// DeviceData for slot 1
        /// </summary>
        public DeviceData Data1 { set => data1 = value; get =>data1; }

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
                GenericWaterPipeData data = (GenericWaterPipeData)deviceInterface.GetDeviceData();

                FluidData InData = (FluidData)data1;
                FluidData outData = new FluidData(InData);

                // Compute pressure Lost in Pipe
                if (aState == SimulationState.EPressureLoss)
                {
                    // FIXME: Pressure loss is zero in pipe
                    float pipePressureLost = 0.01f;
                    outData.PressureLoss = pipePressureLost + InData.PressureLoss;
                    deviceInterface.SetPressureLoss(outData.PressureLoss);
                }
                else if (aState == SimulationState.EParallel)
                {
                    float c = data.ComputeVolume() / (InData.Q / 3600f);
                    FluidData tmpData = new FluidData(InData);

                    this.data.Add(tmpData);
                    if (this.data.Count > 1000)
                        this.data.RemoveAt(0);

                    outData.FlowID = InData.FlowID;
                    outData.TimeStamp = tmpData.TimeStamp;
                    outData.Temperature = tmpData.Temperature;
                    outData.PressureLoss = tmpData.PressureLoss;
                    outData.Mass = tmpData.Mass;
                    outData.FlowRate = tmpData.FlowRate;
                    outData.Q = tmpData.Q;

                    float temp = 0;
                    int count = 0;
                    foreach (DeviceData a in this.data)
                    {
                        if (Time.realtimeSinceStartup >= (a.TimeStamp + c))
                        {
                            temp = ((FluidData)a).Temperature;
                            count++;

                        }
                        else if (count == 0)
                        {
                            // USE FIRST BUT DO NOT DELETE IT
                            temp = ((FluidData)a).Temperature;
                            break;
                        }
                        else
                        {
                            break;
                        }
                        outData.Temperature = temp;
                    }
                    if (count > 1)
                    {
                        this.data.RemoveRange(0, count - 1);
                    }


                }
                data1 = null;
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
        public PipeMath()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public PipeMath(IDevice device) : base(device)
        {

        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}