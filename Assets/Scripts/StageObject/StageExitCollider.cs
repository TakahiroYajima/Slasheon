using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageExitCollider : MonoBehaviour {

    [SerializeField] private StageExitType stageExitType = StageExitType.ExitMission;
    public StageExitType StageExitType { get { return stageExitType; } }

    [SerializeField] private string nextStageKey = "";//遷移先のステージデータのキー
	// Use this for initialization
	void Start ()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (SlasheonUtility.IsLayerNameMatch(other.gameObject, "Player"))
        {
            switch (stageExitType)
            {
                case StageExitType.FieldChange:

                    break;
                case StageExitType.ToHome:
                    SceneControllManager.Instance.ChangeSceneAsync("HomeScene", true, true, true);
                    break;
                case StageExitType.ExitMission:
                    SceneControllManager.Instance.ChangeSceneAsync("HomeScene", true, true, true);
                    break;
                default: break;
            }
        }
    }
}

public enum StageExitType
{
    FieldChange,//フィールド移動
    ToHome,//ホーム画面へ帰還する
    ExitMission,//ミッションクリア(出口)
}