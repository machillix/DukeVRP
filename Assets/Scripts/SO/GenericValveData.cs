/******************************************************************************
 * File        : GenericValveData.cs
 * Version     : 0.9
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
    /// Valve Data Sheet
    /// </summary>
    [CreateAssetMenu(fileName = "GenericValveData", menuName = "Valves/Generic Valve")]
    public class GenericValveData : GenericWaterPipeData
    {

        /// <summary>
        /// Valve Curve Position/Flow
        /// </summary>
        [Tooltip("Valve Curve Position/Flow")]
        [SerializeField] private AnimationCurve curve;

        /// <summary>
        /// Valve Curve Position/Flow
        /// </summary>
        public AnimationCurve Curve { set => curve = value; get => curve; }


        /// <summary>
        /// Min Position of Valve
        /// </summary>
        [Tooltip("Min Position of Valve")]
        [SerializeField] private float minValvePosition;

        /// <summary>
        /// Min Position of Valve
        /// </summary>
        public float MinValvePosition { set => minValvePosition = value; get => minValvePosition; }



        /// <summary>
        /// Max Position of Valve
        /// </summary>
        [Tooltip("Max Position of Valve")]
        [SerializeField] private float maxValvePosition;

        /// <summary>
        /// Max Position of Valve
        /// </summary>
        public float MaxValvePosition { set => maxValvePosition = value; get => maxValvePosition; }


        /// <summary>
        /// Current Valve Position (Initial Position)
        /// </summary>
        private float valvePosition;


        /// <summary>
        /// Current Valve Position (Initial Position)
        /// </summary>
        public float ValvePosition { set => valvePosition = value; get => valvePosition; }



    }
}