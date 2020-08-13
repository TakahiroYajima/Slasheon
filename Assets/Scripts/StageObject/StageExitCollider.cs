using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageExitCollider : MonoBehaviour {

    [SerializeField] private StageExitType stageExitType = StageExitType.ExitMission;
    public StageExitType StageExitType { get { return stageExitType; } }

    [SerializeField] private string nextStageKey = "";//遷移先のステージデータのキー
    public string TargetKey { get { return nextStageKey; } }

    [SerializeField] private Transform startPlayerPosition = null;

    public delegate void TriggerEnterCallback(Collider collider, StageExitCollider script);
    public TriggerEnterCallback triggerEnterCallback;

	// Use this for initialization
	void Start ()
    {

    }

    public void Initialize()
    {
        
    }

    public Transform GetInitPlayerTransform()
    {
        return startPlayerPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnterCallback(other, this);
    }
}

public enum StageExitType
{
    FieldChange,//フィールド移動
    ToHome,//ホーム画面へ帰還する
    ExitMission,//ミッションクリア(出口)
}