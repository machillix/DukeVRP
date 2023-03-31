/******************************************************************************
 * File        : FlowRateMath.cs
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
    /// FlowRateMath
    /// </summary>
    public class PumpMath : DeviceMath
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
                GenericPumpData data = (GenericPumpData)deviceInterface.GetDeviceData();
                FluidData inFData = (FluidData)data1;
                FluidData outData = new FluidData(inFData);


                if (aState == SimulationState.EPumpRound)
                {
                    FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[outData.FlowID];

                    if (flowData.SerialPump != null)
                    {
                        flowData.SerialPump = MathHelper.SumCurve(flowData.SerialPump, data.Curve);
                    }
                    else
                    {
                        flowData.SerialPump = data.Curve;
                        flowData.Ready = false;
                    }
                }
                else if (aState == SimulationState.EPressureLoss)
                {

                    if (SimulationSettings.Instance.Flows.ContainsKey(outData.CircuitID))
                    {
                        FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[outData.CircuitID];

                        if (flowData.Ready == false)
                        {
                            outData.Q = MathHelper.FlipCurve(flowData.SerialPump).Evaluate(((CircuitData)SimulationSettings.Instance.Circuits[outData.CircuitID]).PressureLost * 10f);
                            flowData.Ready = true;
                        }
                    }
                }
                else if (aState == SimulationState.EParallel)
                {

                    FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[outData.CircuitID];

                    if (flowData.Ready == false)
                    {
                        float kPa = ((CircuitData)SimulationSettings.Instance.Circuits[outData.CircuitID]).PressureLost;

                        //float delta = (kPa - fDataIn.PressureLoss)/100f;
                        //kPa -= delta;

                        float m = 1;
                        /* if(Mathf.Abs(kPa - fDataIn.PressureLoss) > 1000)
                             m = 1000;
                         else if(Mathf.Abs(kPa - fDataIn.PressureLoss) > 100)
                             m = 100;
                             Debug.LogError(Mathf.Abs(kPa - fDataIn.PressureLoss));*/
                        if (kPa > (inFData.PressureLoss)) { kPa -= 0.001f * m; }
                        else { kPa += 0.001f * m; }

                        ((CircuitData)SimulationSettings.Instance.Circuits[outData.CircuitID]).PressureLost = kPa;


                        outData.Q = MathHelper.FlipCurve(flowData.SerialPump).Evaluate(kPa * 10f);
                        flowData.Ready = true;
                    }
                }

                // Forward Fluid Data
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
        public PumpMath()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public PumpMath(IDevice device): base(device)
        {

        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}