/******************************************************************************
 * File        : Movable.cs
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


    /// <summary>
    /// Rotate the specified Transform along specified local or global axes.
    /// </summary>
    public class Rotatable : Interactable
    {
        
        #region Variables

        /// <summary><typeparamref name="UnityEvent"/> which passes the current rotation of <paramref name="InteractableTransform"/> as a parameter.</summary>
        [SerializeField] protected QuaternionEvent OnRotate;

        protected Vector3 baseOffset;

        /// <summary>Current world coordinate position offset to <paramref name="InteractableTransform"/>'s origin point.</summary>
        protected Vector3 currentOffset;

        /// <summary>New world coordinate position offset to <paramref name="InteractableTransform"/>'s origin point.</summary>
        protected Vector3 newOffset;

        /// <summary>Recorded EulerAngles at the start of the process or action.</summary>
        protected Vector3 savedEulerAngles;
        
        float distanceMultiplierForDesktop;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        protected override void Awake()
        {
            base.Awake();
            lockInputWhenInteracting = true;
        }

        #endregion
        #region Override Methods

        /// <summary>Add event listeners for <typeparamref name="Input"/> <typeparamref name="Actions"/.></summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected override void AddInputListeners (InputContext _context)
        {
            if (_context.IsDesktop) {

                _context.DesktopInput.OnLMBDown += BeginInteraction;
                _context.DesktopInput.OnLMBUp += EndInteraction;

            } else if (_context.IsVR) {

                if (vrInteractionButton == VRInteractionButton.Grip)            { _context.VRInput.OnGripDown += BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Grip)            { _context.VRInput.OnGripUp += EndInteraction; }
                if (vrInteractionButton == VRInteractionButton.Trigger)         { _context.VRInput.OnTriggerDown += BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Trigger)         { _context.VRInput.OnTriggerUp += EndInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DClick)  { _context.VRInput.OnPrimary2DAxisDown += BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DClick)  { _context.VRInput.OnPrimary2DAxisUp += EndInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DTouch)  { _context.VRInput.OnPrimary2DAxisTouchDown += BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DTouch)  { _context.VRInput.OnPrimary2DAxisTouchUp += EndInteraction; }                             
            }
        }

        /// <summary>Remove event listeners from <typeparamref name="Input"/> <typeparamref name="Actions"/.></summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected override void RemoveInputListeners (InputContext _context)
        {
            if (_context.IsDesktop) {

                _context.DesktopInput.OnLMBDown -= BeginInteraction;
                _context.DesktopInput.OnLMBUp -= EndInteraction;

            } else if (_context.IsVR) {

                if (vrInteractionButton == VRInteractionButton.Grip)            { _context.VRInput.OnGripDown -= BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Grip)            { _context.VRInput.OnGripUp -= EndInteraction; }
                if (vrInteractionButton == VRInteractionButton.Trigger)         { _context.VRInput.OnTriggerDown -= BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Trigger)         { _context.VRInput.OnTriggerUp -= EndInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DClick)  { _context.VRInput.OnPrimary2DAxisDown -= BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DClick)  { _context.VRInput.OnPrimary2DAxisUp -= EndInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DTouch)  { _context.VRInput.OnPrimary2DAxisTouchDown -= BeginInteraction; }
                if (vrInteractionButton == VRInteractionButton.Primary2DTouch)  { _context.VRInput.OnPrimary2DAxisTouchUp -= EndInteraction; }      
            }
        }

        /// <summary>Enable interaction if <typeparamref name="HasInteractionMode"/> returns TRUE.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about the interaction.</param>
        protected override void BeginInteraction (InputContext _context)
        {
            if (_context.input.InteractionLocked)   { return; }
            if (!CheckInput(_context.input))        { return; }

            base.BeginInteraction(_context);

            if (source.IsDesktop)
            {
                baseOffset = currentOffset = newOffset = (desktopInput.MousePointPosition - InteractableTransform.position).normalized;
                distanceMultiplierForDesktop = Vector3.Distance(desktopInput.MousePointPosition, InteractableTransform.position);
            }
            else if (source.IsVR)
            {
                Vector3 offsetPoint = vrInput.InteractionMode == InteractionMode.Overlap
                    ? vrInput.TouchPointPosition
                    : vrInput.SavedRayPoint;
                baseOffset = currentOffset = newOffset = (offsetPoint - InteractableTransform.position);
            }

            savedEulerAngles = InteractableTransform.localEulerAngles;
        }
  
        /// <summary>
        /// Update loop for VR.
        /// </summary>
        /// <returns>TRUE if the loop should be called again during the next frame.</returns>
        protected override bool VRInteractionUpdate ()
        {
            if (!base.VRInteractionUpdate()) { return false; }

            if (CheckVRInput(vrInput)) 
            { 
                RotateObject(activeContext.mode == InteractionMode.Overlap ? vrInput.TouchPointPosition : vrInput.SavedRayPoint);
            } 
            else 
            {
                EndInteraction(new InputContext(source, InteractableTransform, InputContextState.End));
            }
            
            return true;
        }
        
        /// <summary>
        /// Update loop for desktop.
        /// </summary>
        /// <returns>TRUE if the loop should be called again during the next frame.</returns>
        protected override bool DesktopInteractionUpdate ()
        {
            if (!base.DesktopInteractionUpdate()) { return false; }

            if (CheckDesktopInput(desktopInput)) 
            {
                if (!desktopInput.RMBHeld) 
                {       
                    // NOTE! _point of RotateObject(_point) is not used for desktop.
                    RotateObject(desktopInput.MousePointPosition);
                }
            } 
            else 
            {
                EndInteraction(new InputContext(source, InteractableTransform, InputContextState.End));
            }

            return true;
        }

        #endregion
        #region Virtual Methods

        /// <summary>
        /// Rotate <paramref name="InteractableTransform"/>.
        /// </summary>
        /// <param name="_point">World coordinate offset position with which the object is rotated in VR mode.</param>
        protected virtual void RotateObject (Vector3 _point)
        {
            if (source.IsVR)
            {
                //if (Vector3.Distance(currentOffset, _point - InteractableTransform.position) < 0.01f) { return; }

                currentOffset = newOffset;
                newOffset = (_point - InteractableTransform.position).normalized; 
                InteractableTransform.rotation = Quaternion.FromToRotation(currentOffset, newOffset) * InteractableTransform.rotation;            
            }
            else if (source.IsDesktop)
            {
                Transform camTransform = desktopInput.Camera.transform;
                float xAngle = desktopInput.MouseY * Mathf.Deg2Rad / distanceMultiplierForDesktop;
                float yAngle = desktopInput.MouseX * Mathf.Deg2Rad/ distanceMultiplierForDesktop;

                InteractableTransform.Rotate(InteractableTransform.InverseTransformVector(camTransform.right), xAngle);
                InteractableTransform.Rotate(InteractableTransform.InverseTransformVector(camTransform.up), -yAngle);   
            }  

            OnRotate?.Invoke(InteractableTransform.rotation);
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace