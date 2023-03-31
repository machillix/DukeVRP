/******************************************************************************
 * File        : VRLaser.cs
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


namespace DUKE.Controls {


    /// <summary>
    /// Display modes of the point object at the end point of <typeparamref name="VRLaser"/>.
    /// </summary>
    public enum PointDisplayType
    {
        Never,
        OnInteractable,
        OnHit    
    }


    /// <summary>
    /// Controls the visual representation of a VR-controller's laser pointer.
    /// </summary>
    public class VRLaser : MonoBehaviour 
    {
        #region Variables

        [SerializeField] VRInput input;
        [SerializeField] LineRenderer line;
        [SerializeField] Light glowLight;
        [SerializeField] Color idleColor;
        [SerializeField] Color highlightColor;
        [SerializeField] Color interactColor;
        [SerializeField] float colorIntensity;
        [SerializeField] float glowLightIntensity;
        [SerializeField] float idleLaserLength = 1f;
        [SerializeField] float laserWidth;


        Material mat;
        Transform point;


        [Space(10), SerializeField] PointDisplayType pointDisplayType;
        [SerializeField] float pointSize;
        [SerializeField] float pointSizeMultiplier;
        [SerializeField] Transform overlap;
        
        #endregion


        #region Properties

        public bool Enabled 
        {
            get;
            set;
        }

        /// <summary>TRUE if laser should be visible.</summary>
        public bool ShowLaser 
        { 
            get; 
            set; 
        }

        /// <summary>TRUE is overlap sphere should be visible.</summary>
        public bool ShowOverlap 
        { 
            get; 
            set; 
        }

        /// <summary>Current color of the laser.</summary>
        public Color ActiveColor 
        { 
            get; 
            protected set; 
        }

        /// <summary>Current intensity of the laser.</summary>
        public float ActiveIntensity 
        { 
            get { return colorIntensity; } 
        }

        #endregion


        #region Methods

        private void Start ()
        {
            if (null == input) { input = transform.parent.parent.GetComponent<VRInput>(); }

            point = transform.parent.Find("Laser Hit Point");
            ShowLaser = true;

            mat = Instantiate(Resources.Load("Materials/EmissiveColor") as Material);      
            mat.color = idleColor;

            point.GetComponent<MeshRenderer>().material = mat;
            line.material = mat;
            line.positionCount = 2;
            line.useWorldSpace = false;
            Enabled = true;
        }

        private void LateUpdate ()
        {
            if (!Enabled) 
            { 
                line.enabled = false; 
                point.gameObject.SetActive(false);
                return; 
            }

            idleLaserLength = input.RayDistance;

            ShowOverlap = input.OverlapTarget;
            ShowLaser = !ShowOverlap;

            if (ShowLaser) 
            {
                overlap.gameObject.SetActive(false);
                line.enabled = true;

                // Input is currently interacting with an object.
                if (input.SavedRayLength > 0f)
                {
                    line.SetPosition(0, Vector3.zero);   
                    line.SetPosition(1, Vector3.up * (input.SavedRayLength - 0.1f));
                    line.startWidth = laserWidth;
                    line.endWidth = laserWidth;          
                    
                    ActiveColor = input.TriggerPressed ? interactColor : idleColor;  
                    point.gameObject.SetActive(true);
                }
                else
                {
                    // Input is pointing at NULL.
                    if (null == input.PointerTarget) 
                    {
                        // Input ray is hitting a blocking object.
                        if (input.RayIsBlocked && input.Hit.distance < idleLaserLength) 
                        {
                            line.SetPosition(0, Vector3.zero);   
                            line.SetPosition(1, Vector3.up * Mathf.Min(input.BlockedRayLength - 0.1f, idleLaserLength));     
                            line.startWidth = laserWidth;
                            line.endWidth = laserWidth;                

                            ActiveColor = input.TriggerPressed ? interactColor : idleColor;  
                            point.gameObject.SetActive(pointDisplayType == PointDisplayType.OnHit);
                            point.position = transform.TransformPoint(line.GetPosition(1));
                        }
                        // Input ray is not hitting anything.
                        else 
                        {
                            line.SetPosition(0, Vector3.zero);
                            line.SetPosition(1, Vector3.up * idleLaserLength);   
                            line.startWidth = laserWidth;
                            line.endWidth = 0f;

                            ActiveColor = input.TriggerPressed ? interactColor : idleColor;  
                            point.gameObject.SetActive(false);
                        }       
                    } 
                    // Input is pointing at an interactable object.
                    else 
                    {
                        line.SetPosition(0, Vector3.zero);
                        line.SetPosition(1, transform.InverseTransformPoint(input.Hit.point));  
                        line.startWidth = laserWidth;
                        line.endWidth = laserWidth;

                        ActiveColor = input.TriggerPressed ? interactColor : highlightColor;   
                        point.gameObject.SetActive(pointDisplayType != PointDisplayType.Never);  
                        point.position = input.Hit.point;
                    }

                    mat.SetColor("_Color", ActiveColor);
                    mat.SetFloat("_Intensity", ActiveIntensity);
                    glowLight.color = ActiveColor;
                    glowLight.intensity = glowLightIntensity;

                    if (point.gameObject.activeSelf) 
                    {
                        float length = Vector3.Distance(Vector3.zero, line.GetPosition(1));      
                        point.localScale = (1 + (length / pointSizeMultiplier)) * pointSize * Vector3.one;
                    }
                }
            } 
            else 
            {
                point.gameObject.SetActive(false);
                line.enabled = false;

                overlap.gameObject.SetActive(true);
                overlap.transform.position = input.TouchPointPosition;
                overlap.transform.localScale = Vector3.one * input.TouchRadius * 2f;
            }         
        }

        #endregion


    } /// End of Class


} /// End of Namespace