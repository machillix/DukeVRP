/******************************************************************************
 * File        : GenericDevice.cs
 * Version     : 0.9 alpha
 * Author      : Toni Westerlund (toni.westerlund@lapinamk.com)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2022 Lapland University of Applied Sciences
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
    /// GenericDevice
    /// </summary>
    public class GenericDevice : MonoBehaviour, IDevice
    {


        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /// <summary>
        /// Device Data
        /// </summary>
        [SerializeField] protected GenericDeviceData deviceData;

        /// <summary>
        /// Device Data
        /// </summary>
        public GenericDeviceData DeviceData { get => deviceData; }


        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// Pressure Loss (kPa)
        /// </summary>
        [SerializeField] protected float pressureLoss;

        /// <summary>
        /// Current Q (m3)
        /// </summary>
        [SerializeField] protected float currentQ;

        /// <summary>
        /// simulationStartPoint
        /// </summary>
        [SerializeField] public bool simulationStartPoint = false;

        /// <summary>
        /// listOfSlot
        /// </summary>
        private SlotOut[] listOfSlotOut;

        /// <summary>
        /// listOfSlot
        /// </summary>
        protected SlotIn[] listOfSlotIn;

        /// <summary>
        /// SimMath
        /// </summary>
        private SimMath simMath;


        /// <summary>
        /// instanceID
        /// </summary>
        protected int instanceID;

        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /// <summary>
        /// Custom Awake Method
        /// </summary>
        protected virtual void OnAwake()
        {
            instanceID = gameObject.GetInstanceID();
            listOfSlotOut = GetComponents<SlotOut>();
            listOfSlotIn = GetComponents<SlotIn>();
            simMath = GetComponent<SimMath>();

            if (simMath == null)
            {
                Debug.LogError("GenericDevice - Awake() -> No Math Component (" + gameObject.name + ")");
                gameObject.SetActive(false);
            }

            if (listOfSlotIn.Length == 0)
            {
                Debug.LogError("GenericDevice - Awake() -> No Slots In (" + gameObject.name + ")");
                gameObject.SetActive(false);
            }

            if (listOfSlotOut.Length == 0)
            {
                Debug.LogError("GenericDevice - Awake() -> No Slots Ou (" + gameObject.name + ")");
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            OnAwake();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {

            if (simulationStartPoint)
                Simulation.OnPreSimulation += PreSimulation;

            if (simulationStartPoint)
                Simulation.OnRunSimulation += RunSimulation;

        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            if (simulationStartPoint)
                Simulation.OnPreSimulation -= PreSimulation;

            if (simulationStartPoint)
                Simulation.OnRunSimulation -= RunSimulation;
        }

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /// <summary>
        /// Send
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="slotId"></param>
        /// <param name="state"></param>
        public void Send(DeviceData data, int slotId, SimulationState state)
        {
            foreach (SlotOut a in listOfSlotOut)
            {
                if (a.SlotID == slotId)
                {
                    a.PushData(data, state);
                }
            }
        }

        /// <summary>
        /// UpdateData
        /// </summary>
        /// <param name="data"></param>
        /// <param name="slotId"></param>
        public void UpdateData(DeviceData data, int slotId)
        {
            foreach (SlotOut a in listOfSlotOut)
            {
                if (a.SlotID == slotId)
                {
                    a.SetLastDeviceData(data);
                }
            }
        }

        /// <summary>
        /// Return DeviceData (DataSheet)
        /// </summary>
        /// <returns></returns>
        public virtual GenericDeviceData GetDeviceData()
        {
            GenericDeviceData newData = (GenericDeviceData)GenericDeviceData.CreateInstance("GenericDeviceData");
            newData = deviceData;

            return deviceData;
        }

        /// <summary>
        /// GetParams
        /// </summary>
        /// <returns></returns>
        public virtual DeviceParam GetParams()
        {
            DeviceParam param = new DeviceParam();
            return param;
        }


        /// <summary>
        /// GetSlotsOut
        /// </summary>
        /// <returns></returns>
        public SlotOut[] GetSlotsOut()
        {
            return listOfSlotOut;
        }

        /// <summary>
        /// GetSlotsIn
        /// </summary>
        /// <returns></returns>
        public SlotIn[] GetSlotsIn()
        {
            return listOfSlotIn;
        }


        /// <summary>
        /// ForwardData
        /// </summary>
        /// <param name="deviceData"></param>
        /// <param name="slotId"></param>
        /// <param name="state"></param>
        public virtual void ForwardData(DeviceData deviceData, int slotId, SimulationState state = SimulationState.EPressureLoss)
        {
            simMath.SetData(deviceData,
                slotId, state);
        }

        /// <summary>
        /// Simulation Start Point
        /// </summary>
        protected virtual void RunSimulation()
        {
        }

        /// <summary>
        /// PreSimulation
        /// </summary>
        protected virtual void PreSimulation()
        {
        }


        /// <summary>
        /// GetParellel
        /// </summary>
        /// <returns></returns>
        public virtual GenericDevice GetParellel()
        {
            return null;
        }

        /// <summary>
        /// GetParellel
        /// </summary>
        /// <returns></returns>
        public virtual GenericDevice GetDevice()
        {
            return this;
        }

        /// <summary>
        /// GetSlotOutData
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public DeviceData GetSlotOutData(int slotId)
        {
            foreach (SlotOut a in listOfSlotOut)
            {
                if (a.SlotID == slotId)
                {
                    return a.LastDeviceData;
                }
            }
            return null;
        }


        /// <summary>
        /// GetSlotOutInstanceID
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public int GetSlotOutInstanceID(int slotId)
        {
            foreach (SlotOut a in listOfSlotOut)
            {
                if (a.SlotID == slotId)
                {
                    return a.GetInstanceID();
                }
            }
            return -1;
        }

        /// <summary>
        /// GetSlotOutInstanceID
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public int GetSlotInInstanceID(int slotId)
        {
            foreach (SlotIn a in listOfSlotIn)
            {
                if (a.SlotID == slotId)
                {
                    return a.GetInstanceID();
                }
            }
            return -1;
        }


        /// <summary>
        /// GetPressureLoss
        /// </summary>
        /// <returns></returns>
        public float GetPressureLoss()
        {
            return pressureLoss;
        }

        /// <summary>
        /// SetPressureLoss
        /// </summary>
        /// <param name="aLoss"></param>
        public void SetPressureLoss(float loss)
        {
            pressureLoss = loss;
        }


        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/

    }
}