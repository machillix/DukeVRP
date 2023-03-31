/******************************************************************************
 * File        : Valve.cs
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
using UnityEngine.Events;


namespace DUKE.Controls
{


    public class Valve : Rotatable
    {
        #region Variables

        [SerializeField, Delayed] float minAngle = 0;
        [SerializeField, Delayed] float maxAngle = 360;
        [SerializeField, Range(0f, 1f)] float ratio = 0;
        [SerializeField] float curAngle;
        [SerializeField] float debugRadius = 0.5f;
        float _curAngle;
        float _ratio;


        [SerializeField] UnityEvent OnMinimumAngle;
        [SerializeField] UnityEvent OnMaximumAngle;
        [SerializeField] FloatEvent OnValueChange;
        [SerializeField] FloatEvent OnRatioChange;


        Vector3 baseUp;
        Vector3 minDirection;
        Vector3 maxDirection;
        float prevTotalAngle;

        #endregion


        #region Properties

        /// <summary>
        /// Minimum angle of this <typeparamref name="Valve"/>.
        /// </summary>
        public float MinimumAngle
        {
            get { return minAngle; }
        }

        /// <summary>
        /// Maximum angle of this <typeparamref name="Valve"/>.
        /// </summary>
        public float MaximumAngle
        {
            get { return maxAngle; }
        }

        /// <summary>
        /// Minimum angle of this <typeparamref name="Valve"/>.
        /// </summary>
        public float CurrentAngle
        {
            get { return curAngle; }
        }

        /// <summary>
        /// Relative value of the angle between minimum (0) and maximum (1) angles.
        /// </summary>
        public float Ratio
        {
            get { return ratio; }
        }

        #endregion


        #region Events

        /// <summary>
        /// Send an Action when minimum angle is reached.
        /// </summary>
        public Action OnStartPointReached;

        /// <summary>
        /// Send an Action when maximum angle is reached.
        /// </summary>
        public Action OnEndPointReached;

        /// <summary>
        /// Send an Action when current angle changes.
        /// </summary>
        public Action<float> OnValueChanged;

        /// <summary>
        /// Send an Action when the relative angle between min and max changes.
        /// </summary>
        public Action<float> OnRatioChanged;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        protected override void Awake()
        {
            base.Awake();

            lockInputWhenInteracting = true;
        }

        void OnValidate()
        {
            minAngle = Mathf.Clamp(minAngle, -Mathf.Infinity, maxAngle);
            maxAngle = Mathf.Clamp(maxAngle, minAngle, Mathf.Infinity);

            if (ratio != _ratio)
            {
                curAngle = Mathf.Clamp(Mathf.Lerp(minAngle, maxAngle, ratio), minAngle, maxAngle);
                ratio = Mathf.Clamp01((curAngle - minAngle) / (maxAngle - minAngle));
            }
            else
            {
                ratio = Mathf.Clamp01((curAngle - minAngle) / (maxAngle - minAngle));
                curAngle = Mathf.Clamp(Mathf.Lerp(minAngle, maxAngle, ratio), minAngle, maxAngle);
            }

            InteractableTransform.localEulerAngles = new Vector3(
                InteractableTransform.localEulerAngles.x,
                InteractableTransform.localEulerAngles.y,
                curAngle
            );

            _ratio = ratio;
            _curAngle = curAngle;

            Vector3 tempX = Vector3.Cross(InteractableTransform.forward, InteractableTransform.forward + Vector3.up * 0.1f);
            baseUp = -Vector3.Cross(InteractableTransform.forward, tempX).normalized;
            minDirection = Quaternion.AngleAxis(MinimumAngle, InteractableTransform.forward) * baseUp;
            maxDirection = Quaternion.AngleAxis(MaximumAngle, InteractableTransform.forward) * baseUp;         
        }

        void OnDrawGizmos()
        {
            if (!drawDebug) { return; }

            List<Vector3> points = new List<Vector3>();
            float step = 1f;
            float loopDepthAdjustment = debugRadius * 0.025f;
            float pointSize = debugRadius * 0.04f;
            float lineLength = debugRadius + (loopDepthAdjustment * (MaximumAngle - MinimumAngle) / 360f) + 0.1f;

            // Arc
            Gizmos.color = Color.yellow;
            points.Add(InteractableTransform.position);

            for (float i = MinimumAngle; i < MaximumAngle; i += step)
            {
                float currentDepthAdjustment = loopDepthAdjustment / 360f * i;
                Vector3 point = InteractableTransform.position + Quaternion.AngleAxis(i, InteractableTransform.forward) * (baseUp * (debugRadius + currentDepthAdjustment));
                
                Gizmos.DrawLine(point, points[points.Count - 1]);
                points.Add(point);
            }

            points.Add(InteractableTransform.position + Quaternion.AngleAxis(MaximumAngle, InteractableTransform.forward) * (baseUp * (debugRadius + loopDepthAdjustment / 360f * MaximumAngle)));
            points.Add(InteractableTransform.position);
            Gizmos.DrawLine(points[points.Count - 3], points[points.Count - 2]);
            Gizmos.DrawLine(points[points.Count - 2], points[points.Count - 1]);

            // Current angle
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(InteractableTransform.position + Quaternion.AngleAxis(CurrentAngle, InteractableTransform.forward) * (baseUp * (debugRadius + loopDepthAdjustment / 360f * CurrentAngle)), pointSize);

            // Min and max angles
            Gizmos.color = Color.green;
            Gizmos.DrawLine(InteractableTransform.position, InteractableTransform.position + minDirection * lineLength);
            Gizmos.DrawLine(InteractableTransform.position, InteractableTransform.position + maxDirection * lineLength);

            // Current input point of player
            if (Application.isPlaying)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(InteractableTransform.position, ConvertVectorToGlobalFlat(InteractableTransform.position + newOffset));
            }
    
            // baseUp
            Gizmos.color = Color.black;
            Gizmos.DrawLine(InteractableTransform.position, InteractableTransform.position + baseUp * lineLength);
        }

        #endregion
        #region Override Methods

        /// <summary>
        /// Enable interaction if <paramref name="HasInteractionMode"/> returns TRUE.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about the interaction.</param>
        protected override void BeginInteraction(InputContext _context)
        {
            base.BeginInteraction(_context);

            baseOffset = (_context.IsDesktop 
                ? desktopInput.MousePointPosition 
                : (vrInput.InteractionMode == InteractionMode.Overlap 
                    ? vrInput.TouchPointPosition 
                    : vrInput.SavedRayPoint));

            minDirection = Quaternion.AngleAxis(MinimumAngle, InteractableTransform.forward) * Vector3.up;
            maxDirection = Quaternion.AngleAxis(MaximumAngle, InteractableTransform.forward) * Vector3.up;
            prevTotalAngle = Vector3.SignedAngle(baseUp, ConvertVectorToGlobalFlat(InteractableTransform.position + currentOffset) - InteractableTransform.position, InteractableTransform.forward);
        }

        /// <summary>
        /// Rotate <paramref name="InteractableTransform"/>.
        /// </summary>
        /// <param name="_newOffset">New world coordinate position with which to calculate delta.</param>
        protected override void RotateObject(Vector3 _newOffset)
        {
            currentOffset = newOffset;
            newOffset = (_newOffset - InteractableTransform.position); 
            
            float newTotalAngle = Vector3.SignedAngle(baseUp, ConvertVectorToGlobalFlat(InteractableTransform.position + newOffset) - InteractableTransform.position, InteractableTransform.forward);
            float angleDelta = newTotalAngle - prevTotalAngle;
            prevTotalAngle = newTotalAngle;

            if (angleDelta < -180f)     { angleDelta += 360f; }
            else if (angleDelta > 180f) { angleDelta -= 360f; }

            float trueDelta = Mathf.Clamp(angleDelta + curAngle, minAngle, maxAngle) - curAngle;
            curAngle += trueDelta;
            ratio = (curAngle - minAngle) / (maxAngle - minAngle);       
            float eulerAngle = Mathf.Lerp(minAngle, maxAngle, ratio);

            if (curAngle == minAngle)
            {
                OnMinimumAngle?.Invoke();
                OnStartPointReached?.Invoke();
            }
            else if (curAngle == maxAngle)
            {
                OnMaximumAngle?.Invoke();
                OnEndPointReached?.Invoke();
            }

            if (trueDelta != 0f)
            {
                OnValueChange?.Invoke(CurrentAngle);
                OnValueChanged?.Invoke(CurrentAngle);
                OnRatioChange?.Invoke(Ratio);
                OnRatioChanged?.Invoke(Ratio);
            }

            InteractableTransform.eulerAngles = new Vector3(InteractableTransform.eulerAngles.x, InteractableTransform.eulerAngles.y, eulerAngle);
        }

        #endregion
        #region Calculation Methods

        /// <summary>
        /// Convert a world position to the object's local coordinate system and constrain it to its local XY-plane.
        /// </summary>
        /// <param name="_worldPosition">World coordinate position to convert.</param>
        /// <returns>Constrained local coordinate position.</returns>
        protected Vector3 ConvertVectorToLocalFlat (Vector3 _worldPosition)
        {
            Vector3 local = InteractableTransform.InverseTransformPoint(_worldPosition);
            return new Vector3(local.x, local.y, 0f);
        }

        /// <summary>
        /// Constrain a world position to the object's local XY-plane.
        /// </summary>
        /// <param name="_worldPosition">World coordinate position to convert.</param>
        /// <returns>Constrained world coordinate position.</returns>
        protected Vector3 ConvertVectorToGlobalFlat (Vector3 _worldPosition)
        {
            return InteractableTransform.TransformPoint(ConvertVectorToLocalFlat(_worldPosition));
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace