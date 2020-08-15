#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageSettingEditor : EditorWindow
{
    public static StageSettingEditor instance { get; private set; }
    private const string ASSET_PATH = "Assets/Resources/ScriptableObject/StageDatas/";
    private string exportName = "";
    private StageScriptable stageScriptable = null;

    public StageData stageData = null;
    //public List<bool> stageActiveList = new List<bool>();

    public Color defaultColor { get; private set; }
    private Vector2 scrollPosition = Vector2.zero;

    private OK_Or_CancelWindow ok_or_canselWindow = null;

    [MenuItem("Editor/StageSetting")]
    public static void CreateEditor()
    {
        if (instance == null)
        {
            instance = CreateInstance<StageSettingEditor>();
            instance.Init();
            instance.ShowUtility();
            instance.defaultColor = new Color();
        }
    }

    public void Init()
    {
        defaultColor = GUI.backgroundColor;

        EditorStartUp();
    }
    private void OnDestroy()
    {
        EditorOFF();
        instance = null;
    }

    private void OnGUI()
    {
        if (ok_or_canselWindow != null)
        {
            return;
        }
        Layout();
    }

    private void Layout()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                exportName = EditorGUILayout.TextField("読み込むステージ名", exportName);
            }
            EditorGUILayout.EndVertical();
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                if (GUILayout.Button("新規作成"))
                {
                    NewCreate();
                }
                if (!string.IsNullOrEmpty(exportName))
                {
                    if (GUILayout.Button("読み込み"))
                    {
                        Import(exportName);
                    }

                    if (GUILayout.Button("書き込み"))
                    {
                        Export(exportName);
                    }
                }
            }
            GUILayout.Space(30);
            if (stageData != null)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);
                {
                    EditorGUILayout.LabelField("ステージ設定");
                    GUILayout.Space(10);
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            {
                                exportName = EditorGUILayout.TextField("ファイル名", exportName);
                                stageData.prefab = EditorGUILayout.ObjectField("ステージプレハブ", stageData.prefab, typeof(GameObject), false) as GameObject;
                                GUILayout.Space(30);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndScrollView();
            }
        }
    }

    private void Import(string loadName)
    {
        if (stageScriptable == null)
        {
            stageScriptable = ScriptableObject.CreateInstance<StageScriptable>();
        }
        StageScriptable scriptable = AssetDatabase.LoadAssetAtPath<StageScriptable>(ASSET_PATH + loadName + ".asset");

        if (scriptable == null)
        {
            Debug.Log("ScriptableObjectがnullです");
            return;
        }
        stageScriptable.Copy(scriptable);
        stageScriptable = scriptable;

        stageData = stageScriptable.stageData;
    }
    private void NewCreate()
    {
        stageScriptable = ScriptableObject.CreateInstance<StageScriptable>();
        stageData = new StageData();
    }
    private void Export(string exportName)
    {
        string exportFilePath = ASSET_PATH + exportName + ".asset";
        //stageScriptable = AssetDatabase.LoadAssetAtPath<StageScriptable>(ASSET_PATH + exportName + ".asset");
        if (stageScriptable == null)
        {
            stageScriptable = ScriptableObject.CreateInstance<StageScriptable>();
        }
        // 新規の場合はディレクトリ作成
        if (!AssetDatabase.Contains(stageScriptable as UnityEngine.Object))
        {
            string directory = System.IO.Path.GetDirectoryName(exportFilePath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            // アセット作成
            AssetDatabase.CreateAsset(stageScriptable, exportFilePath);
            stageScriptable.Copy(stageScriptable);
            //ScriptableObjectを設定
            SetScriptable();

            // 更新通知
            EditorUtility.SetDirty(stageScriptable);
            // 保存
            AssetDatabase.SaveAssets();
            // エディタを最新の状態にする
            AssetDatabase.Refresh();
        }
        else
        {
            ok_or_canselWindow = CreateInstance<OK_Or_CancelWindow>();
            ok_or_canselWindow.ShowUtility();
            ok_or_canselWindow.SetOKAction(() =>
            {
                stageScriptable.Copy(stageScriptable);
                //ScriptableObjectを設定
                SetScriptable();

                // 更新通知
                EditorUtility.SetDirty(stageScriptable);
                // 保存
                AssetDatabase.SaveAssets();
                // エディタを最新の状態にする
                AssetDatabase.Refresh();
                ok_or_canselWindow.Close();
                ok_or_canselWindow = null;
                Layout();
            });
            ok_or_canselWindow.SetCancelAction(() =>
            {
                ok_or_canselWindow.Close();
                ok_or_canselWindow = null;
                Layout();
            });
        }
    }

    private void SetScriptable()
    {
        stageScriptable.SetStageDatas(stageData);
    }

    /// <summary>
    /// エディタでの設定中
    /// </summary>
    private void EditorStartUp()
    {
        // インスペクターから設定できないようにする
        //stageScriptable.hideFlags = HideFlags.NotEditable;
    }
    /// <summary>
    /// エディタ非表示中
    /// </summary>
    private void EditorOFF()
    {
        // インスペクターから設定させる
        //stageScriptable.hideFlags = HideFlags.None;
    }
}
#endif