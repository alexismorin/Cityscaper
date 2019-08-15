using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cityscaper))]
public class CityscaperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Cityscaper managerScript = (Cityscaper)target;
        if (GUILayout.Button("Generate"))
        {
            managerScript.Regenerate();
        }
    }
}