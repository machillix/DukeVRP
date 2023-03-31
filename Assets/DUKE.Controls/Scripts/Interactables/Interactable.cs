/******************************************************************************
 * File        : Interactable.cs
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


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace DUKE.Controls {

    /// <summary>
    /// Coordinate axis flags.
    /// </summary>
    [System.Flags]
    public enum AxisFlags {
        X = (1 << 0),
        Y = (1 << 1),
        Z = (1 << 2)
    }

    /// <summary>
    /// Coordinate selection.
    /// </summary>
    public enum WorldOrLocal {
        World,
        Local
    }



    /// <summary>
    /// Base class for all interaction classes for VR.
    /// </summary>
    public class Interactable : MonoBehaviour 
    {
        #region Variables

        /// <summary><typeparamref name="Input"/> instances currently pointing at this instance.</summary>
        [SerializeField] protected List<Input> hoveringInputs;

        /// <summary>TRUE when interaction should be enabled.</summary>
        [SerializeField] protected bool isInteractable = true;
        
        /// <summary>TRUE when interaction is currently happening.</summary>
        protected bool lockInputWhenInteracting = false;

        /// <summary><typeparamref name="Transform"/> which is affected by the interaction. If left as NULL, this <typeparamref name="Transform"/> is used.</summary>
        [SerializeField] protected Transform interactableTransform;
        
        /// <summary><typeparamref name="VRInteractionButton"/> used for this interaction.</summary>
        [SerializeField] protected VRInteractionButton vrInteractionButton = VRInteractionButton.Trigger;
        
        /// <summary><typeparamref name="DesktopInteractionModifier"/> flags used for this interaction.</summary>
        [SerializeField] protected DesktopInteractionModifier desktopInteractionModifierFlags;
        
        /// <summary>Currently selected <typeparamref name="InteractionMode"/> flags. </summary>
        [SerializeField] protected InteractionMode interactionMode = InteractionMode.Raycast;
        
        /// <summary>TRUE if debug visuals should be drawn.</summary>
        [SerializeField] protected bool drawDebug;


        
        /// <summary><typeparamref name="InteractionMode"/> of the <paramref name="source"/>.</summary>
        [SerializeField, HideInInspector] protected InteractionMode sourceInteractionMode;
        
        /// <summary><typeparamref name="Input"/> that is currently bound to this <typeparamref name="Interactable"/>.</summary>
        [SerializeField, HideInInspector] protected Input source;
        
        /// <summary><paramref name="source"/> cast as <typeparamref name="VRInput"/>.</summary>
        [SerializeField, HideInInspector] protected VRInput vrInput;
        
        /// <summary><paramref name="source"/> cast as <typeparamref name="DesktopInput"/>.</summary>
        [SerializeField, HideInInspector] protected DesktopInput desktopInput;
        
        /// <summary>Configuration info for this type of <typeparamref name="Interactable"/>.</summary>
        [SerializeField, HideInInspector] protected string configurationInfo;

        protected InputContext activeContext;

        #endregion


        #region Properties

        /// <summary><typeparamref name="Transform"/> which is affected by the interaction. If left as NULL, this <typeparamref name="Transform"/> is used.</summary>
        protected Transform InteractableTransform 
        { 
            get { return interactableTransform == null ? transform : interactableTransform; } 
        }

        /// <summary>TRUE when interaction should be enabled.</summary>
        public bool IsInteractable 
        { 
            get { return isInteractable; } 
            set { isInteractable = value; } 
        }

        /// <summary>TRUE when this instance is currently interacting with <paramref name="source"/>.</summary>
        public bool IsInteracting 
        {
            get { return null != source && isInteractable; } 
        }

        /// <summary>Configuration info for this type of <typeparamref name="Interactable"/>.</summary>
        public string ConfigInfo 
        { 
            get { return configurationInfo; } 
            set { configurationInfo = value; } 
        }

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        protected virtual void Awake ()
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        protected virtual void OnEnable() 
        { 
            SubscribeToEvents();
        }

        protected virtual void OnDisable ()
        {
            UnsubscribeFromEvents();
            ForceEndInteraction();

            hoveringInputs.Clear();
        }

        protected virtual void Update ()
        {
            if (IsInteracting) 
            {
                if (source.IsDesktop)   { DesktopInteractionUpdate(); }  
                else if (source.IsVR)   { VRInteractionUpdate(); } 
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>Called by an <typeparamref name="Input"/> when pointing at this instance begins.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected virtual void PointingBegan (InputContext _context)
        {
            if (_context.target == InteractableTransform && interactionMode.HasFlag(_context.mode)) 
            {
                if (!hoveringInputs.Contains(_context.input))
                {
                    hoveringInputs.Add(_context.input);
                    AddInputListeners(_context);
                }
            }
        }

        /// <summary>Called by an <typeparamref name="Input"/> when pointing at this instance ends.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected virtual void PointingEnded (InputContext _context)
        {
            if (_context.target == InteractableTransform && hoveringInputs.Contains(_context.input)) 
            {
                hoveringInputs.Remove(_context.input);
                RemoveInputListeners(_context);
            }     
        }

        /// <summary>Add event listeners for <typeparamref name="Input"/> <typeparamref name="Actions"/>.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected virtual void AddInputListeners (InputContext _context) { }
        
        /// <summary>Remove event listeners from <typeparamref name="Input"/> <typeparamref name="Actions"/>.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected virtual void RemoveInputListeners (InputContext _context) { }

        /// <summary>Add listeners to relevant Events.</summary>
        protected virtual void SubscribeToEvents() 
        { 
            Input.OnPointingBegan += PointingBegan;
            Input.OnPointingEnded += PointingEnded;
        }

        /// <summary>Remove previously added listeners.</summary>
        protected virtual void UnsubscribeFromEvents() 
        { 
            Input.OnPointingBegan -= PointingBegan;
            Input.OnPointingEnded -= PointingEnded;
        }



        /// <summary>Returns TRUE when the corret input or input combination is active.</summary>
        /// <param name="_input"><typeparamref name="Input"/> instance to check.</param>
        /// <returns>TRUE if the correct input combination for this instance is entered.</returns>
        protected virtual bool CheckInput (Input _input)
        {         
            if (_input.IsDesktop)   { return CheckDesktopInput((DesktopInput)_input); }
            else if (_input.IsVR)   { return CheckVRInput((VRInput)_input); } 
            else                    { return false; }     
        }

        /// <summary>Returns TRUE when the corret input or input combination is active.</summary>
        /// <param name="_input"><typeparamref name="VRInput"/> instance to check.</param>
        /// <returns>TRUE if the correct input combination for this instance is entered.</returns>
        protected virtual bool CheckVRInput (VRInput _vr) 
        {
            return vrInteractionButton switch {
                VRInteractionButton.Grip => _vr.GripPressed,
                VRInteractionButton.Highlight => _vr.Hit.transform == InteractableTransform,
                VRInteractionButton.Trigger => _vr.TriggerPressed,
                VRInteractionButton.Primary2DClick => _vr.Primary2DAxisPressed,
                VRInteractionButton.Primary2DTouch => _vr.Primary2DAxisTouched,
                _ => false
            };
        }

        /// <summary>Returns TRUE when the corret input or input combination is active.</summary>
        /// <param name="_input"><typeparamref name="DesktopInput"/> instance to check.</param>
        /// <returns>TRUE if the correct input combination for this instance is entered.</returns>
        protected virtual bool CheckDesktopInput (DesktopInput _dt) 
        { 
            if (desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftShift) != _dt.LeftShiftPressed)  { return false; }
            if (desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftControl) != _dt.LeftCtrlPressed) { return false; }
            if (desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftAlt) != _dt.LeftAltPressed)      { return false; }

            return _dt.LMBHeld;
        }



        /// <summary>Enable interaction if <paramref name="HasInteractionMode"/> returns TRUE.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected virtual void BeginInteraction (InputContext _context) 
        {    
            if (!IsInteractable || IsInteracting) { return; }

            source = _context.input;
            sourceInteractionMode = source.InteractionMode;

            if (source.IsDesktop)   { desktopInput = (DesktopInput)source; }
            else if (source.IsVR)   { vrInput = (VRInput)source; }

            StartCoroutine(EndPointingWhenInteractionLockIsReleased(_context));
            source.SetInteractionLock(lockInputWhenInteracting);
            activeContext = _context;
        }

        /// <summary>End interaction with <paramref name="source"/> <typeparamref name="Input"/>.</summary>
        protected virtual void EndInteraction ()
        {           
            if (null != source) 
            {
                if (lockInputWhenInteracting) 
                { 
                    source.SetInteractionLock(false); 
                }
            }
            
            source = null;
        }

        /// <summary>End interaction with <paramref name="source"/> <typeparamref name="Input"/>. Used for <typeparamref name="Actions"/> that provide <typeparamref name="InputContext"/>.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected virtual void EndInteraction (InputContext _context)
        {
            EndInteraction();
        }

        /// <summary>End the interaction immediately.</summary>
        public virtual void ForceEndInteraction ()
        {
            EndInteraction();
        }



        /// <summary>Update loop for VR.</summary>
        /// <returns>TRUE if the loop should be called again during the next frame.</returns>
        protected virtual bool VRInteractionUpdate ()  
        {
            if (!isInteractable || !IsInteracting)  { return false; } 
            else                                    { return true; }
        }
    
        /// <summary>Update loop for desktop.</summary>
        /// <returns>TRUE if the loop should be called again during the next frame.</returns>
        protected virtual bool DesktopInteractionUpdate () 
        {
            if (!isInteractable || !IsInteracting)  { return false; }
            else                                    { return true; }
        }

        #endregion
        #region Coroutines
        
        /// <summary>Wait until interaction starts and make sure pointing starts if needed.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected IEnumerator StartPointingWhenInteractionLockIsReleased (InputContext _context)
        {                
            yield return new WaitUntil(()=>(_context.input.InteractionLocked == false));

            if (_context.input.TargetTransform == InteractableTransform && interactionMode.HasFlag(_context.mode))
            {
                PointingBegan(new InputContext(_context.input, _context.target, InputContextState.Start));
            }
        }

        /// <summary>Wait until interaction ends and make sure pointing ends if needed.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected IEnumerator EndPointingWhenInteractionLockIsReleased (InputContext _context, bool _logInfo = false)
        {                
            yield return new WaitUntil(()=>(_context.input.InteractionLocked == false));

            if (_context.input.TargetTransform != InteractableTransform)
            {
                PointingEnded(new InputContext(_context.input, _context.target, InputContextState.End));
            }
        }
        
        #endregion
        #region Utility Methods

        /// <summary>Get axis multipliers based on AxisFlags.</summary>
        /// <param name="_flags">Selected axes.</param>
        protected Vector3 GetAxisMultiplier (AxisFlags _flags)
        {
            return (int)_flags switch {
                1 =>  new Vector3(1, 0, 0),
                2 =>  new Vector3(0, 1, 0),
                3 =>  new Vector3(1, 1, 0),
                4 =>  new Vector3(0, 0, 1),
                5 =>  new Vector3(1, 0, 1),
                6 =>  new Vector3(0, 1, 1),
                0 =>  new Vector3(0, 0, 0),
                -1 => new Vector3(1, 1, 1),
                _ =>  new Vector3(1, 1, 1)
            };
        }
    
        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace