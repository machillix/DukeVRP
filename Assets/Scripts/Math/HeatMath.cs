/******************************************************************************
 * File        : HeatMath.cs
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
using System.IO;
using UnityEngine;


namespace FrostBit
{

    /// <summary>
    /// Math Class for Heating Boiler
    /// </summary>
    public class HeatMath : DeviceMath
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


        /// <summary>
        /// Last Request Power
        /// </summary>
        private float lastPowerRequest = -1;

        /// <summary>
        /// Current Power of Boiler
        /// </summary>
        private float currentPower = 0;

        /// <summary>
        /// Structure of Heatdata
        /// </summary>
        class HeatMathStruct
        {

            /// <summary>
            /// power
            /// </summary>
            private float power;

            /// <summary>
            /// Power 
            /// </summary>
            public float Power { set => power = value; get => power; }

            /// <summary>
            /// Delta Power
            /// </summary>
            private float deltaPower;

            /// <summary>
            /// DeltaPower
            /// </summary>
            public float DeltaPower { set => deltaPower = value; get => deltaPower; }

            /// <summary>
            /// timeStamp
            /// </summary>
            private float timeStamp;


            /// <summary>
            /// Time Stamp
            /// </summary>
            public float TimeStamp { set => timeStamp = value; get => timeStamp; }

            /// <summary>
            /// currentCurveTime
            /// </summary>
            private float currentCurveTime;

            /// <summary>
            /// Current Curve Time
            /// </summary>
            public float CurrentCurveTime { set => currentCurveTime = value; get => currentCurveTime; }


            /// <summary>
            /// Constructor For Structure
            /// </summary>
            public HeatMathStruct(float power, float deltaPower, float timeStamp, float currentCurveTime)
            {
                this.power = power;
                this.deltaPower = deltaPower;
                this.timeStamp = timeStamp;
                this.currentCurveTime = currentCurveTime;
            }
        }

        /// <summary>
        /// List of HeatMath Structure
        /// </summary>
        private List<HeatMathStruct> heatMathStructList = new List<HeatMathStruct>();

        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /// <summary>
        /// Compute
        /// </summary>
        /// <param name="state"></param>
        public override void Compute(SimulationState state = SimulationState.EPressureLoss)
        {

            // Formulas
            // E=m∙c∙ΔT 
            // ΔT = E / ( m ∙ c )
            // 1 kJ = 0.000278 kwh; 1 kwh = 3600 kJ
            // 1 m3/h = 0.277778 L/s; 1 L/s = 3.6 m3/h

            // Q = (p ∙c ∙ v ∙ (t1-t2))/3600
            // Q = Needed Power

            // For ex.
            // 2MW = 2000kj/s
            // 80m3/h = 22.2 l/s
            // 20 / ( 0.2222 * 4.19) = 20 / 0.93 = 


            if (null != data1)
            {
                GenericHeaterData data = (GenericHeaterData)deviceInterface.GetDeviceData();

                FluidData inData = (FluidData)data1;
                FluidData outData = new FluidData(inData);

                if (state == SimulationState.EPressureLoss)
                {
                    float pipePressureLost = 0.01f;
                    outData.PressureLoss = pipePressureLost + inData.PressureLoss;
                }
                else if (state == SimulationState.EParallel)
                {
                    float k = 1; // change time in seconds

                    outData.Temperature = inData.Temperature + ((data.Power) / (1000f * 4.2f * inData.Q) / 50f);


                    // If Power Request is changed
                    if (data.Power != lastPowerRequest)
                    {
                        // Set new exist time for data
                        if (heatMathStructList.Count > 0)
                        {
                            heatMathStructList[(heatMathStructList.Count - 1)].TimeStamp = Time.realtimeSinceStartup + data.DeadTime;
                        }

                        // Set new data for heater
                        HeatMathStruct heatMathStruct = new HeatMathStruct(data.Power, 0,
                            data.DeadTime + Time.realtimeSinceStartup, Time.realtimeSinceStartup + data.DeadTime + k);
                        heatMathStructList.Add(heatMathStruct);

                        lastPowerRequest = data.Power;

                    }

                    // Remove data from list (Check Exit time)
                    if (heatMathStructList.Count > 1 && heatMathStructList[0].TimeStamp < Time.realtimeSinceStartup)
                    {
                        heatMathStructList.RemoveAt(0);
                        heatMathStructList[0].DeltaPower = currentPower;
                        heatMathStructList[0].Power = data.Power - currentPower;
                    }
                    float power = heatMathStructList[0].Power;
                    float eTime = (Time.realtimeSinceStartup - heatMathStructList[0].CurrentCurveTime) / k;

                    // Get new power from curve
                    if (eTime <= 1)
                    {
                        power = heatMathStructList[0].DeltaPower + data.PowerCurve.Evaluate(eTime) * power;
                    }
                    float dt = (power / (4.19f * (inData.Q / 3.6f)));
                    outData.Temperature = inData.Temperature + dt;

                    if (outData.Temperature > 120) outData.Temperature = 120; // FIXME: Limiter

                }
                data1 = null;
                deviceInterface.Send(outData, 1, state);
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

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HeatMath()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public HeatMath(IDevice device) : base(device)
        {

        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}