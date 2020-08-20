using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyDeathState : MissionEnemyStateBase
{
    
    /// <summary>
    /// このステートになった瞬間のアクション
    /// </summary>
    public override void StateBeginAction()
    {
        Collider collider = enemyController.GetComponent<Collider>();
        if(collider != null)
        {
            collider.enabled = false;
        }
        enemyController.StartCoroutine(DeathAction());
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

    }

    private IEnumerator DeathAction()
    {
        yield return new WaitForSeconds(1.2f);
        enemyController.DestroyEnemy();
    }
}
