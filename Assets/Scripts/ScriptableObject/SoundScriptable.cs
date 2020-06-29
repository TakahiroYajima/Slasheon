using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScriptable : ScriptableObject {

    [SerializeField] public List<BGMData> bgmDatas = new List<BGMData>();
    [SerializeField] public List<SEData> seDatas = new List<SEData>();

    public void SetSoundDatas(List<BGMData> setBGMData, List<SEData> setSEData)
    {
        bgmDatas.Clear();
        bgmDatas.AddRange(setBGMData);
        seDatas.Clear();
        seDatas.AddRange(setSEData);
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
