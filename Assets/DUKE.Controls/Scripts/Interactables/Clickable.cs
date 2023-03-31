/******************************************************************************
 * File        : Clickable.cs
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace DUKE.Controls {


    /// <summary>
    /// Receives clicks and activations.
    /// </summary>
    public class Clickable : Interactable 
    {
        #region Variables

        /// <summary>
        /// <typeparamref name="UnityEvent"/> which is activated when a click starts. 
        /// </summary>
        [SerializeField] protected UnityEvent OnClickDown;

        /// <summary>
        /// <typeparamref name="UnityEvent"/> which is activated when a click occurs. 
        /// </summary>
        [Space(20f)]
        [SerializeField] protected UnityEvent OnClickUp;

        /// <summary>
        /// TRUE when update events have been subscribed to.
        /// </summary>
        [SerializeField, HideInInspector] protected bool updateEventSubscribedTo = false;

        /// <summary>
        /// TRUE when a click has been started over this instance.
        /// </summary>
        [SerializeField, HideInInspector] protected bool clickStarted = false;

        /// <summary>
        /// Collider of this instance.
        /// </summary>
        [SerializeField, HideInInspector] protected Collider col;

        /// <summary>
        /// Base color of the instance.
        /// </summary>
        [SerializeField, HideInInspector] protected Color baseColor = Color.white;

        /// <summary>
        /// Toggled color of this instance.
        /// </summary>
        [SerializeField, HideInInspector] protected Color toggleColor = Color.green;



        /// <summary>
        /// TRUE registers a click only if it started and ended within the same instance.
        /// </summary>
        [Header("Button Settings")]
        [SerializeField] protected bool requireClickStartedToActivate = true;


        /// <summary>
        /// TRUE allows this instance to be toggleable.
        /// </summary>
        [SerializeField] protected bool isToggleable = false;

        /// <summary>
        /// TRUE when this instance is toggled on.
        /// </summary>
        [SerializeField] protected bool isToggled = false;
        
        /// <summary>
        /// TRUE allows the color to be changed on toggling.
        /// </summary>
        [SerializeField] protected bool changeColorOnToggle = false;
        
        /// <summary>
        /// TRUE marks this instance as a part of a group.
        /// </summary>
        [SerializeField] protected bool isPartOfGroup = false;
        
        /// <summary>
        /// TRUE allows the group be toggled off completely. 
        /// FALSE prevents the last toggled <typeparamref name="Clickable"/> in the group from being toggled off.
        /// </summary>
        [SerializeField] protected bool groupCanBeOff = false;
        
        /// <summary>
        /// TRUE allows multiple <typeparamref name="Clickables"/> in the group to be toggled on simultaneously.
        /// FALSE toggles every other <typeparamref name="Clickable"/> off when one is toggled on.
        /// </summary>
        [SerializeField] protected bool isMultiToggleableInGroup = false;
        
        #endregion


        #region Properties

        /// <summary>
        /// <typeparamref name="Image"/> component attached to <paramref name="InteractableTransform"/>.
        /// </summary>
        public Image Image { 
            get {return InteractableTransform.GetComponent<Image>(); } }
        
        /// <summary>
        /// TRUE when <paramref name="Image"/> is not NULL.
        /// </summary>
        public bool HasImage {
            get { return null != Image; } }

        /// <summary>
        /// TRUE allows this instance to be toggleable. 
        /// </summary>
        public bool IsToggleable { 
            get { return isToggleable; } }

        /// <summary>
        /// TRUE when this instance is toggled on.
        /// </summary>
        public bool IsToggled { 
            get { return isToggled; } }
       
        /// <summary>
        /// TRUE allows the color to be changed on toggling.
        /// </summary>
        public bool ChangeColorOnToggle { 
            get { return changeColorOnToggle; } }

        /// <summary>
        /// TRUE marks this instance as a part of a group.
        /// </summary>
        public bool IsPartOfGroup { 
            get { return isPartOfGroup; } }

        /// <summary>
        /// TRUE allows multiple <typeparamref name="Clickables"/> in the group to be toggled on simultaneously. FALSE toggles every other <typeparamref name="Clickable"/> off when one is toggled on.
        /// </summary>
        public bool IsMultiToggleableInGroup { 
            get { return isMultiToggleableInGroup; } }

        /// <summary>
        /// TRUE allows the group to be toggled off completely. FALSE prevents the last toggled <typeparamref name="Clickable"/> in the group from being toggled off. 
        /// </summary>
        public bool GroupCanBeOff { 
            get { return groupCanBeOff; } }
           
        /// <summary>
        /// Current color of the instance.
        /// </summary>
        public Color Color { 
            get { return ChangeColorOnToggle 
                ? IsToggled ? toggleColor : baseColor
                : HasImage ? Image.color : Color.clear; } }

        #endregion


        #region Events

        /// <summary>
        /// Called when this instance is toggled on or off.
        /// </summary>
        public Action<bool> Toggled;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Log message with Debug.Log(<paramref name="_s"/>).
        /// </summary>
        /// <param name="_s"></param>
        public virtual void LogMessage (string _s)
        {
            #if UNITY_EDITOR
            Debug.Log(_s);
            #endif
        }

        /// <summary>
        /// Toggle a <typeparamref name="GameObject"/> on/off.
        /// </summary>
        /// <param name="_obj"><typeparamref name="GameObject"/> to be toggled.</param>
        public virtual void ToggleGameObject (GameObject _obj)
        {
            _obj.SetActive(!_obj.activeSelf);
        }

        public  virtual void SetScale (float _newScale)
        {
            InteractableTransform.localScale = Vector3.one * _newScale;
        }

        /// <summary>
        /// Set the Clickable's 'IsToggled' to true or false if IsToggleable has been defined as true.
        /// </summary>
        /// <param name="_on">Desired state of IsToggled.</param>
        public virtual void SetToggled (bool _on, bool _ignoreRestrictions = false)
        {
            if (_on == true || (_on == false && CanBeToggledOff()) || _on == false && _ignoreRestrictions) {

                isToggled = _on;
                Toggled?.Invoke(_on);

                SetImageColor();
            }
        }

        /// <summary>
        /// Create a BoxCollider for the UI element if it doesn't have one yet.
        /// This method automates a tedious part of the manual UI setup, 
        /// but is not the perfect solution for complex element shapes etc.
        /// DO NOT USE WITH LAYOUT GROUPS! Greyed-out rect fields count as 0.
        /// </summary>
        public virtual void SetupColliderForUIElement ()
        {
            if (InteractableTransform.localScale == Vector3.zero) { return; }

            if (!InteractableTransform.TryGetComponent(out col) && InteractableTransform.TryGetComponent(out RectTransform rTrn)) {

                BoxCollider bCol = InteractableTransform.gameObject.AddComponent<BoxCollider>();
                bCol.size = new Vector3(rTrn.rect.width, rTrn.rect.height, 1f);
                col = bCol;       
            }

            SubscribeToEvents();
        }

        #endregion
        #region MonoBehaviour Methods

        protected virtual void Start ()
        {
            if (!InteractableTransform.TryGetComponent(out col) ) { 
                
                SetupColliderForUIElement(); 
            }
        }

        #endregion
        #region Override Methods

        protected override void AddInputListeners (InputContext _context)
        {
            if (_context.IsDesktop) {
                    
                _context.DesktopInput.OnLMBDown += ClickStarted;
                _context.DesktopInput.OnLMBUp += ClickEnded;
                
            } else if (_context.IsVR) {

                _context.VRInput.OnTriggerDown += ClickStarted;
                _context.VRInput.OnTriggerUp += ClickEnded;
            } 
        }

        protected override void RemoveInputListeners(InputContext _context)
        {
            if (_context.IsDesktop) {
                    
                    _context.DesktopInput.OnLMBDown -= ClickStarted;
                    _context.DesktopInput.OnLMBUp -= ClickEnded;
                    
                } else if (_context.IsVR) {

                    _context.VRInput.OnTriggerDown -= ClickStarted;
                    _context.VRInput.OnTriggerUp -= ClickEnded;
                } 
        }

        #endregion
        #region Setup Methods

        /// <summary>
        /// A new click started.
        /// </summary>
        protected virtual void ClickStarted (InputContext _context) 
        {
            clickStarted = true;
            OnClickDown?.Invoke();
        }

        /// <summary>
        /// End the current click and invoke <paramref name="OnClick"/>.
        /// </summary>
        protected virtual void ClickEnded (InputContext _context) 
        {
            if ((clickStarted && requireClickStartedToActivate) || !requireClickStartedToActivate) {

                clickStarted = false;

                if (HasImage && ChangeColorOnToggle) {

                    if (isToggleable)   { SetToggled(!isToggled); } 
                    else                { Image.color = baseColor; }

                    if (isPartOfGroup)  { UpdateGroup(); }
                }
                
               OnClickUp?.Invoke();
            }
        }

        /// <summary>
        /// Change the color of this Clickable's Image component if it has one.
        /// </summary>
        protected virtual void SetImageColor ()
        {          
            if (HasImage && ChangeColorOnToggle) {

                Image.color = IsToggled
                    ? toggleColor
                    : baseColor;
            }
        }

        #endregion
        #region Toggle Methods

        /// <summary>
        /// Update the toggled state of every Clickable in the same group as this Clickable.
        /// </summary>
        protected virtual void UpdateGroup ()
        {
            foreach (Clickable c in GetGroup()) {

                if (c == this)                      { continue; }
                if (!c.isMultiToggleableInGroup)    { c.SetToggled(false, true); }
            }
        }
        
        /// <summary>
        /// Determine whether this Clickable can be turned off based on other Clickables' IsToggled state in the group 
        /// and whether GroupCanBeTurnedOff is true or false.
        /// </summary>
        /// <returns>True if toggling off is allowed.</returns>
        protected virtual bool CanBeToggledOff ()
        {
            if (!groupCanBeOff) {

                bool lastActive = true;

                foreach (Clickable c in GetGroup()) {

                    if (c.IsToggled) { lastActive = false; }
                }

                return !lastActive;
            }

            return true;
        }
        
        /// <summary>
        /// Find every other Clickable that has the same parent than this and is also part of a group.
        /// </summary>
        /// <returns>Array of Clickables that share the same parent with this Clickable and have been set as part of a group.</returns>
        protected virtual Clickable[] GetGroup ()
        {
            List<Clickable> clickables = new List<Clickable>();

            foreach (Transform child in InteractableTransform.parent) {

                Clickable c;
                if (child.TryGetComponent(out c)) {

                    if (this != c && c.isPartOfGroup) { clickables.Add(c); }
                }
            }

            return clickables.ToArray();
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace