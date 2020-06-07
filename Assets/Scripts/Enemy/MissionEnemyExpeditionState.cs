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
        Vector3 playerPos = MissionSceneManager.Instance.playerPosition;
        Vector3 targetPos = new Vector3(playerPos.x, enemyController.transform.position.y, playerPos.z);
        if (Vector3.Distance(targetPos, enemyController.transform.position) > 5f)
        {
            enemyController.transform.LookAt(targetPos);
            enemyController.transform.Translate((enemyController.transform.position - targetPos).normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            enemyController.ChangeState(EnemyState.Encount);
        }
    }
}
