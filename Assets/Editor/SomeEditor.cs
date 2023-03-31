using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrostBit
{
    [CustomEditor(typeof(SimMath))]
    public class SomeEditor : Editor
    {

        public Object source;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SimMath myScript = (SimMath)target;
            EditorGUILayout.HelpBox("Available Math Class\n-PipeMath\n-ValveMath\n-PumpMath\n-PipeTMath\n-PipeTMath2In1\n-HeatMath\n-ConsumerMath\n-HeatExchangerMath\n", MessageType.Info);


        }
    }
}