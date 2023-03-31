/******************************************************************************
 * File        : Interacrable_editor.cs
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
using UnityEditor;
using DUKE.Controls;


[CustomEditor(typeof(Interactable))]
public class Interactable_editor : Editor
{
    #region Variables

    protected SerializedProperty isInteractableProp;
    protected SerializedProperty interactableTransformProp;
    protected SerializedProperty vrInteractionButtonProp;
    protected SerializedProperty desktopInteractionModifierFlagsProp;
    protected SerializedProperty interactionModeProp;
    protected SerializedProperty drawDebugProp;

    #endregion


    #region Methods

    protected virtual void OnEnable ()
    {
        isInteractableProp = serializedObject.FindProperty("isInteractable");
        interactableTransformProp = serializedObject.FindProperty("interactableTransform");
        vrInteractionButtonProp = serializedObject.FindProperty("vrInteractionButton");
        desktopInteractionModifierFlagsProp = serializedObject.FindProperty("desktopInteractionModifierFlags");
        interactionModeProp = serializedObject.FindProperty("interactionMode");
        drawDebugProp = serializedObject.FindProperty("drawDebug");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(isInteractableProp);

        if (isInteractableProp.boolValue) 
        {
            EditorGUILayout.PropertyField(drawDebugProp);
        }

        serializedObject.ApplyModifiedProperties();
    }

    #endregion


}///  End of Class



[CustomEditor(typeof(Clickable)), CanEditMultipleObjects]
public class Clickable_editor : Interactable_editor 
{
    #region Variables

    private Clickable clickable;
    protected SerializedProperty onClickDownProp;
    protected SerializedProperty onClickUpProp;
    protected SerializedProperty requireClickStartedToActivateProp;
    protected SerializedProperty isToggleableProp;
    protected SerializedProperty isToggledProp;
    protected SerializedProperty isPartOfGroupProp;
    protected SerializedProperty groupCanBeOffProp;
    protected SerializedProperty isMultiToggleableInGroupProp;

    #endregion


    #region Methods

    protected override void OnEnable ()
    {
        clickable = (Clickable)serializedObject.targetObject;

        base.OnEnable();

        onClickDownProp = serializedObject.FindProperty("OnClickDown");
        onClickUpProp = serializedObject.FindProperty("OnClickUp");
        requireClickStartedToActivateProp = serializedObject.FindProperty("requireClickStartedToActivate");
        isToggleableProp = serializedObject.FindProperty("isToggleable");
        isToggledProp = serializedObject.FindProperty("isToggled");
        isPartOfGroupProp = serializedObject.FindProperty("isPartOfGroup");
        groupCanBeOffProp = serializedObject.FindProperty("groupCanBeOff");
        isMultiToggleableInGroupProp = serializedObject.FindProperty("isMultiToggleableInGroup");
    }

    public override void OnInspectorGUI ()
    {
        GUILayout.Space(10);
        base.OnInspectorGUI();        
        serializedObject.Update();

        if (isInteractableProp.boolValue) 
        {
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(onClickDownProp);
            EditorGUILayout.PropertyField(onClickUpProp);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(requireClickStartedToActivateProp);
            EditorGUILayout.PropertyField(isToggleableProp);

            if (isToggleableProp.boolValue) 
            {
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(isToggledProp);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("changeColorOnToggle"));
                EditorGUILayout.PropertyField(isPartOfGroupProp);
                EditorGUILayout.PropertyField(groupCanBeOffProp);
                EditorGUILayout.PropertyField(isMultiToggleableInGroupProp);
            }
        } 
        else 
        {
            EditorGUILayout.HelpBox("Interaction is currently disabled.", MessageType.Warning);
        }

        GUILayout.Space(20);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("toggleColor"));

        serializedObject.ApplyModifiedProperties();       
    }

    #endregion
    

} /// End of Class



[CustomEditor(typeof(Movable)), CanEditMultipleObjects]
public class Movable_editor : Interactable_editor 
{
    #region Variables

    private Movable movable;
    protected SerializedProperty movementAxesProp;
    protected SerializedProperty coordinateSystemProp;
    protected SerializedProperty limitMovementDistanceProp;
    protected SerializedProperty movementLimitsProp;
    protected SerializedProperty OnPositionProp;
    protected SerializedProperty OnRatioProp;

    #endregion


    #region Methods

    protected override void OnEnable ()
    {
        movable = (Movable)serializedObject.targetObject;
        
        base.OnEnable();

        movementAxesProp = serializedObject.FindProperty("movementAxes");
        coordinateSystemProp = serializedObject.FindProperty("coordinateSystem");
        limitMovementDistanceProp = serializedObject.FindProperty("limitMovementDistance");
        movementLimitsProp = serializedObject.FindProperty("movementLimits");
        OnPositionProp = serializedObject.FindProperty("OnPositionChange");
        OnRatioProp = serializedObject.FindProperty("OnRatioChange");
    }

    public override void OnInspectorGUI ()
    {
        GUILayout.Space(10);
        base.OnInspectorGUI();
        serializedObject.Update();

        if (isInteractableProp.boolValue) 
        {
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(interactableTransformProp);        
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(vrInteractionButtonProp);
            EditorGUILayout.PropertyField(desktopInteractionModifierFlagsProp);
            EditorGUILayout.PropertyField(interactionModeProp);
            GUILayout.Space(20);
            EditorGUILayout.PropertyField(movementAxesProp);
            EditorGUILayout.PropertyField(coordinateSystemProp);
            EditorGUILayout.PropertyField(OnPositionProp);
            GUILayout.Space(10f);
            EditorGUILayout.PropertyField(limitMovementDistanceProp);

            if (limitMovementDistanceProp.boolValue) 
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedMovToPosDir"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedMovToNegDir"));
                GUILayout.Space(10f);
                
                if (null != OnRatioProp)
                {
                    EditorGUILayout.PropertyField(OnRatioProp);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    #endregion


} /// End of Class



[CustomEditor(typeof(Rotatable)), CanEditMultipleObjects]
public class Rotatable_editor : Interactable_editor
{
    #region Variables

    private Rotatable rotatable;
    protected SerializedProperty OnRotateProp;

    #endregion


    #region Methods

    protected override void OnEnable()
    {
        rotatable = (Rotatable)serializedObject.targetObject;

        base.OnEnable();

        OnRotateProp = serializedObject.FindProperty("OnRotate");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        base.OnInspectorGUI();

        serializedObject.Update();

        if (isInteractableProp.boolValue)
        {
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(interactionModeProp);
            GUILayout.Space(20);
            EditorGUILayout.PropertyField(OnRotateProp);
        }
        else
        {
            EditorGUILayout.HelpBox("Interaction is currently disabled.", MessageType.Warning);          
        }

        serializedObject.ApplyModifiedProperties();
    }

    #endregion


} /// End of Class



[CustomEditor(typeof(Valve)), CanEditMultipleObjects]
public class Valve_editor : Interactable_editor 
{
    #region Variables

    private Valve valve;
    protected SerializedProperty debugRadiusProp;
    protected SerializedProperty minAngleProp;
    protected SerializedProperty maxAngleProp;
    protected SerializedProperty curAngleProp;
    protected SerializedProperty ratioProp;
    protected SerializedProperty OnMinProp;
    protected SerializedProperty OnMaxProp;
    protected SerializedProperty OnValChangeProp;
    protected SerializedProperty OnRatioChangeProp;

    #endregion


    #region Methods

    protected override void OnEnable()
    {
        valve = (Valve)serializedObject.targetObject;     

        base.OnEnable();

        debugRadiusProp = serializedObject.FindProperty("debugRadius");
        minAngleProp = serializedObject.FindProperty("minAngle");
        maxAngleProp = serializedObject.FindProperty("maxAngle");
        curAngleProp = serializedObject.FindProperty("curAngle");
        ratioProp = serializedObject.FindProperty("ratio");
        OnMinProp = serializedObject.FindProperty("OnMinimumAngle");
        OnMaxProp = serializedObject.FindProperty("OnMaximumAngle");
        OnValChangeProp = serializedObject.FindProperty("OnValueChange");
        OnRatioChangeProp = serializedObject.FindProperty("OnRatioChange");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        base.OnInspectorGUI();
        serializedObject.Update();

        if (drawDebugProp.boolValue)
        {
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(debugRadiusProp);
            GUILayout.Space(20);
        }

        if (isInteractableProp.boolValue)
        {
            EditorGUILayout.PropertyField(vrInteractionButtonProp);
            EditorGUILayout.PropertyField(desktopInteractionModifierFlagsProp);
            EditorGUILayout.PropertyField(interactionModeProp);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Angle Settings");
            EditorGUILayout.PropertyField(minAngleProp);
            EditorGUILayout.PropertyField(maxAngleProp);
            EditorGUILayout.PropertyField(curAngleProp);
            EditorGUILayout.PropertyField(ratioProp);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(OnMinProp);
            EditorGUILayout.PropertyField(OnValChangeProp);
            EditorGUILayout.PropertyField(OnRatioChangeProp);
            EditorGUILayout.PropertyField(OnMaxProp);
            GUILayout.Space(10);
        }
        else 
        {
            EditorGUILayout.HelpBox("Interaction is currently disabled.", MessageType.Warning);
        }


        serializedObject.ApplyModifiedProperties();
    }

    #endregion


} /// End of Class



[CustomEditor(typeof(Highlightable)), CanEditMultipleObjects]
public class Highlightable_editor: Interactable_editor
{
    #region Variables

    private Highlightable highlightable;
    protected SerializedProperty OnHighlightBeginProp;
    protected SerializedProperty OnHighlightEndProp;

    #endregion


    #region Methods

    protected override void OnEnable ()
    {
        highlightable = (Highlightable)serializedObject.targetObject;

        base.OnEnable();

        OnHighlightBeginProp = serializedObject.FindProperty("OnHighlightBegin");
        OnHighlightEndProp = serializedObject.FindProperty("OnHighlightEnd");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        base.OnInspectorGUI();
        serializedObject.Update();

        if (isInteractableProp.boolValue) 
        {
            EditorGUILayout.PropertyField(interactionModeProp);
            GUILayout.Space(20);
            EditorGUILayout.PropertyField(OnHighlightBeginProp);
            EditorGUILayout.PropertyField(OnHighlightEndProp);             
        } 
        else 
        {
            EditorGUILayout.HelpBox("Interaction is currently disabled.", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();      
    }

    #endregion


} /// End of Class
