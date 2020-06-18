using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPlayerResultState : MissionPlayerStateBase {

    public override void StateBeginAction()
    {
        _playerController.SlashEffect.EndSlashEffect();
    }

    public override void StateEndAction()
    {

    }

    public override void StateActionUpdate()
    {

    }
}
