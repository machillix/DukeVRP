/******************************************************************************
 * File        : VRClipBoard.cs
 * Version     : 0.5 Alpha
 * Author      : Jere Jalonen (jere.jalonen@lapinamk.com)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2023 Lapland University of Applied Sciences
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
using DUKE.Controls;
using UnityEngine.InputSystem;

public class HastyFadeScript : MonoBehaviour
{

    public GameObject logos;

    public float fadeSpeed = 2.5f;

    public Renderer fader;

    public Material fademat;

    public float mAlpha = 1;

    public bool start;

    bool begun = false;

    public VRInput[] inputit;

    public GameObject[] hidehands;

    private void Update()
    {
        foreach (VRInput inputti in inputit)
        {
            if (inputti.TriggerPressed)
                start = true;
        }

        if (UnityEngine.Input.GetButtonDown("Fire1"))
            start = true;

    }

    void FixedUpdate()
    {
        var color = fademat.color;

        if (start)
        {
            if (mAlpha <= 0.99f)
                mAlpha += 0.01f * fadeSpeed;
            else
            {
                begun = true;
                Destroy(logos);
            }

            fademat.color = new Color(color.r, color.g, color.b, mAlpha);
        }

        if (begun) {
            start = false;
            if (mAlpha >= 0.01f)
                mAlpha -= 0.01f * fadeSpeed;
            else {
                foreach (GameObject hand in hidehands)
                    hand.SetActive(true);
                
                Destroy(this.gameObject); }
            fademat.color = new Color(color.r, color.g, color.b, mAlpha);
        }

    }

}
