using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ミッション内、プレイヤーが探索モード時のアクション
/// </summary>
public class MissionPlayerExpeditionState : MissionPlayerStateBase {


    public override void StateBeginAction()
    {

    }

    public override void StateEndAction()
    {
        
    }

    public override void StateActionUpdate()
    {
        _playerController.ExpeditionAction();
    }
}
