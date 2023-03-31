/******************************************************************************
 * File        : ScaleOptions.cs
 * Version     : 1.0
 * Author      : Miika Puljujärvi (miika.puljujarvi@lapinamk.fi)
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


namespace DUKE.Controls
{


    public class ScaleOptions : MonoBehaviour
    {
        [SerializeField] Transform scalableObj;
        [SerializeField] AxisFlags scalableAxes;
        [SerializeField] float scaleMultiplier = 1f;
        [SerializeField] float scaleAddition = 0f;
        [SerializeField] float minimumScale = 0f;
        [SerializeField] float maximumScale = 1f;




        public void ChangeScale (float _scale)
        {
            ChangeScale(Vector3.one * _scale);
        }

        public void ChangeScale (Vector3 _scale)
        {
            scalableObj.localScale = new Vector3(
                scalableAxes.HasFlag(AxisFlags.X) ? Mathf.Clamp(_scale.x * scaleMultiplier + scaleAddition, minimumScale, maximumScale) : scalableObj.localScale.x,
                scalableAxes.HasFlag(AxisFlags.Y) ? Mathf.Clamp(_scale.y * scaleMultiplier + scaleAddition, minimumScale, maximumScale) : scalableObj.localScale.y,
                scalableAxes.HasFlag(AxisFlags.Z) ? Mathf.Clamp(_scale.z * scaleMultiplier + scaleAddition, minimumScale, maximumScale) : scalableObj.localScale.z
            );
        }


    }


}