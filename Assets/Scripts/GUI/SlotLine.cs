/******************************************************************************
 * File        : SlotLine.cs
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
using UnityEngine;
using UnityEngine.UI;


namespace FrostBit
{
    /// <summary>
    /// SlotLine
    /// </summary>
    public class SlotLine : MonoBehaviour
    {
        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/
        /// <summary>
        /// descText
        /// </summary>
        [SerializeField]
        private Text descText;

        /// <summary>
        /// dataText
        /// </summary>
        [SerializeField]
        private Text dataText;

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
        /// SetSlotlineInfo()
        /// </summary>
        /// <param name="aDescText"></param>
        /// <param name="aDataText"></param>
        public void SetSlotlineInfo(string aDescText = "", string aDataText = "")
        {
            descText.text = aDescText;
            dataText.text = aDataText;
        }

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}