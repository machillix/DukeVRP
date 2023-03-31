/******************************************************************************
 * File        : DeviceData.cs
 * Version     : 0
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
    /// Fluid Data
    /// </summary>
    public class FluidData : DeviceData
    {
        /// <summary>
        /// Fluid Temperature 
        /// </summary>
        private float temperature;

        /// <summary>
        /// Fluid Temperature 
        /// </summary>
        public float Temperature { set => temperature = value; get => temperature; }

        /// <summary>
        /// Fluid Mass
        /// </summary>
        private float mass;

        /// <summary>
        /// Fluid Mass
        /// </summary>
        public float Mass { set => mass = value; get => mass; }

        /// <summary>
        /// Fluid Flow Rate
        /// </summary>
        private float flowRate;

        /// <summary>
        /// Fluid Flow Rate
        /// </summary>
        public float FlowRate { set => flowRate = value; get => flowRate; }

        /// <summary>
        /// Fluid Q
        /// </summary>
        private float q;

        /// <summary>
        /// Fluid Q
        /// </summary>
        public float Q { set => q = value; get => q; }

        /// <summary>
        /// maxCubicMeterPerHour
        /// </summary>
        private float maxCubicMeterPerHour;

        /// <summary>
        /// System Pressure Lost
        /// </summary>
        private float pressureLoss;

        /// <summary>
        /// System Pressure Lost
        /// </summary>
        public float PressureLoss { set => pressureLoss = value; get => pressureLoss; }

        /// <summary>
        /// Circuit Identifier
        /// </summary>
        private int circuitID;

        /// <summary>
        /// Circuit Identifier
        /// </summary>
        public int CircuitID { set => circuitID = value; get => circuitID; }

        /// <summary>
        /// Flow Identifier
        /// </summary>
        private int flowID;

        /// <summary>
        /// Flow Identifier
        /// </summary>
        public int FlowID { set => flowID = value; get => flowID; }


        /// <summary>
        /// Fluid Constructor
        /// </summary>
        /// <param name="fluidData"></param>
        public FluidData(FluidData fluidData)
        {
            temperature = fluidData.temperature;
            mass = fluidData.mass;
            flowRate = fluidData.flowRate;
            q = fluidData.q;
            maxCubicMeterPerHour = fluidData.maxCubicMeterPerHour;
            pressureLoss = fluidData.pressureLoss;
            circuitID = fluidData.circuitID;
            flowID = fluidData.flowID;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FluidData()
        {
            temperature = 0;
            mass = 0;
            flowRate = 0;
            q = 0;
            maxCubicMeterPerHour = 0;
            pressureLoss = 0;
            circuitID = 0;
            flowID = 0;
        }

    }

}