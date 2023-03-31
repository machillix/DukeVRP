/******************************************************************************
 * File        : SlotIn.cs
 * Version     : 0.1 Alpha
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
    /// Divice Slot In
    /// </summary>
    public class SlotIn : GenericSlot, ISlot
    {


        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// reference to connected slot
        /// </summary>
        [SerializeField] private SlotOut slot;

        /// <summary>
        /// Device Data Sheet
        /// </summary>
        private GenericDevice genericDevice;

        /// <summary>
        /// simMath
        /// </summary>
        private SimMath simMath;


        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// 
        /// Register in  out slots
        /// </summary>
        public void OnEnable()
        {
            slot.RegisterSlot(this);

        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Awake is called either when an active GameObject that contains the 
        /// script is initialized when a Scene loads, or when a previously inactive 
        /// GameObject is set to active, or after a GameObject created with 
        /// Object.Instantiate is initialized.Use Awake to initialize variables 
        /// or states before the application starts.
        /// Initialize Gameobject, get reference to GenericDevice and SimMath objects
        /// </summary>
        public void Awake()
        {
            genericDevice = GetComponent<GenericDevice>();
            simMath = GetComponent<SimMath>();
        }


        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /// <summary>
        /// Returns Device Data
        /// </summary>
        /// <returns></returns>
        public GenericDeviceData GetDeviceData()
        {
            return genericDevice.DeviceData;
        }

        /// <summary>
        /// SlotOut object call this methdod. Main purpose is forward data from slotout
        /// to slot in.
        /// </summary>
        /// <param name="aDeviceData"></param>
        public void Ready(DeviceData deviceData, SimulationState state = SimulationState.EPressureLoss)
        {
            // Make new TimeStamp;
            deviceData.TimeStamp = Time.realtimeSinceStartup;
            lastDeviceData = deviceData;

            FluidData inFData = (FluidData)deviceData;

            if (genericDevice.simulationStartPoint == false && null != simMath)
            {
                genericDevice.ForwardData(deviceData, SlotID, state);
            }
            else if (genericDevice.simulationStartPoint == true && state == SimulationState.EPumpRound)
            {
                // First Round
                state = SimulationState.EPressureLoss;
                foreach (DictionaryEntry key in SimulationSettings.Instance.Flows)
                {
                    FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[key.Key];
                    flowData.Ready = false;
                }
                genericDevice.ForwardData(deviceData, SlotID, state);
                return;
            }
            else if (genericDevice.simulationStartPoint == true && state == SimulationState.EPressureLoss)
            {
                // Second Round
                state = SimulationState.EParallel;
                foreach (DictionaryEntry key in SimulationSettings.Instance.Flows)
                {
                    FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[key.Key];
                    flowData.Ready = false;
                }
                genericDevice.ForwardData(deviceData, SlotID, state);
                return;
            }
            else if (genericDevice.simulationStartPoint == true && state == SimulationState.EParallel)
            {
                // Last round -> Stop simulation

                foreach (DictionaryEntry key in SimulationSettings.Instance.Flows)
                {
                    FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[key.Key];
                    flowData.SerialPump = null;
                }
                if (true == SimulationSettings.Instance.Simulations.ContainsKey(inFData.CircuitID))
                {
                    SimulationSettings.Instance.Simulations[inFData.CircuitID] = true;
                }
                return;
            }
            else if (genericDevice.simulationStartPoint == true && state == SimulationState.EPreSimulation)
            {
                // Pre Simulation Round
                foreach (DictionaryEntry key in SimulationSettings.Instance.Flows)
                {
                    FlowData flowData = (FlowData)SimulationSettings.Instance.Flows[key.Key];
                    flowData.SerialPump = null;
                }
                if (true == SimulationSettings.Instance.Simulations.ContainsKey(inFData.CircuitID))
                {
                    SimulationSettings.Instance.Simulations[inFData.CircuitID] = true;
                }

                return;
            }

        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/

    }
}