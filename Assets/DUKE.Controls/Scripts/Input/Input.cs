/******************************************************************************
 * File        : Input.cs
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


    /// <summary>
    /// Interaction mode flags.
    /// </summary>
    [System.Flags]
    public enum InteractionMode {
        Raycast = (1 << 0),
        Overlap = (1 << 1)
    }

    /// <summary>
    /// Progress states of an <typeparamref name="InputContext"/>.
    /// </summary>
    public enum InputContextState 
    {
        Start,
        Ongoing,
        End
    };


    /// <summary>
    /// Contains information about <typeparamref name="Input"/> and its interaction with objects.
    /// </summary>
    public struct InputContext
    {
        public Input input;
        public Transform target;
        public InputContextState state;
        public InteractionMode mode;



        public bool IsVR {
            get {return input.IsVR; } }

        public bool IsDesktop {
            get {return input.IsDesktop; } }

        public VRInput VRInput {
            get { return (VRInput)input; } }

        public DesktopInput DesktopInput {
            get { return (DesktopInput)input; } }



        public InputContext ( Input _source, Transform _target, InputContextState _state)
        {
            input = _source;
            target = _target;
            state = _state;
            mode = InteractionMode.Raycast;
        }

        public InputContext ( Input _source, Transform _target, InputContextState _state, InteractionMode _mode)
        {
            input = _source;
            target = _target;
            state = _state;
            mode = _mode;
        }
    }



    /// <summary>
    /// Base class for VRInput and DesktopInput.
    /// </summary>
    public class Input : MonoBehaviour {

        #region Variables

        /// <summary>
        /// Currently targeted <typeparamref name="Transform"/> (can be NULL).
        /// </summary>
        protected Transform targetTransform;

        /// <summary>
        /// Current locked state.
        /// </summary>
        [SerializeField] protected bool interactionLock = false;

        /// <summary>
        /// Current <typeparamref name="InteractionMode"/>.
        /// </summary>
        [SerializeField] protected InteractionMode currentInteractionMode;

        /// <summary>
        /// Distance of <paramref name="PointerRay"/>.
        /// </summary>
        [SerializeField] protected float rayDistance;

        /// <summary>
        /// Hit information of the most recent raycast.
        /// </summary>
        [SerializeField] protected RaycastHit rHit;

        /// <summary>
        /// <typeparamref name="LayerMask"/> which registers <paramref name="PointerRay"/>.
        /// </summary>
        [SerializeField] protected LayerMask rayHitLayers;
        
        /// <summary>
        /// <typeparamref name="LayerMask"/> which blocks <paramref name="PointerRay"/>.
        /// </summary>
        [SerializeField] protected LayerMask rayBlockLayers;

        protected InputContext currentInputContext;

        #endregion


        #region Properties

        #region General Properties

        /// <summary>
        /// <typeparamref name="Transform"/> of the currently targeted object.
        /// </summary>
        public Transform TargetTransform 
        { 
            get { return targetTransform; } 
            protected set { UpdateTargetTransform(value); } 
        }

        public InteractionMode InteractionMode
        {
            get { return currentInteractionMode; }
            protected set { currentInteractionMode = value; }
        }

        /// <summary>TRUE if this instance is locked.</summary>
        public bool InteractionLocked
        { 
            get { return interactionLock; }
            protected set { interactionLock = value; } 
        }

        /// <summary>TRUE if primary interaction button is being held.</summary>
        public virtual bool PrimaryInteractionHeld 
        { 
            get; 
            protected set; 
        }

        /// <summary>TRUE if this <typeparamref name="Input"/> should send and receive information.</summary>
        public virtual bool InputActive 
        { 
            get; 
            protected set; 
        } = false;

        /// <summary>TRUE if this <typeparamref name="Input"/> is of type <typeparamref name="VRInput"/>.</summary>
        public bool IsVR 
        { 
            get { return GetType() == typeof(VRInput); } 
        }

        /// <summary>TRUE if this <typeparamref name="Input"/> is of type <typeparamref name="DesktopInput"/>.</summary>
        public bool IsDesktop 
        { 
            get { return GetType() == typeof(DesktopInput); } 
        }

        #endregion
        #region Raycast Properties

        /// <summary><typeparamref name="Transform"/> of the object pointed at through <typeparamref name="Raycast"/>.</summary>
        public virtual Transform PointerTarget 
        { 
            get; 
            protected set; 
        }

        /// <summary><typeparamref name="Ray"/> through which <typeparamref name="Interactables"/> are detected.</summary>
        public virtual Ray PointerRay 
        { 
            get; 
            protected set; 
        }

        /// <summary>Current distance of <paramref name="PointerRay"/>.</summary>
        public float RayDistance
        {
            get { return rayDistance; }
        }

        /// <summary><typeparamref name="Hit"/> information of <paramref name="PointerRay"/>.</summary>
        public virtual RaycastHit Hit 
        { 
            get { return rHit; } 
            protected set { rHit = value; } 
        }

        /// <summary>
        /// TRUE when PointerRay is blocked by an object.
        /// </summary>
        public bool RayIsBlocked 
        {
            get;
            protected set; 
        }

        /// <summary>
        /// Hit distance of PointerRay when being blocked.
        /// </summary>
        public float BlockedRayLength 
        {
            get;
            protected set; 
        }

        #endregion

        #endregion


        #region Events

        /// <summary>
        /// Called when pointing at a <typeparamref name="Transform"/> other than <paramref name="targetTransform"/> starts.
        /// </summary>
        public static Action<InputContext> OnPointingBegan;

        /// <summary>
        /// Called when pointing at <paramref name="targetTransform"/> ends.
        /// </summary>
        public static Action<InputContext> OnPointingEnded;

        /// <summary>
        /// Called when pointing at <paramref name="targetTransform"/> ends.
        /// </summary>
        public Action<InputContext> OnInteractionLockActivated;

        /// <summary>
        /// Called when pointing at <paramref name="targetTransform"/> ends.
        /// </summary>
        public Action<InputContext> OnInteractionLockReleased;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>Set this <typeparamref name="Input"/> inactive.</summary>
        public virtual void DisableInput ()
        {
            //ResetInteractables();

            InputActive = false;
        }

        /// <summary>Set this <typeparamref name="Input"/> active.</summary>
        public virtual void EnableInput () 
        {
            InputActive = true;
        }

        /// <summary>Set <paramref name="InteractionLocked"/> on/off.</summary>
        /// <param name="_lock">TRUE if interaction should be locked.</param>
        public virtual void SetInteractionLock (bool _lock)
        {
            InteractionLocked = _lock;

            if (_lock)  { OnInteractionLockActivated?.Invoke(GetDefaultContext(InputContextState.Start)); }
            else        { OnInteractionLockActivated?.Invoke(GetDefaultContext(InputContextState.End)); }        
        }

        /// <summary>Send <paramref name="OnPointingEnded"/> <typeparamref name="Action"/> manually (used when this instance is disabled etc.).</summary>
        public virtual void ForcePointingEnded ()
        {
            OnPointingEnded?.Invoke(GetDefaultContext(InputContextState.End));
        }

        #endregion      
        #region MonoBehaviour Methods
    
        protected virtual void OnEnable ()
        {
            SubscribeToInput();
            SubscribeToEvents();
        }

        protected virtual void OnDisable ()
        {
            UnsubscribeFromInput();
            UnsubscribeFromEvents();
        }

        #endregion
        #region Event Subscription Methods
        
        protected virtual void SubscribeToInput () { }
        protected virtual void UnsubscribeFromInput() { }

        protected virtual void SubscribeToEvents () { }
        protected virtual void UnsubscribeFromEvents () { }

        #endregion
        #region Raycast Methods

        /// <summary>Create a default <typeparamref name="InputContext"/> since most of them differ only by <typeparamref name="InputContextState"/>.</summary>
        /// <param name="_state"><typeparamref name="InputContextState"/> of this instance (usually <paramref name="Start"/> or <paramref name="End"/>).</param>
        /// <returns><typeparamref name="InputContext"/> instance with default values.</returns>
        protected virtual InputContext GetDefaultContext (InputContextState _state)
        {
            return new InputContext(this, targetTransform, _state, currentInteractionMode);
        }

        /// <summary>Find the highest priority transform that is currently being pointed at.</summary>
        /// <param name="_newTransform"></param>
        /// <returns><typeparamref name="Transform"/> of the highest priority object.</returns>
        protected virtual void UpdateTargetTransform (Transform _newTransform) 
        {     
            if (_newTransform != targetTransform || currentInputContext.mode != currentInteractionMode) 
            {
                OnPointingEnded?.Invoke(GetDefaultContext(InputContextState.End));

                targetTransform = _newTransform;
                currentInputContext = GetDefaultContext(InputContextState.Start);

                OnPointingBegan?.Invoke(currentInputContext);
            }
        }

        /// <summary>Cast a <paramref name="PointerRay"/> and record the resulting information to <paramref name="Hit"/>.</summary>
        protected virtual void PointerRaycast ()
        {
            float dist = Mathf.Infinity;

            if (Physics.Raycast(PointerRay, out RaycastHit tempHit, rayDistance, rayBlockLayers)) 
            {
                dist = tempHit.distance;
                BlockedRayLength = tempHit.distance;
                RayIsBlocked = true;
            } 
            else 
            {
                BlockedRayLength = rayDistance;
                RayIsBlocked = false;
            }
            
            if (!Physics.Raycast(PointerRay, out rHit, rayDistance, rayHitLayers) || rHit.distance > dist) 
            {
                rHit = new RaycastHit();   
            }
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace