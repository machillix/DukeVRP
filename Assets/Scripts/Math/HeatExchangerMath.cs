/******************************************************************************
 * File        : HeatExchangerMath.cs
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
    public class HeatExchangerMath : DeviceMath
    {

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// DeviceData
        /// </summary>
        private DeviceData data1;


        /// <summary>
        /// DeviceData slot 2
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

        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /// <summary>
        /// Computing heat Excanger math. Heat Excanger has two slots in and two slots out
        /// Both of Slot in data must set before start to compute math
        /// </summary>
        public override void Compute(SimulationState aState = SimulationState.EPressureLoss)
        {
            if (null == data1 || null == data2)
            {
                // No Data
                return;
            }
            FluidData inData1 = (FluidData)data1;
            FluidData outData1 = new FluidData(inData1);

            FluidData inData2 = (FluidData)data2;
            FluidData outData2 = new FluidData(inData2);


            if (aState == SimulationState.EPressureLoss)
            {
                // FIXME: Pressure loss is now "hard defined"
                float pipePressureLost = 0.01f;
                outData1.PressureLoss = pipePressureLost + inData1.PressureLoss;
                outData2.PressureLoss = pipePressureLost + inData2.PressureLoss;
            }
            else if (aState == SimulationState.EParallel)
            {

                // Math for counterflow heat exchanger
                // Material and Links
                // http://fireflylabs.com/disted/courses/m262(2014)/docs/Will-Week8/m262-HeatExchanges-Part2.pdf
                // https://www.mathworks.com/help/physmod/hydro/ref/entuheattransfer.html
                // https://www.pdhonline.com/courses/m371/m371content.pdf
                // Q = heat energy (Joules) (Btu);
                // m = mass of the substance(kilograms)(pounds);
                // Cp = specific heat of the substance(J/ kg°C) (Btu / pound /°F);
                // (T2 – T1) = is the change in temperature(°C)(°F).

                // Specific Heat of Water (J/kg K)
                float cp = 4190f;

                // Overall heat transfer coefficient (W/m^2 K)
                float U = 300f;

                // Heat Transfer Area (Square Meters)
                float A = 300f;

                // Primary side incoming Temperature
                float t1 = inData1.Temperature;
                // Secondary side incoming Temperature
                float t2 = inData2.Temperature;
                // Primary side incoming Volume Flow
                float m1 = inData1.Q / 3.6f;
                // Secondary side incoming Volume Flow
                float m2 = inData2.Q / 3.6f;


                float mCp_1 = cp * m1;
                float mCp_2 = cp * m2;

                // NTU method
                // https://en.wikipedia.org/wiki/NTU_method
                float C_min = Mathf.Min(mCp_1, mCp_2);
                float C_max = Mathf.Max(mCp_1, mCp_2);

                // heat capacity ratio
                float Cr = C_min / C_max;

                // number of transfer units
                float NTU = U * A / C_min;

                // Init e
                float e = 0;

                if (Cr == 1)
                {
                    // For counter-current flow heat exchanger 
                    e = NTU / (1 + NTU);
                }
                else
                {
                    // effectiveness of a counter-current flow heat exchanger
                    e = (1 - Mathf.Exp(-NTU * (1 - Cr))) / (1 - Cr * Mathf.Exp(-NTU * (1 - Cr)));
                }

                // Q = m.Cp. (T2 – T1), use effectiveness
                float Q = e * C_min * (t1 - t2);

                // Outlet temperature are estimated as following :
                // https://cheguide.com/heat_analysis.html
                float b2 = Q / mCp_2 + t2;
                float b1 = t1 - Q / mCp_1;

                outData2.Temperature = b2; // Secondary outcoming temp

                outData1.Temperature = b1; // Primary outcoming temp
            }

            // Forward data and clear incoming data
            data2 = null;
            data1 = null;
            deviceInterface.Send(outData1, 1, aState);
            deviceInterface.Send(outData2, 2, aState);

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
        public HeatExchangerMath()
        {

        }

        /// <summary>
        /// Overloaded Constructor, Use this constructor, Math class needs reference
        /// to device
        /// </summary>
        /// <param name="device"></param>
        public HeatExchangerMath(IDevice device) : base(device)
        {

        }
        /***************************************************************************
         *                          PRIVATE METHODS
         **************************************************************************/
    }

}