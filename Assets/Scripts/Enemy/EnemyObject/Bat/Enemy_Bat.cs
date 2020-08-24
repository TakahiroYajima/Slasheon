using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// コウモリ型の敵のコントローラー
/// </summary>
public class Enemy_Bat : MissionEnemyController {

    
    private const EnemyType myEnemyType = EnemyType.Bat;

    protected override void InitializePrivateSetting()
    {
        actorState.hp =10;
        initActorState.hp = 10;

        animationID = "AnimationID";
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
        if (nowActionState == enemyStatus[EnemyState.Death.ToString()])
        {
            return;
        }
        //アニメーションの情報取得
        AnimatorClipInfo[] clipInfo = myAnimator.GetCurrentAnimatorClipInfo(0);

        // 再生中のクリップ名
        string clipName = clipInfo[0].clip.name;
        if (clipName == "HIT01" || clipName == "HIT02")
        {
            myAnimator.SetInteger(animationID, 0);
        }
    }
    public override void AttackAction()
    {

    }
    public override void DamageAction()
    {
        int rand = Random.Range(4, 6);

        myAnimator.SetInteger(animationID, rand);
    }
    public override void DeathAction()
    {
        myAnimator.SetInteger(animationID, 6);
    }
}
