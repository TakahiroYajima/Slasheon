#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageSettingEditor : EditorWindow
{
    public static StageSettingEditor instance { get; private set; }
    private const string ASSET_PATH = "Assets/Resources/StageData.asset";
    private StageScriptable stageScriptable = null;

    public List<StageSettingEdit> stageList = new List<StageSettingEdit>();
    public List<bool> stageActiveList = new List<bool>();

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
        stageScriptable = AssetDatabase.LoadAssetAtPath<StageScriptable>(ASSET_PATH);
        defaultColor = GUI.backgroundColor;
        stageActiveList.Clear();
        for (int i = 0; i < stageList.Count; i++)
        {
            stageActiveList.Add(false);
        }
        if (stageScriptable == null)
        {
            Import();
        }
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
        if (stageScriptable == null)
        {
            Import();
        }

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                if (GUILayout.Button("読み込み"))
                {
                    Import();
                }
                if (GUILayout.Button("書き込み"))
                {
                    Export();
                }
            }
            GUILayout.Space(30);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);
            {
                EditorGUILayout.LabelField("ステージ設定");
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    using (new GUILayout.HorizontalScope(GUI.skin.box))
                    {
                        if (GUILayout.Button("追加", GUILayout.Width(120)))
                        {
                            AddStage();
                        }
                    }

                    for (int findID = 0; findID < stageList.Count; findID++)
                    {
                        using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                        {
                            GUILayout.Label((findID + 1).ToString());
                        }
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                string s = stageActiveList[findID] ? "-" : "+";
                                if (GUILayout.Button(s, GUILayout.Width(20)))
                                {
                                    stageActiveList[findID] = !stageActiveList[findID];
                                }

                                if (stageActiveList[findID])
                                {
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    {
                                        stageList[findID].stageKey = EditorGUILayout.TextField("Key", stageList[findID].stageKey);
                                        stageList[findID].stagePref = EditorGUILayout.ObjectField("ステージプレハブ", stageList[findID].stagePref, typeof(GameObject), false) as GameObject;
                                        GUILayout.Space(30);
                                        if (GUILayout.Button("削除", GUILayout.Width(120)))
                                        {
                                            DeleteStage(findID);
                                        }
                                        GUILayout.Space(30);
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }

                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void AddStage()
    {
        stageList.Add(new StageSettingEdit("",null));
        stageActiveList.Add(false);
    }
    private void DeleteStage(int findID)
    {
        stageList.RemoveAt(findID);
        stageActiveList.RemoveAt(findID);
    }

    private void Import()
    {
        if (stageScriptable == null)
        {
            stageScriptable = ScriptableObject.CreateInstance<StageScriptable>();
        }
        StageScriptable scriptable = AssetDatabase.LoadAssetAtPath<StageScriptable>(ASSET_PATH);

        if (scriptable == null)
        {
            Debug.Log("ScriptableObjectがnullです");
            return;
        }
        stageScriptable.Copy(scriptable);
        stageScriptable = scriptable;

        stageList.Clear();
        stageActiveList.Clear();
        foreach(var stage in stageScriptable.stageDatas)
        for (int i = 0; i < stageScriptable.stageDatas.Count; i++)
        {
            stageList.Add(new StageSettingEdit(stage.Key,stage.Value));
            stageActiveList.Add(false);
        }
    }
    private void Export()
    {
        stageScriptable = AssetDatabase.LoadAssetAtPath<StageScriptable>(ASSET_PATH);
        if (stageScriptable == null)
        {
            stageScriptable = ScriptableObject.CreateInstance<StageScriptable>();
        }
        // 新規の場合はディレクトリ作成
        if (!AssetDatabase.Contains(stageScriptable as UnityEngine.Object))
        {
            string directory = System.IO.Path.GetDirectoryName(ASSET_PATH);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            // アセット作成
            AssetDatabase.CreateAsset(stageScriptable, ASSET_PATH);
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
        Dictionary<string, GameObject> setData = new Dictionary<string, GameObject>();
        for(int i = 0; i < stageList.Count; i++)
        {
            setData.Add(stageList[i].stageKey, stageList[i].stagePref);
        }
        stageScriptable.SetStageDatas(setData);
    }

    /// <summary>
    /// エディタでの設定中
    /// </summary>
    private void EditorStartUp()
    {
        // インスペクターから設定できないようにする
        stageScriptable.hideFlags = HideFlags.NotEditable;
    }
    /// <summary>
    /// エディタ非表示中
    /// </summary>
    private void EditorOFF()
    {
        // インスペクターから設定させる
        stageScriptable.hideFlags = HideFlags.None;
    }
}

[System.Serializable]
public class StageSettingEdit
{
    public string stageKey;
    public GameObject stagePref;

    public StageSettingEdit(string key, GameObject pref)
    {
        stageKey = key;
        stagePref = pref;
    }
}
#endif