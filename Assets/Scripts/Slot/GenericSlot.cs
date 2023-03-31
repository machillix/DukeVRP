/******************************************************************************
 * File        : GenericSlot.cs
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
    /// Slot Base Class
    /// </summary>
    public class GenericSlot : MonoBehaviour
    {
        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /// <summary>
        /// Slot ID
        /// </summary>
        [SerializeField] protected int slotID = 1;

        /// <summary>
        /// Slot ID
        /// </summary>
        public int SlotID { get => slotID; }


        /// <summary>
        /// Last received Device Data
        /// </summary>
        protected DeviceData lastDeviceData;

        /// <summary>
        /// Last received Device Data
        /// </summary>
        public DeviceData LastDeviceData { get => lastDeviceData; }
        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// List Of PID
        /// </summary>
        protected List<GeneralPID> listOfPID = new List<GeneralPID>();


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
        /// Add PID to Slot
        /// </summary>
        /// <param name="pid"></param>
        public void AddPIDtoSlot(GeneralPID pid)
        {
            listOfPID.Add(pid);
        }



        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/


    }

}