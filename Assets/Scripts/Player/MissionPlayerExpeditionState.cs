using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ミッション内、プレイヤーが探索モード時のアクション
/// </summary>
public class MissionPlayerExpeditionState : MissionPlayerStateBase {

    private float raycastDistance = 30f;

    public override void StateBeginAction()
    {

    }

    public override void StateEndAction()
    {
        
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
    }

    private Vector3 TouchActionOnField()
    {
        if (InputManager.Instance.IsTouchEnd())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetTouchPosition(1));
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
        return Vector3.zero;
    }
}
