using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyController : MissionActor {

    //protected EnemyState nowEnemyState = EnemyState.Expedition;
    [SerializeField] protected float allowableApproachDistance = 5f;
    public float AllowableApproachDistance { get { return allowableApproachDistance; } }
    [SerializeField] private float encountPlayerDistance = 10f;//エンカウント判定するプレイヤーとの距離
    public float EncountPlayerDistance { get { return encountPlayerDistance; } }

    private Dictionary<string, MissionEnemyStateBase> enemyStatus = null;
    private MissionEnemyStateBase nowActionState = null;

    public delegate void EnemyDamageCallback();
    private EnemyDamageCallback damageCallback;

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
        InitializePrivateSetting();
        ChangeState(EnemyState.Expedition);
	}
    /// <summary>
    /// 内部の初期化
    /// </summary>
    protected virtual void InitializePrivateSetting()
    {

    }
	
	// Update is called once per frame
	void Update () {
        if (nowActionState != null && MissionSceneManager.Instance.IsEnemyActionable)
        {
            nowActionState.StateUpdateAction();
        }
	}
    public void SetEnemyDamageCallback(EnemyDamageCallback callback)
    {
        damageCallback = callback;
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

    public override void Damage(float damage)
    {
        if (nowActionState != enemyStatus[EnemyState.Death.ToString()] || 
            nowActionState != enemyStatus[EnemyState.Expedition.ToString()])
        {
            DamageAction();
            base.Damage(damage);
            damageCallback();
        }
    }
    public override void Death()
    {
        DeathAction();
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
    public virtual void AttackAction()
    {

    }
    public virtual void DamageAction()
    {

    }
    public virtual void DeathAction()
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