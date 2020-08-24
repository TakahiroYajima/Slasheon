using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        base.enemyController.StartCoroutine(CurveLowFlightAttack(1f,()=>{
            enemyController.MyAnimator.SetInteger(enemyController.AnimationID, 2);
        },
        () => {

        },()=> {
            enemyController.MyAnimator.SetInteger(enemyController.AnimationID, 0);
        }));
    }

    /// <summary>
    /// 一旦遠くへ離れ、曲線の起動でプレイヤーへ突進する攻撃
    /// </summary>
    /// <returns></returns>
    public IEnumerator CurveLowFlightAttack(float actionTime = 1f, UnityAction beginCallback = null,UnityAction beginAttackCallback = null, UnityAction endCallback = null)
    {
        Vector3 initPos = enemyController.transform.position;
        Vector3 playerPos = MissionSceneManager.Instance.playerPosition;
        playerPos -= (initPos - playerPos).normalized * 2f;
        playerPos.x += 2f;

        float elapsedTime = 0f;
        float distance = Vector3.Distance(initPos, playerPos);
        Vector3 direction = initPos - playerPos;
        if (beginCallback != null)
        {
            beginCallback();
        }

        bool beginAttackCallbackPlayed = false;
        Vector3 middlePos = new Vector3(initPos.x - 30f, initPos.y + 3f, initPos.z + (initPos.z - playerPos.z) * 5f);

        while (elapsedTime < actionTime)
        {
            if (damageFlg)
            {
                elapsedTime = actionTime;
            }
            if (!beginAttackCallbackPlayed && beginAttackCallback != null)
            {
                beginAttackCallback();
            }
            Vector3 currentPos = GetBezierCurvePoint2P(initPos, middlePos, playerPos, elapsedTime / actionTime);
            enemyController.transform.position = currentPos;
            //Debug.Log("current : " + currentPos + " : " + playerPos);
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= actionTime)
            {
                enemyController.transform.position = playerPos;
                if (!damageFlg)
                {
                    enemyController.AttackAction();
                    MissionSceneManager.Instance.Player.Damage(1);
                }
            }
            yield return null;
        }
        if (endCallback != null)
        {
            endCallback();
        }
        enemyController.StartCoroutine(MoveToInitPos());
    }

    public Vector3 GetBezierCurvePoint2P(Vector3 startPoint,Vector3 middlePoint, Vector3 goalPoint, float t)
    {
        var a = Vector3.Lerp(startPoint, middlePoint, t);
        var b = Vector3.Lerp(middlePoint, goalPoint, t);

        return Vector3.Lerp(a, b, t);
    }
}
