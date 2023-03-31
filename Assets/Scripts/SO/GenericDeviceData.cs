/******************************************************************************
 * File        : GenericDeviceData.cs
 * Version     : 0.9 alpha
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
    /// Base Class for Device Data
    /// </summary>
    public class GenericDeviceData : ScriptableObject
    {

        /// <summary>
        /// Device Name
        /// </summary>
        [SerializeField] private string deviceName;

        /// <summary>
        /// Device Name
        /// </summary>
        public string DeviceName { set => deviceName = value; get => deviceName; }


        /// <summary>
        /// Model Name
        /// </summary>
        [SerializeField] private string deviceModel;

        /// <summary>
        /// Model Name
        /// </summary>
        public string DeviceModel { set => deviceModel = value; get => deviceModel; }


        /// <summary>
        /// Manufactor
        /// </summary>
        [SerializeField]private string manufactor;


        /// <summary>
        /// Model Name
        /// </summary>
        public string Manufactor { set => manufactor = value; get => manufactor; }


        /// <summary>
        /// Compute Device Volume (m3)
        /// </summary>
        /// <returns></returns>
        public virtual float ComputeVolume()
        {
            return 0f;
        }
    }
}