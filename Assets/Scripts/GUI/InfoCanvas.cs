/******************************************************************************
 * File        : InfoCanvas.cs
 * Version     : 0.9
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
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/*
 * NOTE !. THIS IS ONLY FAST PROTOTYPE FOR UI
 * 
 */

namespace FrostBit
{
    /// <summary>
    /// InfoCanvas. NOTE !. THIS IS ONLY FAST PROTOTYPE FOR UI
    /// </summary>
    public class InfoCanvas : MonoBehaviour
    {
        /// <summary>
        /// slotTemplate
        /// </summary>
        [SerializeField] private GameObject slotTemplate;

        /// <summary>
        /// templateParent
        /// </summary>
        [SerializeField] private GameObject templateParent;

        /// <summary>
        /// listOfSlotInTemplates
        /// </summary>
        [SerializeField] private List<SlotTemplate> listOfSlotInTemplates;

        /// <summary>
        /// listOfSlotOutTemplates
        /// </summary>
        [SerializeField] private List<SlotTemplate> listOfSlotOutTemplates;

        /// <summary>
        /// others
        /// </summary>
        [SerializeField] private List<GameObject> others;

        /// <summary>
        /// deviceInfo
        /// </summary>
        [SerializeField] private List<GameObject> deviceInfo;

        /// <summary>
        /// floatLineParam
        /// </summary>
        [SerializeField] private GameObject floatLineParam;

        /// <summary>
        /// instance
        /// </summary>
        private static InfoCanvas instance = null;

        /// <summary>
        /// Instance
        /// </summary>
        public static InfoCanvas Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Awake is called either when an active GameObject that contains
        /// the script is initialized when a Scene loads, or when a
        /// previously inactive GameObject is set to active, or after
        /// a GameObject created with Object.Instantiate is initialized.Use
        /// Awake to initialize variables or states before the application starts.
        /// </summary>
        private void Awake()
        {
            instance = this;

            listOfSlotOutTemplates = new List<SlotTemplate>();

            listOfSlotInTemplates = new List<SlotTemplate>();

            others = new List<GameObject>();
        }

        /// <summary>
        /// SetTitle
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {

        }

        /// <summary>
        /// SetSlotInfoIN
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="data"></param>
        public void SetSlotInfoIN(string desc, string data, int id)
        {
            foreach (SlotTemplate a in listOfSlotInTemplates)
            {
                if (id == a.SlotID)
                {
                    a.AddData(desc, data);

                    return;
                }
            }
            GameObject newObject = GameObject.Instantiate(slotTemplate, templateParent.transform);
            SlotTemplate newTemplate = newObject.GetComponent<SlotTemplate>();
            newTemplate.SlotID = id;
            newTemplate.SetTitle("Slot In " + id);
            newTemplate.AddData(desc, data);
            listOfSlotInTemplates.Add(newTemplate);
        }

        /// <summary>
        /// SetSlotOutInfo()
        /// </summary>
        /// <param name="aDesc"></param>
        /// <param name="aData"></param>
        public void SetSlotOutInfo(string desc, string data, int id)
        {
            foreach (SlotTemplate a in listOfSlotOutTemplates)
            {
                if (id == a.SlotID)
                {
                    a.AddData(desc, data);

                    return;
                }
            }
            GameObject newObject = GameObject.Instantiate(slotTemplate, templateParent.transform);
            SlotTemplate newTemplate = newObject.GetComponent<SlotTemplate>();
            newTemplate.SlotID = id;
            newTemplate.SetTitle("Slot Out " + id);
            newTemplate.AddData(desc, data);
            listOfSlotOutTemplates.Add(newTemplate);
        }


        /// <summary>
        /// AddDeviceInfo
        /// </summary>
        /// <param name="aGenericDevice"></param>
        public void AddDeviceInfo(GenericDevice aGenericDevice)
        {
            GenericDeviceData asa = (GenericDeviceData)aGenericDevice.GetDeviceData();
            PropertyInfo[] assaa = aGenericDevice.GetDeviceData().GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            GameObject newObject = GameObject.Instantiate(slotTemplate, templateParent.transform);

            deviceInfo.Add(newObject);

            SlotTemplate newTemplate = newObject.GetComponent<SlotTemplate>();

            newTemplate.SetTitle("Device Data");


            foreach (PropertyInfo prop in assaa)
            {
                newTemplate.AddData(prop.Name, prop.GetValue(aGenericDevice.GetDeviceData(), null).ToString());
            }
        }

        /// <summary>
        /// AddDeviceData
        /// </summary>
        /// <param name="aGenericDevice"></param>
        public void AddDeviceData(GenericDevice aGenericDevice)
        {
            DeviceParam param = aGenericDevice.GetParams();

            if (DeviceParamType.EFloat == param.paramType)
            {
                GameObject newLine = GameObject.Instantiate(floatLineParam, templateParent.transform);
                others.Add(newLine);


                newLine.GetComponent<FloatLineParam>().SetFloatline(param.name, (float)param.min, (float)param.max, (float)param.value, (UnityAction<float>)param.target);
            }

        }

        /// <summary>
        /// ClearInfo
        /// </summary>
        public void ClearInfo()
        {

            foreach (SlotTemplate a in listOfSlotOutTemplates)
            {
                DestroyImmediate(a.gameObject);
            }

            listOfSlotOutTemplates.Clear();


            foreach (SlotTemplate a in listOfSlotInTemplates)
            {
                DestroyImmediate(a.gameObject);
            }

            listOfSlotInTemplates.Clear();

            foreach (GameObject a in deviceInfo)
            {
                DestroyImmediate(a);
            }

            deviceInfo.Clear();


        }

        /// <summary>
        /// ClearActions
        /// </summary>
        public void ClearActions()
        {
            foreach (GameObject a in others)
            {
                DestroyImmediate(a);
            }
            others.Clear();
        }
    }
}