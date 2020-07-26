#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class OK_Or_CancelWindow : EditorWindow {

    public delegate void OKButtonAction();
    private OKButtonAction okAction = null;
    public delegate void CancelButtonAction();
    private CancelButtonAction cancelAction = null;

    public const float WidthSize = 500f;
    public const float HeightSize = 200f;

    public void SetOKAction(OKButtonAction callback)
    {
        okAction = callback;
    }
    public void SetCancelAction(CancelButtonAction callback)
    {
        cancelAction = callback;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("データを書き出しますか？");
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Space(50);
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                if (GUILayout.Button("OK", GUILayout.Width(100)))
                {
                    okAction();
                }
                GUILayout.Space(50);
                if (GUILayout.Button("Cancel", GUILayout.Width(100)))
                {
                    cancelAction();
                }
            }
            GUILayout.Space(50);
        }
    }
}
#endif