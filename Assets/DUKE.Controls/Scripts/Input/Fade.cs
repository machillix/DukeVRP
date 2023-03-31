/******************************************************************************
 * File        : Fade.cs
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DUKE.Controls {

    public class Fade : MonoBehaviour
    {   
        #region Variables

        [SerializeField] List<MeshRenderer> fadeRenderers;
        [SerializeField] Material fadeMaterial;
        [SerializeField] float fadeValue = 0f;
        [SerializeField] float defaultFadeDuration = 1f;

        #endregion


        #region Properties

        public float FadeValue {
            get { return fadeValue; } }

        #endregion
        
        
        #region Properties

        public static Action FadeStarted;
        public static Action FadeEnded;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Start the fade process.
        /// </summary>
        /// <param name="_value">Target opacity to transition to.</param>
        /// <param name="_duration">Duration of the transition in seconds.</param>
        public void StartFade (float _value, float _duration = -1)
        {
            if (_duration == -1) { _duration = defaultFadeDuration; }

            StartCoroutine(DoFade(_value, _duration));
        }

        #endregion
        #region MonoBehaviour Methods

        void Start ()
        {
            foreach (MeshRenderer rend in fadeRenderers) {

                rend.material = fadeMaterial;
            }

            fadeMaterial.SetFloat("_Fade", fadeValue);
        }

        void Update()
        {
            fadeMaterial.SetFloat("_Fade", fadeValue);
        }

        #endregion
        #region Coroutines

        /// <summary>
        /// Adjust <paramref name="fadeValue"/> periodically.
        /// </summary>
        /// <param name="_targetValue">Target opacity to transition to.</param>
        /// <param name="_duration">Duration of the transition in seconds.</param>
        IEnumerator DoFade (float _targetValue, float _duration)
        {
            FadeStarted?.Invoke();

            int dir = _targetValue > fadeValue ? 1 : -1;
            float min = _targetValue > fadeValue ? fadeValue : _targetValue;
            float max = _targetValue > fadeValue ? _targetValue : fadeValue;

            while (fadeValue != _targetValue) {

                fadeValue += Time.deltaTime / _duration * dir;      
                fadeValue = Mathf.Clamp(fadeValue, min, max);

                yield return new WaitForEndOfFrame();
            }

            FadeEnded?.Invoke();
        }

        #endregion

        #endregion


    }/// End of Class


} /// End of Namespace