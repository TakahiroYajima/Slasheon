using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPlayerBattleState : MissionPlayerStateBase {

    private int slashTouchID = -1;


    public override void StateBeginAction()
    {
        
    }

    public override void StateEndAction()
    {

    }

    public override void StateActionUpdate()
    {
        int touchID = InputManager.Instance.GetAnyTouchBeginID();
        if (touchID != -1)
        {
            if (!InputManager.Instance.IsUITouch(touchID))
            {
                slashTouchID = touchID;
            }
        }
        if (slashTouchID != -1)
        {
            _playerController.SlashEffect.UpdateAction();
            if (InputManager.Instance.IsTouchEnd(slashTouchID))
            {
                slashTouchID = -1;
            }
        }
    }
}
