/******************************************************************************
 * File        : GenericHeaterData.cs
 * Version     : 0.9 Alpha
 * Author      : Toni Westerlund (toni.westerlund@lapinamk.com)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2021 Lapland University of Applied Sciences
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
    /// Heater Data
    /// </summary>
    [CreateAssetMenu(fileName = "GenericHeaterData", menuName = "Heater/Generic Heater")]
    public class GenericHeaterData : GenericDeviceData
    {


        /// <summary>
        /// Heater Min Power (idle Power)
        /// </summary>
        [Tooltip("Heater Min Power (idle Power)")]
        [SerializeField] private float minPower;

        /// <summary>
        /// Heater Min Power (idle Power)
        /// </summary>
        public float MinPower { get => minPower; }


        /// <summary>
        /// Heater Max Power
        /// </summary>
        [Tooltip("Heater Max Power")]
        [SerializeField] private float maxPower;

        /// <summary>
        /// Heater Max Power
        /// </summary>
        public float MaxPower { get => maxPower; }


        /// <summary>
        /// Heater Dead Time (seconds)
        /// </summary>
        [Tooltip("Heater Dead Time (seconds)")]
        [SerializeField] private float deadTime;

        /// <summary>
        /// Heater Dead Time (seconds)
        /// </summary>
        public float DeadTime { set => deadTime = value; get => deadTime; }


        /// <summary>
        /// Heater Power Curve
        /// </summary>
        [Tooltip("Heater Power Curve")]
        [SerializeField] private AnimationCurve powerCurve;

        /// <summary>
        /// Heater Power Curve
        /// </summary>
        public AnimationCurve PowerCurve { set => powerCurve = value; get => powerCurve;  }


        /// <summary>
        /// Power of Heater
        /// </summary>
        [Tooltip("Power of Heater")]
        [SerializeField] private float power;

        /// <summary>
        /// Power of Heater
        /// </summary>
        public float Power { set => power = value; get => power; }



    }
}