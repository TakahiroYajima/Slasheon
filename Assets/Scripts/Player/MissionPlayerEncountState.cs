using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPlayerEncountState : MissionPlayerStateBase {


    public override void StateBeginAction()
    {
        _playerController.MovePropertyReset();
        //scenemanagerから最初に登録された敵の位置を取得し、yだけプレイヤーの位置にしてその方向へ一定時間で向く処理。向き終わったらコールバックで終了処理
        _playerController.StartCoroutine(_playerController.RotationToTargetInTime(MissionSceneManager.Instance.GetFirstEncountEnemy().transform.position, 1f,()=> { MissionSceneManager.Instance.ChangeMissionState(MissionState.Battle); }));
    }

    public override void StateEndAction()
    {

    }

    public override void StateActionUpdate()
    {
        //scenemanagerから最初に登録された敵の位置を取得し、yだけプレイヤーの位置にしてその方向へ一定時間で向く処理。向き終わったら終了処理
    }
}
