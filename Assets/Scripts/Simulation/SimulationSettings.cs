/******************************************************************************
 * File        : SimulationSettings.cs
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SimulationSettings
/// This class contains all settings for simulation (system)
/// </summary>
public class SimulationSettings : MonoBehaviour
{
    /***************************************************************************
     *                             PROPERTIES
     **************************************************************************/

    /// <summary>
    ///  default value is 1;
    ///  examples
    ///  Multiplier value   GameTime    RealTime
    ///  1                      1 s         1 s
    ///  2                      1 s         2 s
    ///  3                      1 s         3 s 
    ///  10                     1 s        10 s
    /// </summary>
    [SerializeField] private float timeMultiplier = 1f;

    /// <summary>
    ///  default value is 1;
    ///  examples
    ///  Multiplier value   GameTime    RealTime
    ///  1                      1 s         1 s
    ///  2                      1 s         2 s
    ///  3                      1 s         3 s 
    ///  10                     1 s        10 s
    /// </summary>
    public float TimeMultiplier { get => timeMultiplier; }


    /// <summary>
    /// Simulation interval, Default 0.01f;
    /// </summary>
    [SerializeField]private float simulationInterval = 0.01f;

    /// <summary>
    /// Simulation interval, Default 0.01f;
    /// </summary>
    public float SimulationInterval { get => simulationInterval; }


    /// <summary>
    /// Instance (Singleton)
    /// </summary>
    private static SimulationSettings instance;

    /// <summary>
    /// Instance (Singleton)
    /// </summary>
    public static SimulationSettings Instance { get => instance; }

    /***************************************************************************
     *                             MEMBERS
     **************************************************************************/

    /// <summary>
    /// List of System Circuit
    /// </summary>
    private Hashtable circuits = new Hashtable();

    /// <summary>
    /// List of System Circuit
    /// </summary>
    public Hashtable Circuits { set => circuits = value; get => circuits; }


    /// <summary>
    /// List of System Flows
    /// </summary>
    private Hashtable flows = new Hashtable();

    /// <summary>
    /// List of System Flows
    /// </summary>
    public Hashtable Flows { set => flows = value; get => flows; }

    /// <summary>
    /// List of Simulations
    /// </summary>
    private Hashtable simulations = new Hashtable();


    /// <summary>
    /// List of Simulations
    /// </summary>
    public Hashtable Simulations { set => simulations = value; get => simulations; }

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
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake() 
    {
        if (instance == null) instance = this;
        DontDestroyOnLoad(this);
    }

    /***************************************************************************
     *                           PRIVATE METHODS
     **************************************************************************/
}
