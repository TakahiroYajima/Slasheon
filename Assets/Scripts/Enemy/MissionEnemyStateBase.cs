using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionEnemyStateBase {

    protected MissionEnemyController enemyController = null;
    public virtual void Initialize(MissionEnemyController enemy)
    {
        enemyController = enemy;
    }

    /// <summary>
    /// このステートになった瞬間のアクション
    /// </summary>
    public abstract void StateBeginAction();

    /// <summary>
    /// ステート切り替え時、切り替わる前のステートの終了アクション
    /// </summary>
	public abstract void StateEndAction();

    /// <summary>
    /// このステートでの毎フレーム更新処理
    /// </summary>
    public abstract void StateUpdateAction();
}
