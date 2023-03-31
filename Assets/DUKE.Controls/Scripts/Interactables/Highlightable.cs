/******************************************************************************
 * File        : Highlightable.cs
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
using UnityEngine.UI;
using UnityEngine.Events;


namespace DUKE.Controls {


    /// <summary>
    /// Receive information about being pointed.
    /// </summary>
    public class Highlightable : Interactable 
    {
        #region Variables

        [SerializeField] UnityEvent OnHighlightBegin;
        [SerializeField] UnityEvent OnHighlightEnd;
        bool highlightOn = false;

        #endregion


        #region Properties
        
        /// <summary>TRUE when the object has <typeparamref name="Image"/> component attached.</summary>
        public bool IsUI { 
            get { return GetComponent<Image>() != null; } }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>Change the object's scale.</summary>
        /// <param name="_scale">New scale.</param>
        public void SetLocalScale (Vector3 _scale)
        {
            InteractableTransform.localScale = _scale;
        }

        #endregion

        #region Override Methods

        /// <summary>Called by an <typeparamref name="Input"/> when pointing at this instance begins.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected override void PointingBegan(InputContext _context)
        {
            if (_context.input.InteractionLocked)
            {
                StartCoroutine(StartPointingWhenInteractionLockIsReleased(_context));
            }
            else
            {
                if (_context.target == InteractableTransform && !_context.input.InteractionLocked && interactionMode.HasFlag(_context.mode) && !highlightOn) {

                    SetHighlight(true);         
                }
                
                base.PointingBegan(_context);
            }
        }

        /// <summary>Called by an <typeparamref name="Input"/> when pointing at this instance ends.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected override void PointingEnded(InputContext _context)
        {
            if (_context.input.InteractionLocked)
            {
                StartCoroutine(EndPointingWhenInteractionLockIsReleased(_context));
            }
            else
            {
                base.PointingEnded(_context);

                if (_context.target == InteractableTransform && !_context.input.InteractionLocked && hoveringInputs.Count == 0 && highlightOn)
                {             
                    SetHighlight(false); 
                }
            }
        }

        #endregion
        #region Protected Virtual Methods


        #endregion
        #region Highlight Methods

        /// <summary>Set highlight on / off.</summary>
        /// <param name="_on">Whether the higlight should be toggled on or off.</param>
        void SetHighlight (bool _on)
        {
            if (_on)    { OnHighlightBegin?.Invoke(); }
            else        { OnHighlightEnd?.Invoke(); }

            highlightOn = _on;
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace