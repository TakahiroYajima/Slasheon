using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ミッション内、プレイヤーが探索モード時のアクション
/// </summary>
public class MissionPlayerExpeditionState : MissionPlayerStateBase {

    private float raycastDistance = 30f;
    private int moveTouchID = -1;

    public override void StateBeginAction()
    {

    }

    public override void StateEndAction()
    {
        moveTouchID = -1;
    }

    public override void StateActionUpdate()
    {
        _playerController.RecoverStamina();

        Vector3 targetPos = TouchActionOnField();
        if (targetPos != Vector3.zero)
        {
            _playerController.moveTargetPos = targetPos;
        }
        if (_playerController.isMoving)
        {
            _playerController.MoveOnField(_playerController.moveTargetPos);
        }
        else
        {
            _playerController.RotationViewAction();
        }
    }

    private Vector3 TouchActionOnField()
    {
        int touchID = InputManager.Instance.GetAnyTouchBeginID();
        if (touchID != -1)
        {
            if (!InputManager.Instance.IsUITouch(touchID))
            {
                moveTouchID = touchID;
            }
        }
        if (moveTouchID != -1)
        {
            if (InputManager.Instance.IsTouchEnd(moveTouchID))
            {
                moveTouchID = -1;
                Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetTouchPosition(moveTouchID));
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, raycastDistance))
                {
                    if (SlasheonUtility.IsLayerNameMatch(hit.collider.gameObject, "Field"))
                    {
                        _playerController.moveBeginPosition = _playerController.transform.position;
                        _playerController.isMoving = true;
                        return hit.point;
                    }
                }
            }
        }
        return Vector3.zero;
    }
}
