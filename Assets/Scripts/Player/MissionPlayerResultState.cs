using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPlayerResultState : MissionPlayerStateBase {

    public override void StateBeginAction()
    {
        _playerController.SlashEffect.EndSlashEffect();
        _playerController.PlayerRotation.OnPointerUP();
    }

    public override void StateEndAction()
    {

    }

    public override void StateActionUpdate()
    {

    }
}
