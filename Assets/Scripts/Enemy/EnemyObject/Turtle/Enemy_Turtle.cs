using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Turtle : MissionEnemyController {

    [SerializeField] private Animator myAnimator = null;
    private const EnemyType myEnemyType = EnemyType.Turtle;
    private string animationID = "AnimationID";

    protected override void InitializePrivateSetting()
    {
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
}
