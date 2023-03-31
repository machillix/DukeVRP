/******************************************************************************
 * File        : ValveLog.cs
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
using System.Text;
using UnityEngine;


namespace FrostBit
{

    /// <summary>
    /// ValveLog
    /// </summary>
    public class ValveLog : MonoBehaviour
    {

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// GenericDevice
        /// </summary>
        private GenericDevice device;

        /// <summary>
        /// GenericSlot
        /// </summary>
        private GenericSlot slot;

        /// <summary>
        /// filePath
        /// </summary>
        [Tooltip("Log File path")]
        [SerializeField] private string filePath = "d:\\ValveDebug.csv";

        /// <summary>
        /// strSeperator
        /// </summary>
        [Tooltip("Data Separator, for ex. use ; for cvs files")]
        [SerializeField] private string strSeperator = ";";

        /// <summary>
        /// activateAutoShutDown
        /// </summary>
        [SerializeField] private bool activateAutoShutDown = false;



        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any
        /// of the Update methods are called the first time.
        /// </summary>
        public void Start()
        {
            slot = gameObject.GetComponent<GenericSlot>();
            device = gameObject.GetComponent<GenericDevice>();
            InvokeRepeating("MakeLog", 0, 0.1f);
        }

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/

        /// <summary>
        /// MakeLog
        /// - Write Log data to file
        /// </summary>
        private void MakeLog()
        {
            if (activateAutoShutDown == true)
            {
                if (((GenericValve)device).ValvePos >= 0)
                {
                    ((GenericValve)device).ValvePos -= 1f;
                }
            }

            GenericValveData data = (GenericValveData)device.GetDeviceData();
            FluidData fData = (FluidData)slot.LastDeviceData;
            File.AppendAllText(filePath, data.ValvePosition + strSeperator + fData.Q + "\n");

        }

    }

}