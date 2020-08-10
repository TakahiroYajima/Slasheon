using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScriptable : ScriptableObject
{
    [SerializeField] public Dictionary<string, GameObject> stageDatas = new Dictionary<string, GameObject>();

    public void SetStageDatas(Dictionary<string,GameObject> setStageData)
    {
        stageDatas.Clear();
        foreach(var stage in setStageData)
        {
            stageDatas.Add(stage.Key, stage.Value);
        }
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
    public void Copy(StageScriptable data)
    {
        intValue = data.IntValue;
    }
#endif
}
