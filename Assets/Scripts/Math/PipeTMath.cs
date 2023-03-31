/******************************************************************************
 * File        : PipeTMath.cs
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
    /// Math class for T Pipe
    /// </summary>
    public class PipeTMath : DeviceMath
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
                FluidData inData = (FluidData)data1;
                FluidData outData = new FluidData(inData);
                FluidData outData2 = new FluidData(inData);


                outData.FlowID = deviceInterface.GetSlotOutInstanceID(1);
                outData.CircuitID = inData.CircuitID;

                outData2.FlowID = deviceInterface.GetSlotOutInstanceID(2);
                outData2.CircuitID = inData.CircuitID;

                if (aState == SimulationState.EPumpRound)
                {

                    outData.Q = inData.Q / 2f;
                    outData.PressureLoss = inData.PressureLoss / 2f;

                    outData2.Q = inData.Q / 2f;
                    outData2.PressureLoss = inData.PressureLoss / 2f;

                }
                // Compute Pressure Loss
                else if (aState == SimulationState.EPressureLoss)
                {
                    float pressureLoss = 0.5f;

                    outData.Q = inData.Q / 2f;
                    outData.PressureLoss = 0;

                    outData2.Q = inData.Q / 2f;
                    outData2.PressureLoss = 0;

                    deviceInterface.SetPressureLoss(inData.PressureLoss + pressureLoss);

                }
                else if (aState == SimulationState.EParallel)
                {
                    FluidData data1 = (FluidData)deviceInterface.GetSlotOutData(1);
                    FluidData data2 = (FluidData)deviceInterface.GetSlotOutData(2);

                    float R = (data1.PressureLoss * data2.PressureLoss) / (data1.PressureLoss + data2.PressureLoss);

                    outData2.Q = (R / data2.PressureLoss) * inData.Q;
                    outData.Q = (R / data1.PressureLoss) * inData.Q;

                    R += deviceInterface.GetPressureLoss();
                    outData.PressureLoss = R;
                    outData2.PressureLoss = R;

                }
                else if (aState == SimulationState.EPreSimulation)
                {

                    SubFlowData subFlowData = new SubFlowData();

                    if (SimulationSettings.Instance.Flows.ContainsKey(inData.FlowID))
                    {
                        FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[inData.FlowID];
                        if (!flowData.Flows.ContainsKey(deviceInterface.GetSlotInInstanceID(1)))
                        {

                            subFlowData.GenericDevice = deviceInterface.GetDevice();
                            subFlowData.SubFlows.Add(deviceInterface.GetSlotOutInstanceID(1));
                            subFlowData.SubFlows.Add(deviceInterface.GetSlotOutInstanceID(2));
                            flowData.Flows.Add(deviceInterface.GetSlotInInstanceID(1), subFlowData);
                        }
                    }

                    // Add new Nodes to root
                    if (!SimulationSettings.Instance.Flows.ContainsKey(deviceInterface.GetSlotOutInstanceID(1)))
                    {
                        FlowData newData = new FlowData();
                        newData.Parent = inData.FlowID;
                        newData.Node = subFlowData;
                        SimulationSettings.Instance.Flows.Add(deviceInterface.GetSlotOutInstanceID(1), newData);
                    }

                    if (!SimulationSettings.Instance.Flows.ContainsKey(deviceInterface.GetSlotOutInstanceID(2)))
                    {
                        FlowData newData = new FlowData();
                        newData.Parent = inData.FlowID;
                        newData.Node = subFlowData;
                        SimulationSettings.Instance.Flows.Add(deviceInterface.GetSlotOutInstanceID(2), newData);

                    }
                }

                deviceInterface.Send(outData, 1, aState);
                deviceInterface.Send(outData2, 2, aState);

                data1 = null;
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
        public PipeTMath()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public PipeTMath(IDevice device) : base(device)
        {

        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}