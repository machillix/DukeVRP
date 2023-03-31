/******************************************************************************
 * File        : DukeDef.cs
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
    /// SimulationState.
    /// </summary>
    public enum SimulationState
    {
        EPreSimulation = 0,
        EPressureLoss,
        EPumpRound,
        EParallel
    };

    /// <summary>
    /// CircuitData
    /// </summary>
    public class CircuitData
    {
        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/
        /// <summary>
        /// Pressure lost of Circuit
        /// </summary>
        private float pressureLost;

        /// <summary>
        /// Pressure lost of Circuit
        /// </summary>
        public float PressureLost { set => pressureLost = value; get => pressureLost; }

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/
        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/
        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/
        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/
        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }

    /// <summary>
    /// Sub Flow/Circtuit Data
    /// </summary>
    public class SubFlowData
    {
        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /// <summary>
        /// Generic Device
        /// </summary>
        private GenericDevice genericDevice;

        /// <summary>
        /// Generic Device
        /// </summary>
        public GenericDevice GenericDevice { set => genericDevice = value; get => genericDevice; }

        /// <summary>
        /// Sub Flows
        /// </summary>
        private List<int> subFlows = new List<int>();

        /// <summary>
        /// Sub Flows
        /// </summary>
        public List<int> SubFlows { set => subFlows = value; get => subFlows; }

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/
        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/
        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/
        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/
        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }

    /// <summary>
    /// Flow Data
    /// </summary>
    public class FlowData
    {
        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /// <summary>
        /// Flow Serial
        /// </summary>
        private AnimationCurve serialPump;

        /// <summary>
        /// Flow Serial
        /// </summary>
        public AnimationCurve SerialPump { set => serialPump = value; get => serialPump; }

        /// <summary>
        /// Flow Data Computed/Ready
        /// </summary>
        private bool ready = false;

        /// <summary>
        /// Flow Data Computed/Ready
        /// </summary>
        public bool Ready { set => ready = value; get => ready; }

        /// <summary>
        /// Flows
        /// </summary>
        private Hashtable flows = new Hashtable();

        /// <summary>
        /// Flows
        /// </summary>
        public Hashtable Flows { set => flows = value; get => flows; }

        /// <summary>
        /// Flow Identifier
        /// </summary>
        private int flowID;

        /// <summary>
        /// Flow Identifier
        /// </summary>
        public int FlowID { set => flowID = value; get => flowID; }

        /// <summary>
        /// Parent of Flow
        /// </summary>
        private int parent;

        /// <summary>
        /// Parent of Flow
        /// </summary>
        public int Parent { set => parent = value; get => parent; }


        /// <summary>
        /// Flow node
        /// </summary>
        private SubFlowData node;

        /// <summary>
        /// Flow node
        /// </summary>
        public SubFlowData Node { set => node = value; get => node; }
        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/
        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/
    }
}