using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bat_BattleState : MissionEnemyBattleState {

    public override void StateBeginAction()
    {
        base.StateBeginAction();
    }
    public override void StateEndAction()
    {
        base.StateEndAction();
    }
    public override void StateUpdateAction()
    {
        base.StateUpdateAction();
    }
    /// <summary>
    /// 攻撃時の処理
    /// </summary>
    public override void AttackAction()
    {
        base.enemyController.StartCoroutine(AttackToPlayerAction());
    }
}
