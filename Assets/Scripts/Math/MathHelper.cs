/******************************************************************************
 * File        : MathHelper.cs
 * Version     : 0.9 Aplha
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
    /// Math Helper Class
    /// </summary>
    public static class MathHelper
    {

        /// <summary>
        /// Flip Curve x and y Axis
        /// </summary>
        /// <param name="normalCurve"></param>
        /// <returns></returns>
        public static AnimationCurve FlipCurve(AnimationCurve normalCurve)
        {
            AnimationCurve invertedCurve = new AnimationCurve();

            float totalTime = normalCurve.keys[normalCurve.length - 1].time;
            float sampleX = 0;
            float deltaX = 1f;
            float lastY = normalCurve.Evaluate(sampleX);
            while (sampleX < totalTime)
            {
                float y = normalCurve.Evaluate(sampleX);
                float deltaY = y - lastY;
                float tangent = deltaX / deltaY;
                Keyframe invertedKey = new Keyframe(y, sampleX, tangent, tangent);
                invertedCurve.AddKey(invertedKey);

                sampleX += deltaX;
                lastY = y;
            }

            for (int i = 0; i < invertedCurve.length; i++)
            {
                invertedCurve.SmoothTangents(i, 0.1f);
            }
            return invertedCurve;
        }

        /// <summary>
        /// Adds curves
        /// </summary>
        /// <param name="firstCurve"></param>
        /// <param name="secondCurve"></param>
        /// <returns></returns>
        public static AnimationCurve SumCurve(AnimationCurve firstCurve, AnimationCurve secondCurve)
        {
            AnimationCurve sumCurve = new AnimationCurve();

            float totalTime1 = firstCurve.keys[firstCurve.length - 1].time;
            float totalTime2 = secondCurve.keys[secondCurve.length - 1].time;

            float totalTime = totalTime1;
            if (totalTime1 < totalTime2)
            {
                totalTime = totalTime2;
            }
            float sampleX = 0;
            float deltaX = 1f;
            while (sampleX < totalTime)
            {
                float y1 = 0;
                if (sampleX < totalTime1)
                    y1 = firstCurve.Evaluate(sampleX);
                float y2 = 0;
                if (sampleX < totalTime2)
                    y2 = secondCurve.Evaluate(sampleX);

                Keyframe invertedKey = new Keyframe(sampleX, y1 + y2);
                sumCurve.AddKey(invertedKey);
                sampleX += deltaX;
            }

            for (int i = 0; i < sumCurve.length; i++)
            {
                sumCurve.SmoothTangents(i, 0.1f);
            }
            return sumCurve;
        }

        /// <summary>
        /// Adds curves
        /// </summary>
        /// <param name="firstCurve"></param>
        /// <param name="secondCurve"></param>
        /// <param name="firstPressureLost"></param>
        /// <param name="secondPressureLost"></param>
        /// <returns></returns>
        public static AnimationCurve SumCurve(AnimationCurve firstCurve, AnimationCurve secondCurve, float firstPressureLost = 0f, float secondPressureLost = 0f)
        {
            AnimationCurve sumCurve = new AnimationCurve();

            float totalTime1 = firstCurve.keys[firstCurve.length - 1].time;
            float totalTime2 = secondCurve.keys[secondCurve.length - 1].time;

            float totalTime = totalTime1;
            if (totalTime1 < totalTime2)
            {
                totalTime = totalTime2;
            }

            float sampleX = 0;
            float deltaX = 1f;
            while (sampleX < totalTime)
            {
                float y1 = 0;
                if (sampleX < totalTime1)
                    y1 = firstCurve.Evaluate(sampleX) - firstPressureLost;

                if (y1 < 0)
                    y1 = 0;
                float y2 = 0;
                if (sampleX < totalTime2)
                    y2 = secondCurve.Evaluate(sampleX) - secondPressureLost;
                if (y2 < 0)
                    y2 = 0;
                Keyframe invertedKey = new Keyframe(sampleX, y1 + y2);
                sumCurve.AddKey(invertedKey);
                sampleX += deltaX;
            }

            for (int i = 0; i < sumCurve.length; i++)
            {
                sumCurve.SmoothTangents(i, 0.1f);
            }
            return sumCurve;
        }

    }

}