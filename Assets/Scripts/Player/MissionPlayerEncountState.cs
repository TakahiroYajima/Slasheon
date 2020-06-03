using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPlayerEncountState : MissionPlayerStateBase {


    public override void StateBeginAction()
    {
        _playerController.EncountBeginAction();
    }

    public override void StateEndAction()
    {

    }

    public override void StateActionUpdate()
    {
        _playerController.EncountAction();
    }
}
