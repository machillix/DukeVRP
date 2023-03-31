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


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;


namespace DUKE.Controls {


    /// <summary>
    /// Move the specified Transform along specified local or global axes.
    /// </summary>
    public class Movable : Interactable 
    {
        #region Variables

        #region General Variables


        /// <summary>Currently selected axes on which movement is allowed.</summary>
        [SerializeField, Space(10)] protected AxisFlags movementAxes;

        /// <summary>Coordinate system of <paramref name="movementAxes"/>.</summary>
        [SerializeField] protected WorldOrLocal coordinateSystem;

        /// <summary>Called when the position is updated.</summary>
        [SerializeField] protected Vector3Event OnPositionChange;

        /// <summary>Called when the relative position within movement limits changes.</summary>
        [SerializeField] protected Vector3Event OnRatioChange;
        Vector3 initialOffsetPoint;

        #endregion
        #region Movement Limit Variables

        /// <summary>TRUE when movement along the permitted axes should be limited in distance.</summary>
        [SerializeField, Space(10)] protected bool limitMovementDistance = false;

        /// <summary>Current total movement allowed in the positive direction on each axis.</summary>
        [SerializeField] protected Vector3 allowedMovToNegDir;

        /// <summary>Current total movement allowed in the negative direction on each axis.</summary>
        [SerializeField] protected Vector3 allowedMovToPosDir;

        #endregion
        #region VRInput Variables

        [SerializeField, HideInInspector] protected Vector3 pointerPos;
        [SerializeField, HideInInspector] protected Vector3 prevPos;
        [SerializeField, HideInInspector] protected float rayLength;

        #endregion
        #region Desktop Variables

        [SerializeField, HideInInspector] protected Vector3 clickPoint;
        [SerializeField, HideInInspector] protected Vector3 transformLocalClickPoint;
        [SerializeField, HideInInspector] protected Vector3 localCenterOffsetFromCamera;
        [SerializeField, HideInInspector] protected Ray prevPointerRay;
        [SerializeField, HideInInspector] protected float distanceToClickPoint;

        #endregion
        


        #endregion


        #region Properties

        /// <summary>TRUE when <paramref name="axisOrientation"/> is set to <typeparamref name="WorldOrLocal"/>.<typeparamref name="Local"/>.</summary>
        public bool UsingLocalCoordinates { 
            get { return coordinateSystem == WorldOrLocal.Local; } }

        /// <summary>Per-axis multiplier of 0 or 1, used for locking axii.</summary>
        public Vector3 AxisMultiplier { 
            get { return GetAxisMultiplier(movementAxes); } }

        /// <summary>TRUE when allowed movement should have limits.</summary>
        public bool LimitMovement {
             get { return limitMovementDistance; } }

        /// <summary>Inverted negative movement direction limit (multiplied by -1 to support adding directly in calculations).</summary>
        protected Vector3 FlippedNegDirMov
        {
            get { return allowedMovToNegDir * -1f; }
        }

        #endregion


        #region Events

        /// <summary>Called when the position is updated.</summary>
        public Action<Vector3> PositionChanged;
        /// <summary>Called when the relative position within movement limits changes.</summary>
        public Action<Vector3> RatioChanged;

        #endregion


        #region Methods

        #region MonoBehaviour Methods


        protected override void Awake()
        {
            base.Awake();
            lockInputWhenInteracting = true;
        }

        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!drawDebug) { return; }

            Vector3 xDir = (coordinateSystem == WorldOrLocal.World ? Vector3.right : InteractableTransform.right.normalized);
            Vector3 yDir = (coordinateSystem == WorldOrLocal.World ? Vector3.up : InteractableTransform.up.normalized);
            Vector3 zDir = (coordinateSystem == WorldOrLocal.World ? Vector3.forward : InteractableTransform.forward.normalized);
            
            if (limitMovementDistance)
            {
                Vector3 multiplier = (coordinateSystem == WorldOrLocal.World ? Vector3.one : InteractableTransform.lossyScale);
                float radius = 0.015f;

                if (movementAxes.HasFlag(AxisFlags.X))
                {
                    Vector3 xMin = InteractableTransform.position + xDir * -allowedMovToNegDir.x;
                    Vector3 xMax = InteractableTransform.position + xDir * allowedMovToPosDir.x;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(xMin, xMax);
                    Gizmos.DrawSphere(xMin, radius);
                    Gizmos.DrawSphere(xMax, radius);
                }

                if (movementAxes.HasFlag(AxisFlags.Y))
                {     
                    Vector3 yMin = InteractableTransform.position + yDir * -allowedMovToNegDir.y;
                    Vector3 yMax = InteractableTransform.position + yDir * allowedMovToPosDir.y;

                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(yMin, yMax);
                    Gizmos.DrawSphere(yMin, radius);
                    Gizmos.DrawSphere(yMax, radius);
                }

                if (movementAxes.HasFlag(AxisFlags.Z))
                {             
                    Vector3 zMin = InteractableTransform.position + zDir * -allowedMovToNegDir.z;
                    Vector3 zMax = InteractableTransform.position + zDir * allowedMovToPosDir.z;

                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(zMin, zMax);
                    Gizmos.DrawSphere(zMin, radius);
                    Gizmos.DrawSphere(zMax, radius);
                }
            }
            else 
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(InteractableTransform.position + xDir * -100f, InteractableTransform.position + xDir * 100f);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(InteractableTransform.position + yDir * -100f, InteractableTransform.position + yDir * 100f);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(InteractableTransform.position + zDir * -100f, InteractableTransform.position + zDir * 100f);

            }
        }
        #endif

        #endregion
        #region Override Methods

        /// <summary>Add event listeners for <typeparamref name="Input"/> <typeparamref name="Actions"/>.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected override void AddInputListeners (InputContext _context)
        {
            if (_context.IsDesktop) 
            {
                _context.DesktopInput.OnLMBDown += BeginInteraction;
                _context.DesktopInput.OnLMBUp += EndInteraction;
            } 
            else if (_context.IsVR) 
            {
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

        /// <summary>Remove event listeners from <typeparamref name="Input"/> <typeparamref name="Actions"/>.</summary>
        /// <param name="_context"><typeparamref name="InputContext"/> containing information about <typeparamref name="Input"/>.</param>
        protected override void RemoveInputListeners (InputContext _context)
        {
            if (_context.IsDesktop) 
            {
                _context.DesktopInput.OnLMBDown -= BeginInteraction;
                _context.DesktopInput.OnLMBUp -= EndInteraction;
            } 
            else if (_context.IsVR) 
            {
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
        /// <param name="_source"></param>
        protected override void BeginInteraction (InputContext _context)
        {
            if (_context.input.InteractionLocked) { return; }
            
            if (!CheckInput(_context.input)) { return; }

            base.BeginInteraction(_context);

            pointerPos = prevPos = source.Hit.point;
            rayLength = source.Hit.distance;
            initialOffsetPoint = source.Hit.point - InteractableTransform.position;

            if (source.IsDesktop)
            {
                prevPos = desktopInput.MousePointPosition;
                clickPoint = desktopInput.MousePointPosition;
                transformLocalClickPoint = transform.InverseTransformPoint(clickPoint);
                localCenterOffsetFromCamera = desktopInput.Camera.transform.InverseTransformPoint(InteractableTransform.position);
            }
            else if (source.IsVR)
            {
                sourceInteractionMode = source.InteractionMode;

                if (interactionMode.HasFlag(InteractionMode.Overlap)  && vrInput.InteractionMode == InteractionMode.Overlap)
                {
                    pointerPos = prevPos = vrInput.TouchPointPosition;
                }
                else if (interactionMode.HasFlag(InteractionMode.Raycast) && vrInput.InteractionMode == InteractionMode.Raycast)
                {
                    rayLength = vrInput.Hit.distance;
                    pointerPos = prevPos = vrInput.transform.position + rayLength * vrInput.PointerRay.direction;
                }
            }
        }
   
        /// <summary>Update loop for VR.</summary>
        /// <returns>TRUE if the loop should be called again during the next frame.</returns>
        protected override bool VRInteractionUpdate ()
        {
            if (!base.VRInteractionUpdate()) { return false; }

            if (CheckVRInput(vrInput)) 
            {
                if (sourceInteractionMode == InteractionMode.Raycast) 
                {
                    pointerPos = vrInput.transform.position + rayLength * vrInput.PointerRay.direction;
                    MoveObjectWithDelta(pointerPos - (InteractableTransform.position + initialOffsetPoint));
                    prevPos = pointerPos;
                } 
                else if (sourceInteractionMode == InteractionMode.Overlap) 
                {
                    pointerPos = vrInput.TouchPointPosition;
                    MoveObjectWithDelta(pointerPos - (InteractableTransform.position + initialOffsetPoint));
                    prevPos = pointerPos;
                }
            } 
            else 
            {
                EndInteraction(new InputContext(source, InteractableTransform, InputContextState.End));
            }

            return true;
        }
        
        /// <summary>Update loop for desktop.</summary>
        /// <returns>TRUE if the loop should be called again during the next frame.</returns>
        protected override bool DesktopInteractionUpdate ()
        {
            if (!base.DesktopInteractionUpdate()) { return false; }

            if (CheckDesktopInput(desktopInput)) 
            {
                if (desktopInput.RMBHeld) 
                {
                    if (!limitMovementDistance) 
                    { 
                        InteractableTransform.position = desktopInput.Camera.transform.TransformPoint(localCenterOffsetFromCamera); 
                    }
                } 
                else 
                {
                    pointerPos = desktopInput.MousePointPosition;
                    MoveObjectWithDelta(pointerPos - (InteractableTransform.position + initialOffsetPoint));
                    prevPos = pointerPos;
                }
            } 
            else 
            {
                EndInteraction(new InputContext(source, InteractableTransform, InputContextState.End));
            }

            return true;
        }

        #endregion
        #region Movement Methods

        /// <summary>Move the object within setting limits.</summary>
        protected virtual void MoveObjectWithDelta (Vector3 _rawDelta)
        {
            Vector3 trueDelta = _rawDelta;

            if (UsingLocalCoordinates) 
            {
                trueDelta =
                    InteractableTransform.right * AxisMultiplier.x * InteractableTransform.InverseTransformDirection(_rawDelta).x +
                    InteractableTransform.up * AxisMultiplier.y * InteractableTransform.InverseTransformDirection(_rawDelta).y +
                    InteractableTransform.forward * AxisMultiplier.z * InteractableTransform.InverseTransformDirection(_rawDelta).z;
            } 
            else 
            {
                trueDelta = new Vector3(
                    _rawDelta.x * AxisMultiplier.x,
                    _rawDelta.y * AxisMultiplier.y,
                    _rawDelta.z * AxisMultiplier.z
                );
            }
          
            
            if (limitMovementDistance)
            {
                Vector3 deltaVector = (UsingLocalCoordinates)
                    ? InteractableTransform.InverseTransformDirection(trueDelta)
                    : trueDelta;

                Vector3 lossyScale = UsingLocalCoordinates ? InteractableTransform.lossyScale : Vector3.one;

                float limitedX = deltaVector.x;
                float limitedY = deltaVector.y;
                float limitedZ = deltaVector.z;

                if (movementAxes.HasFlag(AxisFlags.X))
                {
                    limitedX = (deltaVector.x < 0 
                        ? Mathf.Max(deltaVector.x, FlippedNegDirMov.x) 
                        : Mathf.Min(deltaVector.x, allowedMovToPosDir.x));                         
                }

                if (movementAxes.HasFlag(AxisFlags.Y))
                {
                    limitedY = (deltaVector.y < 0 
                        ? Mathf.Max(deltaVector.y, FlippedNegDirMov.y) 
                        : Mathf.Min(deltaVector.y, allowedMovToPosDir.y)); 
                }

                if (movementAxes.HasFlag(AxisFlags.Z))
                {
                    limitedZ = (deltaVector.z < 0 
                        ? Mathf.Max(deltaVector.z, FlippedNegDirMov.z) 
                        : Mathf.Min(deltaVector.z, allowedMovToPosDir.z));
                }

                Vector3 limitedVector = new Vector3(limitedX, limitedY, limitedZ);

                allowedMovToNegDir += limitedVector;
                allowedMovToPosDir -= limitedVector;

                trueDelta = (UsingLocalCoordinates)
                    ? InteractableTransform.TransformDirection(limitedVector)
                    : limitedVector;
            }
        
            InteractableTransform.position += trueDelta;

            if (limitMovementDistance)
            {
                if (trueDelta != Vector3.zero)
                {
                    PositionChanged?.Invoke(InteractableTransform.position);
                    
                    Vector3 totalMovementVector = allowedMovToNegDir + allowedMovToPosDir;
                    Vector3 ratios = new Vector3(
                        totalMovementVector.x == 0 ? 0 : allowedMovToNegDir.x / totalMovementVector.x,
                        totalMovementVector.y == 0 ? 0 : allowedMovToNegDir.y / totalMovementVector.y,
                        totalMovementVector.z == 0 ? 0 : allowedMovToNegDir.z / totalMovementVector.z
                    );
                    
                    OnRatioChange?.Invoke(ratios);
                    RatioChanged?.Invoke(ratios);
                }
            }
            else
            {
                OnPositionChange?.Invoke(InteractableTransform.position);
                PositionChanged?.Invoke(InteractableTransform.position);
            }
        }

        #endregion
     
        #endregion


    } /// End of Class


} /// End of Namespace