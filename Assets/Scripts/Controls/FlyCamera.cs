/******************************************************************************
 * File        : FlyCamera.cs
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


/// <summary>
/// FlyCamera
/// </summary>
public class FlyCamera : MonoBehaviour {

    /***************************************************************************
     *                             MEMBERS
     **************************************************************************/

    /// <summary>
    /// sensitivity
    /// </summary>
    [SerializeField] private float sensitivity = 90;

    /// <summary>
    /// ySpeed
    /// </summary>
    [SerializeField] private float ySpeed = 6;

    /// <summary>
    /// normalSpeed
    /// </summary>
    [SerializeField] private float normalSpeed = 10;

    /// <summary>
    /// slowFactor
    /// </summary>
    [SerializeField] private float slowFactor = 0.25f;

    /// <summary>
    /// fastFactor
    /// </summary>
    [SerializeField] private float fastFactor = 3;

    /// <summary>
    /// rotationX
    /// </summary>
    private float rotationX = 0.0f;

    /// <summary>
    /// rotationY
    /// </summary>
    private float rotationY = 0.0f;


    /***************************************************************************
     *                          FROM BASE CLASS
     **************************************************************************/

    /***************************************************************************
     *                          UNITY MESSAGES
     **************************************************************************/

    /// <summary>
    /// Update()
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {

        if (Input.GetMouseButton(1)) {
            rotationX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);
            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        }
        float speedFactor = 1;


        if (Input.GetKey(KeyCode.LeftShift))
            speedFactor = fastFactor;
        if (Input.GetKey(KeyCode.LeftControl))
            speedFactor = slowFactor;

    
  
        transform.position += transform.forward * (normalSpeed * speedFactor) * Input.GetAxis("Verticala") * Time.deltaTime;
        transform.position += transform.right * (normalSpeed * speedFactor) * Input.GetAxis("Horizontala") * Time.deltaTime;
      
        if (Input.GetKey(KeyCode.Q))
            transform.position += transform.up * ySpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            transform.position -= transform.up * ySpeed * Time.deltaTime;

    }

    /***************************************************************************
     *                           PUBLIC METHODS
     **************************************************************************/

    /***************************************************************************
     *                           PRIVATE METHODS
     **************************************************************************/

}