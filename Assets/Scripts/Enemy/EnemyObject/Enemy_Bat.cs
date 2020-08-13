using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Enemy_Bat : MissionEnemyController {

    [SerializeField] private Animator myAnimator = null;
    private const EnemyType myEnemyType = EnemyType.Bat;
    private string animationID = "AnimationID";

    protected override void InitializePrivateSetting()
    {
        
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
