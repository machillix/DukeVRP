/******************************************************************************
 * File        : DetectInputDevices.cs
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
using UnityEngine.XR;


namespace DUKE.Controls {


    /// <summary>
    /// Input modes (VR/desktop).
    /// </summary>
    public enum InputMode {
        None,
        VR,
        Desktop
    }

    public enum DeviceManufacturer
    {
        Unknown,
        HTC,
        Oculus,
        Valve
    }


    /// <summary>
    /// Detects which input devices have been connected, and enables the correct Player prefab.
    /// </summary>
    public class DetectInputDevices : MonoBehaviour 
    {
        #region Variables

        #region General Variables

        [Header("General Settings")]
        [SerializeField] InputMode inputMode;
        InputMode prevInputMode;
        DeviceManufacturer manufacturer = DeviceManufacturer.Unknown;

        #endregion
        #region VR Variables
 
        [Space(10f)]
        [Header("VR Settings")]
        [SerializeField] TrackingOriginModeFlags trackingOrigin;
        [SerializeField] GameObject VRPlayer;
        [SerializeField] GameObject VRControllerRight;
        [SerializeField] GameObject VRControllerLeft;
        
        
        InputDevice hmd;
        InputDevice rightController;
        InputDevice leftController;

        #endregion
        #region Desktop Variables

        [Space(10f)]
        [Header("Desktop Settings")]
        [SerializeField] GameObject DesktopPlayer;

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// Public static instance of <typeparamref name="DetectInputDevices"/>.
        /// </summary>
        public static DetectInputDevices Current 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Current <typeparamref name="InputMode"/>.
        /// </summary>
        public static InputMode InputMode 
        { 
            get { return Current.inputMode; } 
        }

        /// <summary>
        /// Most recently detected VR device manufacturer.
        /// </summary>
        public static DeviceManufacturer Manufacturer
        {
            get { return Current.manufacturer; }
            protected set 
            { 
                Current.manufacturer = value; 
                Debug.Log(Current.manufacturer+" set as Manufacturer!"); 
            }
        }

        #endregion


        #region Events

        /// <summary>
        /// Called when <typeparamref name="InputMode"/> is changed.
        /// </summary>
        public static Action<InputMode> InputModeChanged;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        void Awake ()
        {
            Current = this;
            StartCoroutine(CheckDevices(0.1f));
            prevInputMode = inputMode;

            if (null == VRPlayer)           { VRPlayer = GameObject.Find("Player (VR)"); }
            if (null == VRControllerRight)  { VRControllerRight = GameObject.Find("RightHand"); }
            if (null == VRControllerLeft)   { VRControllerLeft = GameObject.Find("LeftHand"); }

            hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
              
        void OnEnable ()
        {
            SubscribeToEvents();
        }
        
        void OnDisable ()
        {
            UnsubscribeFromEvents();
        }

        #endregion
        #region Event Handling Methods

        void SubscribeToEvents ()
        {
            InputDevices.deviceConnected += DeviceConnected;       
        }
        
        void UnsubscribeFromEvents ()
        {
            InputDevices.deviceConnected -= DeviceConnected;
        }

        #endregion
        #region Device Methods

        /// <summary>
        /// Check periodically if a device is connected. Invoked (repeat) at Awake.
        /// </summary>
        void UpdateDevicesPeriodically ()
        {
            if (null == hmd) 
            {
                #if UNITY_EDITOR
                Debug.LogWarning("Could not find a headset.");
                #endif
                return;
            }
            
            hmd.TryGetFeatureValue(CommonUsages.userPresence, out bool userPresent);



            if (userPresent) 
            { 
                inputMode = InputMode.VR; 
            } 
            else 
            { 
                inputMode = InputMode.Desktop; 
            }
             
            if (inputMode != prevInputMode) 
            {
                prevInputMode = inputMode;
                ChangeInputMode();
            }
        }
        
        /// <summary>
        /// Change the InputMode.
        /// </summary>
        void ChangeInputMode ()
        {
            InputModeChanged?.Invoke(inputMode);

            /// Change player position and rotation before switching the active object.
            if(inputMode == InputMode.VR) 
            {
                transform.position = new Vector3(
                    DesktopPlayer.transform.position.x, 
                    transform.position.y, 
                    DesktopPlayer.transform.position.z);
   
                VRPlayer.transform.rotation = DesktopPlayer.transform.rotation;

                /// Set the tracking mode origin to the desired Tracking Mode Origin.
                List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
                SubsystemManager.GetSubsystems(subsystems);

                foreach(XRInputSubsystem subsystem in subsystems) 
                {
                    subsystem.TrySetTrackingOriginMode(trackingOrigin);
                }
            } 
            else 
            {
                DesktopPlayer.transform.position = VRPlayer.transform.GetChild(0).position;
                DesktopPlayer.transform.rotation = VRPlayer.transform.rotation;
            }

            VRPlayer.SetActive(inputMode == InputMode.VR);
            DesktopPlayer.SetActive(inputMode == InputMode.Desktop);

            foreach (Input input in FindObjectsOfType<Input>(true)) 
            {
                input.ForcePointingEnded();
            }
        }

        /// <summary>
        /// Called when a device is connected.
        /// </summary>
        /// <param name="_device"></param>
        void DeviceConnected (InputDevice _device)
        {
            Manufacturer = _device.manufacturer switch 
            {
                "HTC" => DeviceManufacturer.HTC,
                "Oculus" => DeviceManufacturer.Oculus,
                "Valve" => DeviceManufacturer.Valve,
                _ => DeviceManufacturer.Unknown
            };

            Debug.Log("Device connected:   " + _device.manufacturer+"   |   "+_device.name);

            if (_device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted)) 
            { 
                hmd = _device; 
            }
            else if (_device.characteristics.HasFlag(InputDeviceCharacteristics.Right) &&
                _device.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand)) 
            {
                rightController = _device;
            }
            else if (_device.characteristics.HasFlag(InputDeviceCharacteristics.Left) &&
                _device.characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
            {
                leftController = _device;
            }
        }


        IEnumerator CheckDevices (float _interval)
        {
            yield return new WaitForEndOfFrame();

            while(true)
            {
                UpdateDevicesPeriodically();
                yield return new WaitForSecondsRealtime(_interval);
            }
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace