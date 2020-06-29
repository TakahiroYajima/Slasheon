using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SoundSettingEditor : EditorWindow {
    public static SoundSettingEditor instance { get; private set; }
    private SoundScriptable soundScriptable = null;
    private const string ASSET_PATH = "Assets/Resources/SoundData.asset";

    public List<BGMData> bgmList = new List<BGMData>();
    public List<bool> bgmActiveList = new List<bool>();

    public List<SEData> seList = new List<SEData>();
    public List<bool> seActiveList = new List<bool>();

    public Color defaultColor { get; private set; }
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 bgmScrollPosition = Vector2.zero;
    private Vector2 seScrollPosition = Vector2.zero;

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
        bgmActiveList.Clear();
        for(int i = 0; i< bgmList.Count; i++)
        {
            bgmActiveList.Add(false);
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
        Layout();
    }

    private void Layout()
    {
        if (soundScriptable == null)
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
                EditorGUILayout.LabelField("BGM設定");
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    using (new GUILayout.HorizontalScope(GUI.skin.box))
                    {
                        if (GUILayout.Button("追加", GUILayout.Width(120)))
                        {
                            AddBGM();
                        }
                    }

                    bgmScrollPosition = EditorGUILayout.BeginScrollView(bgmScrollPosition, GUI.skin.box);
                    {
                        BGMDataSettingLayout();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(30);
                EditorGUILayout.LabelField("SE設定");
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    using (new GUILayout.HorizontalScope(GUI.skin.box))
                    {
                        if (GUILayout.Button("追加", GUILayout.Width(120)))
                        {
                            AddSE();
                        }
                    }

                    seScrollPosition = EditorGUILayout.BeginScrollView(seScrollPosition, GUI.skin.box);
                    {
                        SEDataSettingLayout();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void BGMDataSettingLayout()
    {
        for(int findID = 0; findID < bgmList.Count; findID++)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label((findID + 1).ToString());
            }
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    string s = bgmActiveList[findID] ? "-" : "+";
                    if (GUILayout.Button(s, GUILayout.Width(20)))
                    {
                        bgmActiveList[findID] = !bgmActiveList[findID];
                    }

                    if (bgmActiveList[findID])
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            bgmList[findID].name = EditorGUILayout.TextField("ID（名前）", bgmList[findID].name);
                            bgmList[findID].audio = EditorGUILayout.ObjectField("Sound", bgmList[findID].audio, typeof(AudioClip), false) as AudioClip;
                            bgmList[findID].isLoop = EditorGUILayout.Toggle("ループ", bgmList[findID].isLoop);
                            if (bgmList[findID].isLoop)
                            {
                                bgmList[findID].loopBeginTime = EditorGUILayout.FloatField("ループ再生開始地点", bgmList[findID].loopBeginTime);
                                bgmList[findID].loopEndTime = EditorGUILayout.FloatField("ループ地点", bgmList[findID].loopEndTime);
                            }
                            GUILayout.Space(30);
                            if (GUILayout.Button("削除", GUILayout.Width(120)))
                            {
                                DeleteBGM(findID);
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
    private void AddBGM()
    {
        bgmList.Add(new BGMData());
        bgmActiveList.Add(false);
    }
    private void DeleteBGM(int findID)
    {
        bgmList.RemoveAt(findID);
        bgmActiveList.RemoveAt(findID);
    }

    private void SEDataSettingLayout()
    {
        for (int findID = 0; findID < seList.Count; findID++)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label((findID + 1).ToString());
            }
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    string s = seActiveList[findID] ? "-" : "+";
                    if (GUILayout.Button(s, GUILayout.Width(20)))
                    {
                        seActiveList[findID] = !seActiveList[findID];
                    }

                    if (seActiveList[findID])
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            seList[findID].name = EditorGUILayout.TextField("ID（名前）", seList[findID].name);
                            seList[findID].audio = EditorGUILayout.ObjectField("Sound", seList[findID].audio, typeof(AudioClip), false) as AudioClip;
                            seList[findID].isPlayOneShot = EditorGUILayout.Toggle("PlayOneShotで再生", seList[findID].isPlayOneShot);
                            
                            GUILayout.Space(30);
                            if (GUILayout.Button("削除", GUILayout.Width(120)))
                            {
                                DeleteSE(findID);
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
    private void AddSE()
    {
        seList.Add(new SEData());
        seActiveList.Add(false);
    }
    private void DeleteSE(int findID)
    {
        seList.RemoveAt(findID);
        seActiveList.RemoveAt(findID);
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

        bgmList.Clear();
        bgmActiveList.Clear();
        for (int i = 0; i < soundScriptable.bgmDatas.Count; i++)
        {
            bgmList.Add(soundScriptable.bgmDatas[i]);
            bgmActiveList.Add(false);
        }
        seList.Clear();
        seActiveList.Clear();
        for(int i = 0; i < soundScriptable.seDatas.Count; i++)
        {
            seList.Add(soundScriptable.seDatas[i]);
            seActiveList.Add(false);
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
        soundScriptable.SetSoundDatas(bgmList,seList);
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

