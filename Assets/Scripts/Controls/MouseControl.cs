/******************************************************************************
 * File        : MouseControl.cs
 * Version     : 0.9 Alpha
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;


namespace FrostBit
{
    /// <summary>
    /// MouseControl
    /// </summary>
    public class MouseControl : MonoBehaviour
    {


        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        ///camera
        /// </summary>
        private Camera camera;

        /// <summary>
        /// lastSelectObject
        /// </summary>
        private GameObject lastSelectObject;

        /// <summary>
        /// lastClickTime
        /// </summary>
        private float lastClickTime;

        private GenericDevice selectedDevice;

        /// <summary>
        /// doubleClickInterVal
        /// </summary>
        [SerializeField] private float doubleClickInterVal = 0.2f;


        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /// <summary>
        /// Start()
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            camera = GetComponent<Camera>();

            if (null == camera)
            {
                Debug.LogError("MouseControl - Start() -> Cannot find camera (" + gameObject.name + ")");
                this.enabled = false;
            }

            InvokeRepeating("UpdateData", 0, 0.1f);
        }

        private bool IsOverUI()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].gameObject.GetComponent<CanvasRenderer>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Update()
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {

            // Mouse Button Pressed
            if (Input.GetMouseButtonDown(0) && !IsOverUI())
            {


                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {

                    GenericDevice gd = hit.collider.gameObject.GetComponentInParent<GenericDevice>();


                    lastClickTime = Time.time;
                    if (null != gd)
                    {
                        selectedDevice = gd;



                        InfoCanvas.Instance.ClearActions();
                        InfoCanvas.Instance.AddDeviceData(selectedDevice);

                        UpdateData();



                    }
                    lastSelectObject = hit.collider.gameObject;


                }
            }

        }

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/

        /// <summary>
        /// UpdateData
        /// </summary>
        private void UpdateData()
        {

            if (null != selectedDevice)
            {
                InfoCanvas.Instance.ClearInfo();

                GenericDeviceData deviceData = selectedDevice.DeviceData;
                SlotIn[] slotsIn = selectedDevice.GetSlotsIn();
                SlotOut[] slotsOut = selectedDevice.GetSlotsOut();

                InfoCanvas.Instance.AddDeviceInfo(selectedDevice);


                foreach (SlotIn a in slotsIn)
                {
                    var objectType = selectedDevice.DeviceData.GetType();

                    PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
                    foreach (PropertyInfo prop in assa)
                    {
                        InfoCanvas.Instance.SetSlotInfoIN(prop.Name, prop.GetValue(a.LastDeviceData, null).ToString(), a.SlotID);

                    }
                }
                foreach (SlotOut a in slotsOut)
                {
                    var objectType = a.LastDeviceData.GetType();
                    PropertyInfo[] assa = a.LastDeviceData.GetType().GetProperties();
                    foreach (PropertyInfo prop in assa)
                    {
                        InfoCanvas.Instance.SetSlotOutInfo(prop.Name, prop.GetValue(a.LastDeviceData, null).ToString(), a.SlotID);

                    }
                }



            }
        }
    }
}