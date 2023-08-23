/******************************************************************************
 * File        : Simulation.cs
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
    /// The Simulation class is responsible for the operation of the simulation.
    /// The instance starts the simulation rounds according to the selected interval
    /// </summary>
    public class Simulation : Singleton<Simulation>
    {

        /***************************************************************************
         *                             PROPERTIES
         **************************************************************************/

        /***************************************************************************
         *                             MEMBERS
         **************************************************************************/

        /// <summary>
        /// RunSimulation Delegate
        /// </summary>
        public delegate void RunSimulation();

        /// <summary>
        /// OnRunSimulation event
        /// </summary>
        public static event RunSimulation OnRunSimulation;

        /// <summary>
        /// PreSimulation Delegate
        /// </summary>
        public delegate void PreSimulation();

        /// <summary>
        /// PreSimulation event
        /// </summary>
        public static event PreSimulation OnPreSimulation;

        /// <summary>
        /// THIS IS ONLY FOR DEV USAGE
        /// </summary>
        [SerializeField] private bool systemSpeedTest = false;

        /// <summary>
        /// THIS IS ONLY FOR DEV USAGE
        /// </summary>
        [SerializeField] private int testRuns = 1000;
        /***************************************************************************
         *                          FROM BASE CLASS
         **************************************************************************/

        /***************************************************************************
         *                          UNITY MESSAGES
         **************************************************************************/

        /// <summary>
        /// Start()
        /// Start is called on the frame when a script is enabled just before any
        /// of the Update methods are called the first time.
        /// 
        /// Run simulation calls according to the selected interval
        /// </summary>
        private void Start()
        {
            if (null != OnPreSimulation)
            {
                OnPreSimulation();
            }
            /*
            if (!systemSpeedTest)
            {
                InvokeRepeating("DoSimulation", 0, SimulationSettings.Instance.SimulationInterval);
            }
            if (systemSpeedTest)
            {
                Debug.LogError("Waiting Test....");
                Invoke("DoSpeedTest", 2);
            }*/
        }

        /***************************************************************************
         *                           PUBLIC METHODS
         **************************************************************************/

        /***************************************************************************
         *                           PRIVATE METHODS
         **************************************************************************/

        /// <summary>
        /// DoSimulation
        ///
        /// Triggers the OnRunSimulation event according to the selected interval
        /// </summary>
        private void Update()
        {
            if (null != OnRunSimulation)
            {
                OnRunSimulation();
            }
        }

        private void DoSimulation()
        {
            if (null != OnRunSimulation)
            {
                OnRunSimulation();
            }
        
        }

        /// <summary>
        /// DoSpeedTest
        ///
        /// 
        /// </summary>
        private void DoSpeedTest()
        {
            float curTime = Time.realtimeSinceStartup;

            for (int a = 0; a < testRuns; a++)
            {
                if (null != OnRunSimulation)
                {
                    OnRunSimulation();

                }
            }
            Debug.LogError("Test Ready, Run Time for " + testRuns.ToString() + " Simulations = " + (Time.realtimeSinceStartup - curTime));
        }

    }

}