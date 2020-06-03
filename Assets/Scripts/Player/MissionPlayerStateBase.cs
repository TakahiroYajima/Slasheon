using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPlayerStateBase {

    protected PlayerController _playerController = null;

    public virtual void Initialize(PlayerController player)
    {
        _playerController = player;
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
    public virtual void StateActionUpdate()
    {

    }
}
