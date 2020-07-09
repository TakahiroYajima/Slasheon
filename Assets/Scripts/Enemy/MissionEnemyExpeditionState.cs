using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyExpeditionState : MissionEnemyStateBase {

    private float moveSpeed = 10f;

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
                        moveDirection = Quaternion.Euler(0f, -120f, 0f) * (enemyController.transform.position - enemy.transform.position).normalized;
                    }
                }
            }
            if (Vector3.Distance(targetPos, enemyController.transform.position) > enemyController.EncountPlayerDistance)
            {
                enemyController.transform.LookAt(enemyController.transform.position - moveDirection);
                enemyController.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                enemyController.transform.LookAt(targetPos);
                enemyController.ChangeState(EnemyState.Encount);
            }
        }
    }
}
