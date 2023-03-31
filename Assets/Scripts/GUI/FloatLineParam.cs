/******************************************************************************
 * File        : FloatLineParam.cs
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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace FrostBit
{

    /// <summary>
    /// Float Line
    /// </summary>
    public class FloatLineParam : MonoBehaviour
    {

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// paramName
        /// </summary>
        [SerializeField] private Text paramName;

        /// <summary>
        /// Slider
        /// </summary>
        [SerializeField] private Slider slider;


        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /// <summary>
        /// Destroying the attached Behaviour will result in the
        /// game or Scene receiving OnDestroy.
        /// </summary>
        public void OnDestroy()
        {
            slider.onValueChanged.RemoveAllListeners();
        }

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /// <summary>
        /// SetFloatline
        /// </summary>
        /// <param name="aParamName"></param>
        /// <param name="aMin"></param>
        /// <param name="aMax"></param>
        /// <param name="aValue"></param>
        /// <param name="aTarget"></param>
        public void SetFloatline(string aParamName, float aMin, float aMax, float aValue, UnityAction<float> aTarget)
        {
            paramName.text = aParamName;
            slider.minValue = aMin;
            slider.maxValue = aMax;
            slider.value = aValue;
            slider.onValueChanged.AddListener(delegate { ValueChangeCheck(aTarget); });

        }

        /// <summary>
        /// ValueChangeCheck
        /// </summary>
        /// <param name="aTarget"></param>
        public void ValueChangeCheck(UnityAction<float> aTarget)
        {

            float value = slider.value;
            aTarget(value);

        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}