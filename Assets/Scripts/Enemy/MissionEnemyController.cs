using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyController : MonoBehaviour {

    //protected EnemyState nowEnemyState = EnemyState.Expedition;
    private Dictionary<string, MissionEnemyStateBase> enemyStatus = null;
    private MissionEnemyStateBase nowActionState = null;

	// Use this for initialization
	void Start () {
        enemyStatus = new Dictionary<string, MissionEnemyStateBase>
        {
            {EnemyState.Expedition.ToString(),new MissionEnemyExpeditionState() },
            {EnemyState.Encount.ToString(),new MissionEnemyEncountState() },
            {EnemyState.Battle.ToString(),new MissionEnemyBattleState() },
            {EnemyState.Death.ToString(),new MissionEnemyDeathState() },
        };
        foreach(var state in enemyStatus)
        {
            state.Value.Initialize(this);
        }
        ChangeState(EnemyState.Expedition);
	}
	
	// Update is called once per frame
	void Update () {
        if (nowActionState != null)
        {
            nowActionState.StateUpdateAction();
        }
	}

    public void ChangeState(EnemyState state)
    {
        if(nowActionState != null)
        {
            nowActionState.StateEndAction();
        }
        nowActionState = enemyStatus[state.ToString()];
        nowActionState.StateBeginAction();
    }

    public virtual void BattleAction()
    {

    }
    public void DeathAction()
    {

    }
}

public enum EnemyState
{
    Expedition,
    Encount,
    Battle,
    Death,
}