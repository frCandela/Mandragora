using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(ConstellationStar)), CanEditMultipleObjects]
public class ConstellationStar_Editor : Editor
{
    void OnSceneGUI()
    {
        ConstellationStar script = (ConstellationStar)target;

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(script.m_initPosition, Quaternion.identity);
        Handles.SphereHandleCap(0, script.m_initPosition, Quaternion.identity, .1f, EventType.Repaint);		
		Handles.DrawDottedLine(newTargetPosition, script.transform.position, 4);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Change Target Position");
            script.m_initPosition = newTargetPosition;
        }
    }
}