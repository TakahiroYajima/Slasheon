using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyController : MissionActor {

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

    public override void Damage(int damage)
    {
        if (nowActionState != enemyStatus[EnemyState.Death.ToString()])
        {
            base.Damage(damage);
        }
    }
    public override void Death()
    {
        ChangeState(EnemyState.Death);
    }
    public void DestroyEnemy()
    {
        MissionSceneManager.Instance.DeleteDeathEnemy(this);
        Destroy(this.gameObject);
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