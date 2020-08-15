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

    public void DataSave<Types>(Types saveClass, string key)
    {
        //Debug.Log("Save : " + key);
        string json = JsonUtility.ToJson(saveClass);
        Save(key, json);
    }
    public Types ReadSaveData<Types>(string key)
    {
        //Debug.Log("Read : " + key);
        string json = Read(key);
        return JsonUtility.FromJson<Types>(json);
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

public class SaveKeys
{
    public const string UserStatusKey = "PlayerStatus";
}