using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class SaveManager : SingletonMonoBehaviour<SaveManager> {

    protected override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        base.Awake();
    }

    public void SaveUserStatus(SaveDataUserStatus saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        Save("PlayerStatus", json);
    }
    public SaveDataUserStatus ReadUserStatus()
    {
        string json = Read("PlayerStatus");
        return JsonUtility.FromJson<SaveDataUserStatus>(json);
    }

    public void Save(string key, string json)
    {
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
    public string Read(string key)
    {
        return PlayerPrefs.GetString(key);
    }
}

[Serializable]
public class SaveDataUserStatus
{
    public int playerLevel;
    public string staminaMaxRecoverDateTime;//スタミナが最大まで回復する日付(yyyy/MM/dd/hh/mm/ss)
}