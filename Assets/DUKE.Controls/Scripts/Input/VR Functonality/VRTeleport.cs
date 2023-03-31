/******************************************************************************
 * File        : VRTeleport.cs
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


    /// <summary>Allow user to move around in the environment with teleportation.</summary>
    public class VRTeleport : MonoBehaviour 
    {
        #region Variables

        [SerializeField] Transform player;
        [SerializeField] VRInput input;
        [SerializeField] VRLaser laser;
        [SerializeField] GameObject teleportRingObj;
        Transform arcObj;
        Transform teleportRing;
        MeshFilter mFilter;
        MeshRenderer mRenderer;
        bool calculatingTeleport = false;
        RaycastHit arcHit;
        RaycastHit arcBlock;





        [SerializeField]  LayerMask teleportableLayers;
        [SerializeField]  LayerMask collisionLayers;
        [SerializeField]  Material arcMaterial;
        [SerializeField]  Color teleportDeniedColor;
        [SerializeField]  Color teleportAllowedColor;
        [SerializeField] float arcIntensity;

        /// <summary>
        /// TRUE if teleportation is allowed.
        /// </summary>
        [SerializeField]  bool allowTeleportation = false;

        /// <summary>
        /// Increases the reach of the teleportation.
        /// </summary>
        [SerializeField]  float arcVelocity;

        /// <summary>
        /// Maximum arc step count for calculating the arc.
        /// </summary>
        [SerializeField]  int maxSteps;

        /// <summary>
        /// Length of a single arc step.
        /// </summary>
        [SerializeField]  float stepLength = 0.01f;

        /// <summary>
        /// Vertex count of an arc's cross section.
        /// </summary>
        [SerializeField]  int cylinderResolution;

        /// <summary>
        /// Width of the arc at the start point.
        /// </summary>
        [SerializeField]  float startWidth;

        /// <summary>
        /// Width of the arc at the end point.
        /// </summary>
        [SerializeField]  float endWidth;

        /// <summary>
        /// TRUE if debug visuals should be drawn.
        /// </summary>
        [SerializeField]  bool drawDebug;


        Coroutine teleportUpdateCoroutine;
        Vector3 smoothedPointingDirection;
        Vector3[] arcPoints;
        List<Vector3> arcPointList;
        Vector3[] vertices;
        
        #endregion


        #region Properties

        /// <summary>
        /// TRUE if the current teleportation spot is valid.
        /// </summary>
        public bool IsValidTeleportSpot { 
            get; 
            private set; }

        /// <summary>
        /// TRUE if the arc collided with an object that belongs to <paramref name="collisionLayers"/>.
        /// </summary>
        public bool LineCollidedWithBlocker { 
            get; 
            private set; }

        public Color ArcColor
        {
            get { return IsValidTeleportSpot ? teleportAllowedColor : teleportDeniedColor; }
        }

        /// <summary>
        /// Current teleport position.
        /// </summary>
        public Vector3 TeleportPoint { 
            get { return teleportRing.position; } }

        /// <summary>
        /// Forward direction of the teleportation action at the arc's end point.
        /// </summary>
        public Vector3 TeleportForwardDirection { 
            get { return teleportRing.forward; } }

        #endregion


        #region Events

        public Action OnTeleportBegan;
        public Action OnTeleportCompleted;
        public Action OnTeleportCanceled;

        #endregion


        #region Methods

        #region MonoBehaviour Methods
        
        void Start ()
        {
            GetReferences();

            arcPointList = new List<Vector3>();

            if (maxSteps < 3)                   maxSteps = 3;
            if (cylinderResolution % 2 == 0)    cylinderResolution++;

            smoothedPointingDirection = transform.forward;
            arcMaterial = Instantiate(arcMaterial) as Material;
            arcMaterial.SetColor("_Color", ArcColor);
            arcMaterial.SetFloat("_Intensity", 5f);
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            input.OnPrimary2DAxisTouchDown += BeginTeleportArcCalculation;
            input.OnPrimary2DAxisTouchUp += EndTeleportArcCalculation;
            input.OnPrimary2DAxisDown += Teleport;

            OnTeleportBegan += input.PreventRaycasting;
            OnTeleportCanceled += input.AllowRaycasting;
        }

        void OnDisable ()
        {
            input.OnPrimary2DAxisTouchDown -= BeginTeleportArcCalculation;
            input.OnPrimary2DAxisTouchUp -= EndTeleportArcCalculation;
            input.OnPrimary2DAxisDown -= Teleport;

            OnTeleportBegan -= input.PreventRaycasting;
            OnTeleportCanceled -= input.AllowRaycasting;
        }
        
        void Update ()
        {
            if (!allowTeleportation) 
            { 
                if (null != teleportRing) { Destroy(teleportRing.gameObject); }

                return; 
            }

            smoothedPointingDirection = Vector3.Lerp(smoothedPointingDirection, transform.forward, 0.2f);
        }


        
        void OnDrawGizmos ()
        {
            if (!drawDebug)             { return; }
            if (!Application.isPlaying) { return; }

            if (null != arcPoints) 
            {
                if (arcPoints.Length > 0) 
                {
                    for (int i = 1; i < arcPoints.Length; i++) 
                    {
                        Gizmos.color = (i % 2 == 0 ? Color.black : Color.white);
                        Gizmos.DrawLine(arcPoints[i - 1], arcPoints[i]);
                    }
                } 
            }

            if(null != vertices) 
            {
                for(int i = 0; i < vertices.Length; i++) 
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(vertices[i], 0.01f);
                }
            } 
        }

        #endregion
        #region Arc Calculation and Visual Representation Methods 

        protected virtual void BeginTeleportArcCalculation (InputContext _context)
        {
            if (null != teleportUpdateCoroutine)
            {
                StopCoroutine(teleportUpdateCoroutine);
                teleportUpdateCoroutine = null;
            }

            laser.Enabled = false;
            calculatingTeleport = true;
            teleportUpdateCoroutine = StartCoroutine(DoTeleportArc());
        }
        protected virtual void EndTeleportArcCalculation (InputContext _context)
        {
            if (null != teleportUpdateCoroutine)
            {
                calculatingTeleport = false;
                OnTeleportCanceled?.Invoke();
            } 

            laser.Enabled = true;
        }

        protected virtual void Teleport (InputContext _context)
        {
            if (IsValidTeleportSpot && null != teleportRing)
            {
                player.position = teleportRing.position;
                player.rotation = teleportRing.rotation;      

                OnTeleportCompleted?.Invoke();       
            }
        }


        /// <summary>
        /// Get the required references.
        /// </summary>
        void GetReferences ()
        {
            if (!TryGetComponent(out mFilter))      { mFilter = gameObject.AddComponent<MeshFilter>(); }
            if (!TryGetComponent(out mRenderer))    { mRenderer = gameObject.AddComponent<MeshRenderer>(); }
            
            mRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
              
        /// <summary>
        /// Calculate the points of the arc.
        /// </summary>
        void CalculateArc ()
        {
            Ray r;
            List<Vector3> pointList = new List<Vector3>();
            //Vector3[] points = new Vector3[maxSteps];
            int iterations = 1;
            pointList.Add(transform.position + transform.forward * 0.1f);

            for(int i = 1; i < maxSteps; i++) {

                iterations++;

                Vector3 point = pointList[i-1] 
                    + (transform.forward * arcVelocity * stepLength) 
                    + (Vector3.down * 9.81f * stepLength * (i * stepLength));
                  
                r = new Ray(pointList[i - 1], (point - pointList[i - 1]));
                float rayLength = Vector3.Distance(point, pointList[i - 1]);

                /// Cast every point twice, First for teleportable layers. 
                /// After that for any layer that prevents normal movement.
                /// (We don't want the player to teleport through a wall.)
                if (Physics.Raycast(r, out arcHit, rayLength, teleportableLayers)) 
                {
                    IsValidTeleportSpot = true;
                    LineCollidedWithBlocker = false; 
                    pointList.Add(arcHit.point);

                    break;
                } 
                else if (Physics.Raycast(r, out arcBlock, rayLength, collisionLayers)) 
                {
                    IsValidTeleportSpot = false;
                    LineCollidedWithBlocker = true;            
                    pointList.Add(arcBlock.point);

                    break;
                } 
                else 
                {
                    pointList.Add(point);
                }

                // Reached the maximum distance of the arc.
                if (i == maxSteps-1) 
                {
                    IsValidTeleportSpot = false;
                    LineCollidedWithBlocker = false;
                    teleportRing.gameObject.SetActive(false);
                }
            }

            //arcPoints = new Vector3[iterations];
            arcPointList = new List<Vector3>();

            for(int i = 0; i < pointList.Count; i++) 
            {
                arcPointList.Add(transform.InverseTransformPoint(pointList[i]));
            }
            
            arcPoints = arcPointList.ToArray();
        }
        
        /// <summary>
        /// Create the arc <typeparamref name="Mesh"/>. 
        /// </summary>
        void GenerateLineMesh ()
        {
            Mesh m = new Mesh();
            int[] triangles = new int[cylinderResolution * arcPoints.Length * 6];
            float angleStep = 360f / cylinderResolution;
            vertices = new Vector3[cylinderResolution * arcPoints.Length];

            for(int i = 0; i < arcPoints.Length; i++) {

                float ratioOfTotalDistance = i / (float)arcPoints.Length;
                float width = startWidth + (ratioOfTotalDistance * (endWidth - startWidth));
                Vector3 centerPoint = arcPoints[i];
                Vector3 dir;

                if (i == 0)                         {  dir = (arcPoints[i] - arcPoints[i + 1]).normalized; }
                else if (i == arcPoints.Length - 1) {  dir = (arcPoints[i - 1] - arcPoints[i]).normalized; }
                else                                { dir = ((arcPoints[i - 1] - arcPoints[i]) + (arcPoints[i] - arcPoints[i + 1])).normalized; }
                
                Vector3 cross = Vector3.Cross(dir, transform.right);
                
                for(int j = 0; j < cylinderResolution; j++) 
                {
                    vertices[i * cylinderResolution + j] = centerPoint + Quaternion.AngleAxis(j * angleStep, dir) * (cross * (width / 2f));
                }

                if(i > 0) 
                {
                    for (int j = 0; j < cylinderResolution; j++) 
                    {
                        int triangleStartIndex = (i - 1) * cylinderResolution * 6 + j * 6;
                        int prevA = (i - 1) * cylinderResolution + j + 0;
                        int prevB =     j == cylinderResolution - 1 
                                        ? ((i - 1) * cylinderResolution + 0) 
                                        : ((i - 1) * cylinderResolution + j + 1);
                        int curA =      i * cylinderResolution + j + 0;
                        int curB =      j == cylinderResolution - 1
                                        ? (i * cylinderResolution + 0)
                                        : (i * cylinderResolution + j + 1);

                        triangles[triangleStartIndex + j * 6 + 0] = prevA;
                        triangles[triangleStartIndex + j * 6 + 1] = curA;
                        triangles[triangleStartIndex + j * 6 + 2] = prevB;
                        triangles[triangleStartIndex + j * 6 + 3] = prevB;
                        triangles[triangleStartIndex + j * 6 + 4] = curA;
                        triangles[triangleStartIndex + j * 6 + 5] = curB;
                    }
                }
            }

            m.vertices = vertices;
            m.triangles = triangles;
            m.RecalculateNormals();
            m.Optimize();

            mFilter.mesh = m;
            mRenderer.material = arcMaterial;
            mRenderer.material.color = ArcColor;
        }
        
        /// <summary>Set the position and rotation of the teleportation end point.</summary>
        /// <param name="_pos"></param>
        void UpdateTeleportRing(Vector3 _pos)
        {    
            if (null == teleportRing)                   { return; }
            if (!teleportRing.gameObject.activeSelf)    { teleportRing.gameObject.SetActive(true); }

            Vector3 levelForward = new Vector3(transform.forward.x, 0f, transform.forward.z);
            Vector3 inputDirection = new Vector3(-input.Primary2DAxisValue.x, 0f, input.Primary2DAxisValue.y);
            float signedAngle = Vector3.SignedAngle(inputDirection, levelForward, Vector3.up);
            Vector3 finalDirection = Quaternion.AngleAxis(signedAngle, Vector3.up) * Vector3.forward;
            Quaternion rot = Quaternion.LookRotation(finalDirection, Vector3.up);
      
            teleportRing.GetComponent<MeshRenderer>().material.SetColor("_Color", ArcColor);
            
            teleportRing.position = _pos;
            teleportRing.rotation = rot;
        }
        
        #endregion
        #region Coroutines

        protected IEnumerator DoTeleportArc ()
        {
            OnTeleportBegan?.Invoke();

            teleportRing = (GameObject.Instantiate(teleportRingObj) as GameObject).transform;
            MeshRenderer mRend = teleportRing.GetComponent<MeshRenderer>();
            mRend.material = Instantiate(new Material(mRend.material));
            arcMaterial.SetColor("_Color", ArcColor);
            arcMaterial.SetFloat("_Intensity", arcIntensity);

            while (calculatingTeleport)
            {
                CalculateArc();
                GenerateLineMesh();
  
                if (IsValidTeleportSpot) 
                { 
                    teleportRing.gameObject.SetActive(true);
                    UpdateTeleportRing(arcHit.point); 
                }
                else
                {
                    teleportRing.gameObject.SetActive(false);
                }
     
                yield return new WaitForEndOfFrame();
            }

            Destroy(teleportRing.gameObject);
            mFilter.mesh = null;
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace