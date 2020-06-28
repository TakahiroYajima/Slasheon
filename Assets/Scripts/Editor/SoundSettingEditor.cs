using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SoundSettingEditor : EditorWindow {
    public static SoundSettingEditor instance { get; private set; }
    private SoundScriptable soundScriptable = null;
    private const string ASSET_PATH = "Assets/Resources/SoundData.asset";

    public List<SoundData> soundList = new List<SoundData>();
    public List<bool> activeList = new List<bool>();

    public Color defaultColor { get; private set; }
    private Vector2 scrollPosition = Vector2.zero;

    private OK_Or_CancelWindow ok_or_canselWindow = null;

    [MenuItem("Editor/SoundSetting")]
    public static void CreateEditor()
    {
        if (instance == null)
        {
            instance = CreateInstance<SoundSettingEditor>();
            instance.Init();
            instance.ShowUtility();
            instance.defaultColor = new Color();
        }
    }

    public void Init()
    {
        soundScriptable = AssetDatabase.LoadAssetAtPath<SoundScriptable>(ASSET_PATH);
        defaultColor = GUI.backgroundColor;
        activeList.Clear();
        for(int i = 0; i< soundList.Count; i++)
        {
            activeList.Add(false);
        }
        if (soundScriptable == null)
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
        if(ok_or_canselWindow != null)
        {
            return;
        }
        if (soundScriptable == null)
        {
            Import();
        }
        EditorGUILayout.LabelField("サウンド設定エディタ");
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
            GUILayout.Space(80);
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                if (GUILayout.Button("追加", GUILayout.Width(120)))
                {
                    AddSound();
                }
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);
            {
                SoundDataSettingLayout();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void SoundDataSettingLayout()
    {
        for(int findID = 0; findID < soundList.Count; findID++)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label((findID + 1).ToString());
            }
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    string s = activeList[findID] ? "-" : "+";
                    if (GUILayout.Button(s, GUILayout.Width(20)))
                    {
                        activeList[findID] = !activeList[findID];
                    }

                    if (activeList[findID])
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            soundList[findID].name = EditorGUILayout.TextField("ID（名前）", soundList[findID].name);
                            soundList[findID].audio = EditorGUILayout.ObjectField("Sound", soundList[findID].audio, typeof(AudioClip), false) as AudioClip;
                            soundList[findID].isLoop = EditorGUILayout.Toggle("ループ", soundList[findID].isLoop);
                            if (soundList[findID].isLoop)
                            {
                                soundList[findID].loopBeginTime = EditorGUILayout.FloatField("ループ再生開始地点", soundList[findID].loopBeginTime);
                                soundList[findID].loopEndTime = EditorGUILayout.FloatField("ループ地点", soundList[findID].loopEndTime);
                            }
                            GUILayout.Space(30);
                            if (GUILayout.Button("削除", GUILayout.Width(120)))
                            {
                                DeleteSound(findID);
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
    private void AddSound()
    {
        soundList.Add(new SoundData());
        activeList.Add(false);
    }
    private void DeleteSound(int findID)
    {
        soundList.RemoveAt(findID);
        activeList.RemoveAt(findID);
    }

    private void Import()
    {
        if(soundScriptable == null)
        {
            soundScriptable = ScriptableObject.CreateInstance<SoundScriptable>();
        }
        SoundScriptable scriptable = AssetDatabase.LoadAssetAtPath<SoundScriptable>(ASSET_PATH);

        if (scriptable == null)
        {
            Debug.Log("ScriptableObjectがnullです");
            return;
        }
        soundScriptable.Copy(scriptable);
        soundScriptable = scriptable;

        soundList.Clear();
        activeList.Clear();
        for (int i = 0; i < soundScriptable.soundDatas.Count; i++)
        {
            soundList.Add(soundScriptable.soundDatas[i]);
            activeList.Add(false);
        }
    }

    private void Export()
    {
        soundScriptable = AssetDatabase.LoadAssetAtPath<SoundScriptable>(ASSET_PATH);
        if (soundScriptable == null)
        {
            soundScriptable = ScriptableObject.CreateInstance<SoundScriptable>();
        }
        // 新規の場合はディレクトリ作成
        if (!AssetDatabase.Contains(soundScriptable as UnityEngine.Object))
        {
            string directory = System.IO.Path.GetDirectoryName(ASSET_PATH);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            // アセット作成
            AssetDatabase.CreateAsset(soundScriptable, ASSET_PATH);
            soundScriptable.Copy(soundScriptable);
            //ScriptableObjectを設定
            SetScriptable();

            // 更新通知
            EditorUtility.SetDirty(soundScriptable);
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
                soundScriptable.Copy(soundScriptable);
                //ScriptableObjectを設定
                SetScriptable();

                // 更新通知
                EditorUtility.SetDirty(soundScriptable);
                // 保存
                AssetDatabase.SaveAssets();
                // エディタを最新の状態にする
                AssetDatabase.Refresh();
                ok_or_canselWindow.Close();
                ok_or_canselWindow = null;
            });
            ok_or_canselWindow.SetCancelAction(() =>
            {
                ok_or_canselWindow.Close();
                ok_or_canselWindow = null;
            });
        }
        
    }

    private void SetScriptable()
    {
        soundScriptable.SetSoundDatas(soundList);
    }

    /// <summary>
    /// エディタでの設定中
    /// </summary>
    private void EditorStartUp()
    {
        // インスペクターから設定できないようにする
        soundScriptable.hideFlags = HideFlags.NotEditable;
    }
    /// <summary>
    /// エディタ非表示中
    /// </summary>
    private void EditorOFF()
    {
        // インスペクターから設定させる
        soundScriptable.hideFlags = HideFlags.None;
    }
}

