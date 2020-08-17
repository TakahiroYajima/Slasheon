using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// コウモリ型の敵のコントローラー
/// </summary>
public class Enemy_Bat : MissionEnemyController {

    [SerializeField] private Animator myAnimator = null;
    private const EnemyType myEnemyType = EnemyType.Bat;
    private string animationID = "AnimationID";

    protected override void InitializePrivateSetting()
    {
        enemyStatus = new Dictionary<string, MissionEnemyStateBase>
        {
            {EnemyState.Expedition.ToString(),new MissionEnemyExpeditionState() },
            {EnemyState.Encount.ToString(),new MissionEnemyEncountState() },
            {EnemyState.Battle.ToString(),new Enemy_Bat_BattleState() },
            {EnemyState.Death.ToString(),new MissionEnemyDeathState() },
        };
        foreach (var state in enemyStatus)
        {
            state.Value.Initialize(this);
        }
        ChangeState(EnemyState.Expedition);
    }

    public override void BattleAction()
    {
        //if(myAnimator.GetInteger(animationID) == 4)
        //{
        //    myAnimator.SetInteger(animationID, 0);
        //}
    }
    public override void AttackAction()
    {

    }
    public override void DamageAction()
    {
        //myAnimator.SetInteger(animationID, 4);
    }
    public override void DeathAction()
    {
        myAnimator.SetInteger(animationID, 6);
    }
}
