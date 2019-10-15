using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MatchManager))]
public class ObjectBuilderEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    MatchManager myScript = (MatchManager)target;
    if (GUILayout.Button("Save AI"))
    {
        myScript.saveAI();
    }
  }
}