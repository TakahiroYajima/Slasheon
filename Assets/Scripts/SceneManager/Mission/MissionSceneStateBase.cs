using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSceneStateBase {

    public virtual void Initialize()
    {
        
    }

    /// <summary>
    /// このステートになった瞬間のアクション
    /// </summary>
    public virtual void StateBeginAction()
    {

    }

    /// <summary>
    /// ステート切り替え時、切り替わる前のステートの終了アクション
    /// </summary>
	public virtual void StateEndAction()
    {

    }

    /// <summary>
    /// このステートでの毎フレーム更新処理
    /// </summary>
    public virtual void StateUpdateAction()
    {

    }
}
