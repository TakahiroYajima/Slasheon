using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyExpeditionState : MissionEnemyStateBase {

    /// <summary>
    /// このステートになった瞬間のアクション
    /// </summary>
    public override void StateBeginAction()
    {

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
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        if (MissionSceneManager.Instance.NowMissionState != MissionState.Result)
        {
            Vector3 playerPos = MissionSceneManager.Instance.playerPosition;
            Vector3 targetPos = new Vector3(playerPos.x, enemyController.transform.position.y, playerPos.z);
            Vector3 moveDirection = (enemyController.transform.position - targetPos).normalized;
            if (MissionSceneManager.Instance.encountEnemyList.Count > 0)
            {
                foreach(var enemy in MissionSceneManager.Instance.encountEnemyList)
                {
                    float distance = Vector3.Distance(enemy.transform.position, enemyController.transform.position);
                    if(distance < enemy.AllowableApproachDistance)
                    {
                        Vector3 controllerPos = new Vector3(enemyController.transform.position.x, 0f, enemyController.transform.position.z);
                        Vector3 otherEnemyPos = new Vector3(enemy.transform.position.x,0f, enemy.transform.position.z);
                        moveDirection = Quaternion.Euler(0f, -120f, 0f) * (controllerPos - otherEnemyPos).normalized;
                    }
                }
            }
            if (Vector3.Distance(targetPos, enemyController.transform.position) > enemyController.EncountPlayerDistance)
            {
                enemyController.transform.LookAt(enemyController.transform.position - moveDirection);
                enemyController.transform.Translate(moveDirection * enemyController.MoveSpeed * Time.deltaTime);
            }
            else
            {
                enemyController.transform.LookAt(targetPos);
                enemyController.ChangeState(EnemyState.Encount);
            }
        }
    }
}
