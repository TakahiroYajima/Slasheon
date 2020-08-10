using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        currentStamina = 10f;
        maxStamina = 20f;
        oneStaminaRecoverTimeElapsed = 0f;
        nextRecoverTime = 10f;//とりあえず10秒
#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged += UnityEditor_PlayStateChangeAction;
#endif
    }

    public void SetStaminaRecoverCallback(StaminaRecoverCallback callback)
    {
        staminaRecoverCallback = callback;

    }
    public void SetUpdateTimeCallback(UpdateTimeCallback callback)
    {
        updateTimeCallback = callback;
    }

    /// <summary>
    /// スタミナ消費
    /// </summary>
    /// <param name="minus"></param>
    public void ConsumptionStaminaOrError(float minus, UnityAction intendedCallback)
    {
        if(currentStamina - minus < 0)
        {
            Debug.Log("マイナス値に入っています");
        }
        else
        {
            currentStamina -= minus;
            Debug.Log("正常にスタミナ消費 残りスタミナ : " + currentStamina + " : max : " + maxStamina);
            intendedCallback();
            SaveUserStatusData();
        }
    }

    /// <summary>
    /// 毎フレーム更新(スタミナ回復)
    /// </summary>
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

    public void RecoverStaminaSaveData(UnityAction<float,float>updateUICallback)
    {
        //セーブデータを取り出してスタミナ回復判定
        if (PlayerPrefs.HasKey(SaveKeys.UserStatusKey))
        {
            SaveDataUserStatus userStatus = SaveManager.Instance.ReadSaveData<SaveDataUserStatus>(SaveKeys.UserStatusKey);
            if (!string.IsNullOrEmpty(userStatus.staminaMaxRecoverDateTime))
            {
                DateTime recoverDate = DateTime.Parse(userStatus.staminaMaxRecoverDateTime);
                Debug.Log("recoverDate : " + recoverDate);
                DateTime nowDate = DateTime.Now;
                TimeSpan timeSpan = recoverDate - nowDate;
                double totalSecond = timeSpan.TotalSeconds;
                Debug.Log("timespan : " + timeSpan + "total : " + totalSecond);
                int totalFloor = Mathf.Abs(Mathf.FloorToInt((float)totalSecond));
                float recoverStamina = totalFloor / nextRecoverTime;
                Debug.Log("recoverStamina : " + recoverStamina);

                currentStamina += Mathf.Floor(recoverStamina);
                if(currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
                updateUICallback(maxStamina, currentStamina);
            }
        }
    }

    public void SaveUserStatusData()
    {
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
            nowDateTime += timeSpan;
            userStatus.staminaMaxRecoverDateTime = nowDateTime.ToString();
            Debug.Log("セーブ : " + userStatus.staminaMaxRecoverDateTime);

            //MobileNotificationManager.Instance.SetNotification("タイトル", "スタミナ回復通知", "", "", DateTime.Now);
        }
        else
        {
            userStatus.staminaMaxRecoverDateTime = "";
        }
        SaveManager.Instance.DataSave<SaveDataUserStatus>(userStatus, SaveKeys.UserStatusKey);
    }

    public void PauseAction(bool pause)
    {
        if (pause)
        {
            //Debug.Log("ポーズ");
        }
        else
        {

            //復帰

        }
    }

    public void AppQuitAction()
    {

    }

    private void OnApplicationPause(bool pause)
    {
        PauseAction(pause);
    }
#if UNITY_EDITOR
    public void UnityEditor_PlayStateChangeAction()
    {
        if (!EditorApplication.isPlaying)
        {

        }
        else if (EditorApplication.isPaused)
        {
            PauseAction(true);
        }
        else if (!EditorApplication.isPaused)
        {
            PauseAction(false);

        }
    }
#endif
}
