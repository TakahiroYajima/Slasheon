using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager> {

    private bool isBootInitialezed = false;
    public bool IsBootInitialized { get { return isBootInitialezed; } }

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }
    /// <summary>
    /// 初期化　BootSceneManagerのみ呼び出し許可
    /// </summary>
    public void Initialize()
    {
        isBootInitialezed = true;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
#if Develop
     public void OnLoadBootSceneDebugMode(string backSceneName)
    {
        SceneControllManager.Instance.ChangeScene("BootScene");
    }   
#endif
}

public enum MissionState
{
    Initialize,//初期化時
    Start,//開始時
    Expedition,//探索
    Encount,//エンカウント時
    Battle,//バトル
    Result,//バトルリザルト
    GameOver,//敗北時
}