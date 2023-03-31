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
    /// Math Class for T Pipe
    /// </summary>
    public class PipeTMath2In1 : DeviceMath
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
        private DeviceData data2;


        /// <summary>
        /// DeviceData for slot 1
        /// </summary>
        public DeviceData Data1 { set => data1 = value; get => data1; }

        /// <summary>
        /// DeviceData for slot 2
        /// </summary>
        public DeviceData Data2 { set => data2 = value; get => data2; }


        private float prepressureLost1 = 0;

        private float prepressureLost2 = 0;

        private GenericDevice gda;
        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /// <summary>
        /// Compute()
        /// </summary>
        public override void Compute(SimulationState aState = SimulationState.EPressureLoss)
        {

            if (null != data1 && null != data2)
            {
                // Compute Flow Rate
                GenericPipeTData data = (GenericPipeTData)deviceInterface.GetDeviceData();

                FluidData fData = new FluidData();
                FluidData inFData = (FluidData)data1;
                FluidData in2FData = (FluidData)data2;
                FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[inFData.FlowID];

                int tmpCur = flowData.Parent;
                // TODO: ADD TREE SEARCH LOOP 
                fData.FlowID = tmpCur;
                fData.CircuitID = inFData.CircuitID;


                if (aState == SimulationState.EPreSimulation)
                {

                    FlowData flowData1 = (FlowData)SimulationSettings.Instance.Flows[inFData.FlowID];

                    gda = flowData1.Node.GenericDevice;

                    FlowData flowData2 = (FlowData)SimulationSettings.Instance.Flows[in2FData.FlowID];

                    AnimationCurve tmp1 = null;
                    AnimationCurve tmp2 = null;
                    AnimationCurve tmp3 = null;

                    if (flowData1.SerialPump != null)
                    {
                        tmp1 = MathHelper.FlipCurve(flowData1.SerialPump);
                        tmp3 = tmp1;
                    }
                    if (flowData2.SerialPump != null)
                    {
                        tmp2 = MathHelper.FlipCurve(flowData2.SerialPump);
                        if (tmp3 != null)
                        {
                            tmp3 = MathHelper.FlipCurve(MathHelper.SumCurve(tmp1, tmp2, prepressureLost1, prepressureLost2));
                        }
                        else
                        {
                            tmp3 = tmp2;
                        }
                    }
                    FlowData flowDataPump = (FlowData)SimulationSettings.Instance.Flows[tmpCur];

                    if (tmp3 != null)
                    {
                        flowDataPump.SerialPump = MathHelper.SumCurve(flowDataPump.SerialPump, tmp3);
                    }

                    fData.Temperature = inFData.Temperature;
                    fData.Mass = inFData.Mass;
                    fData.FlowRate = inFData.FlowRate;
                    fData.Q = in2FData.Q + inFData.Q;
                    fData.PressureLoss = inFData.PressureLoss + in2FData.PressureLoss;
                }

                else if (aState == SimulationState.EPumpRound)
                {

                    FlowData flowData1 = (FlowData)SimulationSettings.Instance.Flows[inFData.FlowID];
                    FlowData flowData2 = (FlowData)SimulationSettings.Instance.Flows[in2FData.FlowID];

                    AnimationCurve tmp1 = null;
                    AnimationCurve tmp2 = null;
                    AnimationCurve tmp3 = null;

                    if (flowData1.SerialPump != null)
                    {
                        tmp1 = MathHelper.FlipCurve(flowData1.SerialPump);
                        tmp3 = tmp1;
                    }

                    if (flowData2.SerialPump != null)
                    {
                        tmp2 = MathHelper.FlipCurve(flowData2.SerialPump);
                        if (tmp3 != null)
                        {
                            tmp3 = MathHelper.FlipCurve(MathHelper.SumCurve(tmp1, tmp2, prepressureLost1, prepressureLost2));
                        }
                        else
                        {
                            tmp3 = tmp2;
                        }
                    }

                    FlowData flowDataPump = (FlowData)SimulationSettings.Instance.Flows[tmpCur];
                    if (tmp3 != null)
                    {
                        flowDataPump.SerialPump = MathHelper.SumCurve(flowDataPump.SerialPump, tmp3);
                    }

                    fData.Temperature = inFData.Temperature;
                    fData.Mass = inFData.Mass;
                    fData.FlowRate = inFData.FlowRate;
                    fData.Q = in2FData.Q + inFData.Q;
                    fData.PressureLoss = inFData.PressureLoss + in2FData.PressureLoss;

                }
                else if (aState == SimulationState.EPressureLoss)
                {
                    float pressureLost = 0.01f;

                    pressureLost = pressureLost + ((inFData.PressureLoss * in2FData.PressureLoss) / (inFData.PressureLoss + in2FData.PressureLoss));

                    prepressureLost1 = inFData.PressureLoss;
                    prepressureLost2 = in2FData.PressureLoss;

                    fData.Temperature = inFData.Temperature;
                    fData.Mass = inFData.Mass;
                    fData.FlowRate = inFData.FlowRate;
                    fData.Q = inFData.Q + in2FData.Q;

                    GenericDevice gd = deviceInterface.GetParellel();
                    // Ennen T haaraa oleva paine häviö
                    float pressureLost_1 = gda.GetPressureLoss();

                    gda.UpdateData(inFData, 1);
                    gda.UpdateData(in2FData, 2);

                    // Ulos päin häviö on rinnan + ennen t haaraa
                    fData.PressureLoss = pressureLost + pressureLost_1;

                }
                else
                {
                    float tM = in2FData.Q + inFData.Q;
                    float aT = in2FData.Temperature;
                    float aM = in2FData.Q;

                    float bT = inFData.Temperature;
                    float bM = inFData.Q;

                    float dT = aT - bT;

                    float oT = bT + ((aM / tM) * (aT - bT));

                    fData.Mass = inFData.Mass;
                    fData.FlowRate = inFData.FlowRate;
                    fData.Temperature = oT;

                    fData.Q = inFData.Q + in2FData.Q;
                }

                // Clear Data
                data1 = null;
                data2 = null;

                // Send Data to forward (to next component)
                deviceInterface.Send(fData, 1, aState);


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
        public PipeTMath2In1()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public PipeTMath2In1(IDevice device) : base(device)
        {

        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}