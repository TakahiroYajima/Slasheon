﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSceneResultState : MissionSceneStateBase
{

    public override void Initialize()
    {

    }

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
        if(MissionSceneManager.Instance.LastEnemy == null)
        {
            
        }
    }
}
