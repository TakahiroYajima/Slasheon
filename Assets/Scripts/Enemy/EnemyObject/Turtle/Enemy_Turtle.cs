using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Turtle : MissionEnemyController {

    private const EnemyType myEnemyType = EnemyType.Turtle;

    protected override void InitializePrivateSetting()
    {
        animationID = "AnimationID";
        enemyStatus = new Dictionary<string, MissionEnemyStateBase>
        {
            {EnemyState.Expedition.ToString(),new MissionEnemyExpeditionState() },
            {EnemyState.Encount.ToString(),new MissionEnemyEncountState() },
            {EnemyState.Battle.ToString(),new Enemy_Turtle_BattleState() },
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
        if (clipName == "Armature|hit1" || clipName == "Armature|hit2")
        {
            myAnimator.SetInteger(animationID, 0);
        }
    }
    public override void AttackAction()
    {
        
    }
    public override void DamageAction()
    {
        int rand = Random.Range(14, 16);

        myAnimator.SetInteger(animationID, rand);
    }
    public override void DeathAction()
    {
        myAnimator.SetInteger(animationID, 20);
    }
}
