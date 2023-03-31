/******************************************************************************
 * File        : AreaSelector.cs
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
using UnityEngine.UI;


namespace DUKE.Controls {


    /// <summary>
    /// Select a coordinate (X and Y value) by clicking on an area.
    /// </summary>
    public class AreaSelector : Clickable 
    {
        #region Variables

        #region General Variables

        [Space(20f)]
        [SerializeField] Image foregroundImg;
        [SerializeField] float margin = 0f;
        [SerializeField] float oldMargin;
        [SerializeField] bool displayPointer = false;
        [SerializeField] bool updateOnDrag = true;
        [SerializeField, HideInInspector] Transform pointer;
        [SerializeField, HideInInspector] RectTransform rectTransform;

        #endregion
        #region Debug Variables

        [Header("Debug Settings")]
        [SerializeField] int meshIndexHighlight;
        [SerializeField] Vector2 outerMinAngles;
        [SerializeField] Vector2 outerMaxAngles;
        [SerializeField] Vector2 innerMinAngles;
        [SerializeField] Vector2 innerMaxAngles;
        [SerializeField] Vector2 hitAngles;
        [SerializeField] Vector2 ratio;

        #endregion
        
        #endregion


        #region Properties

        /// <summary>
        /// TRUE if <paramref name="foregroundImg"/> is not NULL.
        /// </summary>
        public bool HasForegroundImage { 
            get { return null != foregroundImg; } }

        /// <summary>
        /// Minimum angles of <paramref name="foregroundImg"/>.
        /// </summary>
        public Vector2 InnerMinAngles { 
            get { return innerMinAngles; } }

        /// <summary>
        /// Maximum angles of <paramref name="foregroundImg"/>.
        /// </summary>
        public Vector2 InnerMaxAngles { 
            get { return innerMaxAngles; } }

        /// <summary>
        /// Minimum angles of <typeparamref name="RectTransform"/>.
        /// </summary>
        public Vector2 OuterMinAngles { 
            get { return outerMinAngles; } }

        /// <summary>
        /// Maximum angles of <typeparamref name="RectTransform"/>.
        /// </summary>
        public Vector2 OuterMaxAngles { 
            get { return outerMaxAngles; } }

        /// <summary>
        /// Angles of the pointed-at position.
        /// </summary>
        public Vector2 HitAngles { 
            get { return hitAngles; } }

        /// <summary>
        /// Coordinate ratios within the area.
        /// </summary>
        public Vector2 Ratio { 
            get {return ratio; } }

        #endregion


        #region Events

        /// <summary>
        /// Called when <paramref name="margin"/> is changed.
        /// </summary>
        public Action OnMarginChanged;

        /// <summary>
        /// Called when a click occurs within the area.
        /// </summary>
        public Action<Vector2> OnAreaClicked;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        protected override void OnEnable()
        {
            base.OnEnable();
         
            rectTransform = (RectTransform)transform;

            CalculateAreaAngles();
        }

        protected override void Update ()
        {
            base.Update();

            if (margin >= 0 && oldMargin != margin && HasForegroundImage) {

                oldMargin = margin;

                OnMarginChanged?.Invoke();
    
                UpdateBackgroundSize();                   
            } 
        }

        #endregion
        #region Override Methods

        /// <summary>
        /// Update loop for VR.
        /// </summary>
        /// <returns>TRUE if the loop should be called again during the next frame.</returns>
        protected override bool VRInteractionUpdate ()
        {
            if (!base.VRInteractionUpdate()) { return false; }

            /// For some reason the source can become null when dragging outside of the area (even when base returns true after checking source).
            if (null != source && updateOnDrag && vrInput.PrimaryInteractionHeld) {

                ClickedInsideArea();
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

            /// For some reason the source can become null when dragging outside of the area (even when base returns true after checking source).
            if (null != source && updateOnDrag && desktopInput.PrimaryInteractionHeld) {

                ClickedInsideArea();
            }

            return true;
        }

        /// <summary>
        /// End the current click and invoke <paramref name="OnClick"/>.
        /// Calculate the click coordinates after that and send an Action with coordinate information.
        /// </summary>
        protected override void ClickEnded (InputContext _context)
        {
            if ((clickStarted && requireClickStartedToActivate) || !requireClickStartedToActivate) {

                clickStarted = false;
                OnClickUp.Invoke();

                ClickedInsideArea();
            }
        }

        #endregion
        #region Area Selector Methods

        /// <summary>
        /// Scale the background image based on margin size defined by this instance.
        /// </summary>
        protected virtual void UpdateBackgroundSize ()
        {
            RectTransform rt = foregroundImg.GetComponent<RectTransform>();

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(margin, margin);
            rt.offsetMax = new Vector2(-margin, -margin);
        } 

        /// <summary>
        /// Get the coordinate ratios, place a pointer object to the correct position and send an Action with ratios.
        /// </summary>
        protected virtual void ClickedInsideArea ()
        {
            ratio = GetClickCoordinates();

            /// Calculate the proper position of the pointer object.
            /// The pointer object should stay inside the backgroundImg area even if the click position is outside of it.
            if (displayPointer) {

                SetPointerPosition(ratio);
            }

            OnAreaClicked?.Invoke(ratio);                 
        }

        /// <summary>
        /// Calculate the angles of the main area's minimum and maximum points.
        /// If foreground image exists, calculate the angles for it as well.
        /// </summary>
        public virtual void CalculateAreaAngles () 
        {  
            Vector2 localPos = transform.InverseTransformPoint(source.Hit.point);

            ratio = new Vector2(
                (Image.rectTransform.rect.width / 2f + localPos.x) / Image.rectTransform.rect.width,
                (Image.rectTransform.rect.height / 2f + localPos.y) / Image.rectTransform.rect.height);        
        }

        /// <summary>
        /// Calculate the position of the click, defined as a ratio between 0...1 on X and Y axii.
        /// This ratio is the point inside backgroundImg.
        /// </summary>
        /// <returns>Vector2 of ratios between 0...1.</returns>
        protected virtual Vector2 GetClickCoordinates ()
        {
            if (null == source) { 
                
                #if UNITY_EDITOR
                Debug.LogError("AreaSelector.GetClickCoordinates(): source = NULL"); 
                #endif
                
                return new Vector2(0.5f, 0.5f);
            }

            Vector3 worldHitPoint = source.Hit.point;
            Vector2 ratio = new Vector2(0.5f, 0.5f);

            if (null == foregroundImg) { foregroundImg = Image; }

            Vector2 localPos = transform.InverseTransformPoint(source.Hit.point);
            
            ratio = new Vector2(
                (Image.rectTransform.rect.width / 2f + localPos.x) / Image.rectTransform.rect.width,
                (Image.rectTransform.rect.height / 2f + localPos.y) / Image.rectTransform.rect.height);
            
            Vector2 size = GetComponent<RectTransform>().rect.size;
            Vector2 marginPercentage = new Vector2(margin / size.x, margin / size.y);
            Vector2 correctedRatio = new Vector2(
                Mathf.Clamp((ratio.x - marginPercentage.x) / ((size.x - margin - margin) / size.x), 0f, 1f),
                Mathf.Clamp((ratio.y - marginPercentage.y) / ((size.y - margin - margin) / size.y), 0f, 1f));

            Vector2Int pixelCoordinates = new Vector2Int(
                Mathf.FloorToInt(ratio.x * rectTransform.rect.width),
                Mathf.FloorToInt(ratio.y * rectTransform.rect.height));

            return correctedRatio;
        }

        /// <summary>
        /// Set the pointer object to the position defined by <paramref name="_ratio"/>.
        /// </summary>
        /// <param name="_ratio">Ratio of position between min and max coordinates.</param>
        public void SetPointerPosition (Vector2 _ratio) {

            _ratio = new Vector2(
                Mathf.Clamp01(_ratio.x),
                Mathf.Clamp01(_ratio.y));

            if (null == pointer) {

                    pointer = Instantiate(Resources.Load("Prefabs/UI elements/Area Pointer") as GameObject).transform;
                    pointer.SetParent(InteractableTransform);
                    pointer.localScale = Vector3.one * 50f;     
                    pointer.gameObject.layer = LayerMask.NameToLayer("Default");       
            }

            Vector2 pointerAngles = new Vector2(
                    Mathf.Lerp(innerMinAngles.x, innerMaxAngles.x, _ratio.x),
                    Mathf.Lerp(innerMinAngles.y, innerMaxAngles.y, _ratio.y));

            /*
            Vector2 pointCoordinates = cUIElement.CurvedUIBase.GetCoordinatesFromAngles(pointerAngles, false);
            Vector3 pointPos = cUIElement.CurvedUIBase.GetPositionOnCurvedPlaneFromCoordinates(
                        pointCoordinates, cUIElement.CurvedUIBase.Radius);
*/
            pointer.position = Vector3.zero;//pointPos;
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace