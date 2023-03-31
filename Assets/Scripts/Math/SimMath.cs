/******************************************************************************
 * File        : SimMath.cs
 * Version     : 0.9 Aplha
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using FrostBit;

    /// <summary>
    /// Main Class for Device Math. This class is responsible to use correct math class.
    /// Correct Math Class name must set in Unity Editor. MathName find in inspector.
    /// </summary>
    public class SimMath : MonoBehaviour
    {

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// DeviceMath
        /// </summary>
        [SerializeField] private DeviceMath deviceMath;

        /// <summary>
        /// Math Class Name
        /// </summary>
        [SerializeField] private string mathName;


        /// <summary>
        /// Propertyinfo List 
        /// </summary>
        Dictionary<int, PropertyInfo> propertyInfo = new Dictionary<int, PropertyInfo>();


        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/


        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/
        /// <summary>
        /// Awake()
        /// </summary>
        private void Awake()
        {


            GenericDevice gd = GetComponent<GenericDevice>();
            if (null == gd)
            {
                Debug.LogError("SimMath - Start() -> Cannot find Device (" + gameObject.name + ")");
            }

            var objectType = Type.GetType("FrostBit."+mathName);

            if (null == objectType)
            {
                Debug.LogError("SimMath - Start() -> Cannot Get Type Of Math : " + gameObject.name);
            }

            deviceMath = Activator.CreateInstance(objectType, (IDevice)gd) as DeviceMath;

            if (null == deviceMath)
            {
                Debug.LogError("SimMath - Start() -> Cannot Create Instance Of Device Math");
            }


            foreach (SlotIn slot in gameObject.GetComponents<SlotIn>())
            {
                string propertyName = "Data" + slot.SlotID.ToString();
                PropertyInfo prop = deviceMath.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                propertyInfo.Add(slot.SlotID, prop);

            }

        }


        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /// <summary>
        /// Set Data to device for next simulation round
        /// </summary>
        /// <param name="data"></param>
        /// <param name="slotID"></param>
        public void SetData(DeviceData data, int slotID = 1, SimulationState aState = SimulationState.EPressureLoss)
        {
            PropertyInfo prop = null;
            propertyInfo.TryGetValue(slotID, out prop);

            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(deviceMath, data);
            }

            deviceMath.Compute(aState);
        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }

