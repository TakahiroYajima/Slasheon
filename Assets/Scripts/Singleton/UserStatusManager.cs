using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class UserStatusManager : SingletonMonoBehaviour<UserStatusManager> {

    public float currentStamina { get; private set; }
    public float maxStamina { get; private set; }
    public float recoverOneStaminaSecond { get; private set; }//スタミナが1回復するまでの時間（秒で計算）
    public float oneStaminaRecoverTimeElapsed { get; private set; }//経過時間
    public float nextRecoverTime { get; private set; }//次のポイントまで回復する時間

    private float prevElapsedTimeSecond = 0f;
    private float prevRealTime = 0f;

    public delegate void StaminaRecoverCallback(float max,float current);
    private StaminaRecoverCallback staminaRecoverCallback;
    public delegate void UpdateTimeCallback(float remainingTime);
    private UpdateTimeCallback updateTimeCallback;

    protected override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        currentStamina = 0f;
        maxStamina = 10f;
        oneStaminaRecoverTimeElapsed = 0f;
        nextRecoverTime = 10f;//とりあえず10秒

        BeginAction();

        EditorApplication.playmodeStateChanged += UnityEditor_PlayStateChangeAction;
	}

    public void SetStaminaRecoverCallback(StaminaRecoverCallback callback)
    {
        staminaRecoverCallback = callback;

    }
    public void SetUpdateTimeCallback(UpdateTimeCallback callback)
    {
        updateTimeCallback = callback;
    }

    public void UpdateAction()
    {
        if(currentStamina < maxStamina)
        {
            if (oneStaminaRecoverTimeElapsed < nextRecoverTime)
            {
                oneStaminaRecoverTimeElapsed = Time.realtimeSinceStartup - prevRealTime;
                //Debug.Log(oneStaminaRecoverTimeElapsed);
                if(Mathf.Floor(oneStaminaRecoverTimeElapsed) - prevElapsedTimeSecond >= 1f)
                {
                    //Debug.Log("updateTime : " + oneStaminaRecoverTimeElapsed);
                    prevElapsedTimeSecond = Mathf.Floor(oneStaminaRecoverTimeElapsed);
                    updateTimeCallback(nextRecoverTime - prevElapsedTimeSecond);
                }
            }
            else
            {
                oneStaminaRecoverTimeElapsed = 0f;
                prevElapsedTimeSecond = 0f;
                currentStamina += 1f;
                staminaRecoverCallback(maxStamina,currentStamina);
                prevRealTime = Time.realtimeSinceStartup;
            }
        }
    }

    public void BeginAction()
    {
        //セーブデータを取り出してスタミナ回復判定
        if (PlayerPrefs.HasKey("PlayerStatus"))
        {
            SaveDataUserStatus userStatus = SaveManager.Instance.ReadUserStatus();
            DateTime recoverDate = DateTime.Parse(userStatus.staminaMaxRecoverDateTime);
            Debug.Log("recoverDate : " + recoverDate);
            //
        }
    }

    public void PauseAction(bool pause)
    {
        if (pause)
        {
            Debug.Log("ポーズ");
            //ポーズ
            SaveDataUserStatus userStatus = new SaveDataUserStatus();
            userStatus.playerLevel = 1;
            if (currentStamina < maxStamina)
            {
                //スタミナ全回復までの時間を計算
                DateTime nowDateTime = DateTime.Now;
                Debug.Log("現在 : " + nowDateTime.ToString());
                int plusSecond = Mathf.FloorToInt(((maxStamina - currentStamina) * oneStaminaRecoverTimeElapsed));
                TimeSpan timeSpan = new TimeSpan(0, 0, 0, plusSecond);
                Debug.Log("追加秒数 : " + plusSecond.ToString());
                //nowDateTime.AddSeconds(plusSecond);
                nowDateTime += timeSpan;
                userStatus.staminaMaxRecoverDateTime = nowDateTime.ToString();
                Debug.Log("セーブ : " + userStatus.staminaMaxRecoverDateTime);
            }
            else
            {
                userStatus.staminaMaxRecoverDateTime = "";
            }
            SaveManager.Instance.SaveUserStatus(userStatus);
        }
        else
        {
            //復帰

        }
    }

    public void AppQuitAction()
    {
        SaveDataUserStatus userStatus = new SaveDataUserStatus();
        userStatus.playerLevel = 1;
        if (currentStamina < maxStamina)
        {
            //スタミナ全回復までの時間を計算
            DateTime nowDateTime = DateTime.Now;
            double plusSecond = (double)((maxStamina - currentStamina) * oneStaminaRecoverTimeElapsed);
            nowDateTime.AddSeconds(plusSecond);
            userStatus.staminaMaxRecoverDateTime = nowDateTime.ToString();
            Debug.Log(nowDateTime.ToString());
        }
        else
        {
            userStatus.staminaMaxRecoverDateTime = "";
        }
        SaveManager.Instance.SaveUserStatus(userStatus);
    }

    private void OnApplicationPause(bool pause)
    {
        PauseAction(pause);
    }

    public void UnityEditor_PlayStateChangeAction()
    {
        if (!EditorApplication.isPlaying)
        {
            //再生停止したらデータセーブさせてみる
            //AppQuitAction();
        }else if (EditorApplication.isPaused)
        {
            PauseAction(true);
        }
    }
}
