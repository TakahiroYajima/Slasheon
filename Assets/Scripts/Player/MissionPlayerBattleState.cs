﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionPlayerBattleState : MissionPlayerStateBase {

    private int slashTouchID = -1;
    private float slashRayDistance = 10f;
    private List<MissionActor> slashHitEnemyList = new List<MissionActor>();

    public override void StateBeginAction()
    {
        slashHitEnemyList.Clear();
        _playerController.SlashEffect.SetSlashEndCallback(() =>
        {
            slashHitEnemyList.Clear();
        });
    }

    public override void StateEndAction()
    {
        slashHitEnemyList.Clear();
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
            Vector2 touchPos = InputManager.Instance.GetTouchPosition(slashTouchID);
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetTouchPosition(slashTouchID));
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, slashRayDistance))
            {
                if (SlasheonUtility.IsLayerNameMatch(hit.collider.gameObject, "Enemy"))
                {
                    MissionActor hitActor = hit.collider.gameObject.GetComponent<MissionActor>();
                    //一度の斬撃で複数回ダメージ判定しないように調整(linq適用前)
                    //bool cantDamage = false;
                    //for (int i = 0; i < slashHitEnemyList.Count; i++)
                    //{
                    //    if(slashHitEnemyList[i].transform.GetInstanceID() == hitActor.transform.GetInstanceID())
                    //    {
                    //        cantDamage = true;
                    //        break;
                    //    }
                    //}
                    //if (!cantDamage)
                    //{
                    //    hitActor.Damage(_playerController.PlayerActorState.attack);
                    //    slashHitEnemyList.Add(hitActor);
                    //}

                    //一度の斬撃で複数回ダメージ判定しないように調整
                    int hitedCount = slashHitEnemyList.Where(x => x.transform.GetInstanceID() == hitActor.transform.GetInstanceID()).Count();
                    if(hitedCount == 0)
                    {
                        hitActor.Damage(_playerController.PlayerActorState.attack);
                        slashHitEnemyList.Add(hitActor);
                    }
                }
            }

            if (InputManager.Instance.IsTouchEnd(slashTouchID))
            {
                slashTouchID = -1;
            }
        }
    }
}
