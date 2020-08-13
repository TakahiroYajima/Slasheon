using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour {

    [SerializeField] private List<StageExitCollider> exitColliders = new List<StageExitCollider>();
    [SerializeField] private Transform defaultPlayerInitPositionTransform = null;

	// Use this for initialization
	void Start () {
		
	}
	
	public void Initialize()
    {
        foreach(var exit in exitColliders)
        {
            exit.triggerEnterCallback = ExitColliderEnterEvent;
        }

        //プレイヤーの初期位置を設定
        Transform playerPositionTrans = null;
        if (MissionSceneManager.Instance.prevStageID != "") {
            var trans = exitColliders.Select(x => x).Where(x => x.TargetKey == MissionSceneManager.Instance.prevStageID).ToList();
            if(trans.Count > 0)
            {
                playerPositionTrans = trans[0].GetInitPlayerTransform();
            }
            else
            {
                playerPositionTrans = defaultPlayerInitPositionTransform;
            }
        }
        else
        {
            playerPositionTrans = defaultPlayerInitPositionTransform;
        }
        MissionSceneManager.Instance.Player.transform.position = playerPositionTrans.position;
    }

    public void ExitColliderEnterEvent(Collider other, StageExitCollider script)
    {
        if (SlasheonUtility.IsLayerNameMatch(other.gameObject, "Player"))
        {
            switch (script.StageExitType)
            {
                case StageExitType.FieldChange:
                    StartCoroutine(MissionSceneManager.Instance.ChangeStage(script.TargetKey));
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
