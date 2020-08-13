using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyBattleState : MissionEnemyStateBase
{

    private float elapsedToAttackTime = 0f;
    private float attackBeginTime = 5f;

    public EnemyBattleState battleState = EnemyBattleState.Waiting;
    public Vector3 battleInitPos;

    private bool damageFlg = false;

    /// <summary>
    /// このステートになった瞬間のアクション
    /// </summary>
    public override void StateBeginAction()
    {
        enemyController.SetEnemyDamageCallback(DamageCallback);
        battleInitPos = enemyController.transform.position;
    }

    /// <summary>
    /// ステート切り替え時、切り替わる前のステートの終了アクション
    /// </summary>
	public override void StateEndAction()
    {

    }

    /// <summary>
    /// このステートでの毎フレーム更新処理
    /// </summary>
    public override void StateUpdateAction()
    {
        enemyController.BattleAction();
        if(elapsedToAttackTime >= attackBeginTime)
        {
            battleState = EnemyBattleState.Attack;
            enemyController.StartCoroutine(AttackToPlayerAction());
            elapsedToAttackTime = 0f;
        }
        if (battleState == EnemyBattleState.Waiting)
        {
            elapsedToAttackTime += Time.deltaTime;
        }
    }

    public IEnumerator AttackToPlayerAction()
    {
        //Debug.Log("EnemyAttack");
        Vector3 initPos = enemyController.transform.position;
        Vector3 playerPos = MissionSceneManager.Instance.playerPosition;
        playerPos -= (initPos - playerPos).normalized * 1.2f;

        float actionTime = 1f;
        float elapsedTime = 0f;
        float distance = Vector3.Distance(initPos, playerPos);
        Vector3 direction = initPos - playerPos;
        while(elapsedTime < actionTime)
        {
            if (damageFlg)
            {
                elapsedTime = actionTime;
            }
            Vector3 currentPos = enemyController.transform.position - direction * (Time.deltaTime / actionTime);
            enemyController.transform.position = currentPos;
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= actionTime)
            {
                if (!damageFlg)
                {
                    enemyController.AttackAction();
                    MissionSceneManager.Instance.Player.Damage(1);
                }
            }
            yield return null;
        }
        
        enemyController.StartCoroutine(MoveToInitPos());
    }

    public IEnumerator MoveToInitPos()
    {
        yield return null;
        battleState = EnemyBattleState.Waiting;
        enemyController.transform.position = battleInitPos;
        damageFlg = false;
    }

    public void DamageCallback()
    {
        damageFlg = true;
        enemyController.StopCoroutine(AttackToPlayerAction());
        enemyController.StartCoroutine(MoveToInitPos());
    }
}

public enum EnemyBattleState
{
    Waiting,//待機
    Attack,
}