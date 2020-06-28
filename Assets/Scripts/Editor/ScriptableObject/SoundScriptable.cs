using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScriptable : ScriptableObject {

    [SerializeField] public List<SoundData> soundDatas = new List<SoundData>();

    public void SetSoundDatas(List<SoundData> setDatas)
    {
        soundDatas.Clear();
        soundDatas.AddRange(setDatas);
    }

    private int intValue = 0;
    public int IntValue
    {
        get { return intValue; }
#if UNITY_EDITOR
        set { intValue = value; }
#endif
    }
#if UNITY_EDITOR
    public void Copy(SoundScriptable data)
    {
        intValue = data.IntValue;
    }
#endif
}
[System.Serializable]
public class SoundData
{
    public string name;
    public AudioClip audio;
    public bool isLoop;
    public float loopBeginTime;//ループして再生を開始する地点
    public float loopEndTime;//ループするタイミング
}