using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageScriptable : ScriptableObject
{
    [SerializeField] public List<StageData> stageDatas = new List<StageData>();

    public StageData Find(string targetKey)
    {
        var find = stageDatas.Where(x => x.key == targetKey).ToList();
        if(find.Count > 0)
        {
            return find[0];
        }
        else
        {
            return null;
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
    public void SetStageDatas(List<StageData> setStageData)
    {
        stageDatas.Clear();
        foreach (var stage in setStageData)
        {
            stageDatas.Add(stage);
        }
    }
    public void Copy(StageScriptable data)
    {
        intValue = data.IntValue;
    }
#endif
}

[System.Serializable]
public class StageData
{
    public string key;
    public GameObject prefab;
    public List<OnStageEnemy> enemys;
}

[System.Serializable]
public class OnStageEnemy
{
    public Vector3 initPosition;//初期位置
    public float initAngle;//向いている方向
    public EnemyType enemyType;//種別
    public bool isBoss;//ボスであるか
}