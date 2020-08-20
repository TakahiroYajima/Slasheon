using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyController : MissionActor {

    //protected EnemyState nowEnemyState = EnemyState.Expedition;
    [SerializeField] protected float allowableApproachDistance = 5f;
    public float AllowableApproachDistance { get { return allowableApproachDistance; } }
    [SerializeField] private float encountPlayerDistance = 10f;//エンカウント判定するプレイヤーとの距離
    public float EncountPlayerDistance { get { return encountPlayerDistance; } }

    protected Dictionary<string, MissionEnemyStateBase> enemyStatus = null;
    protected MissionEnemyStateBase nowActionState = null;

    public delegate void EnemyDamageCallback();
    private EnemyDamageCallback damageCallback;

    protected bool damageFlg = false;//攻撃中にダメージを受けた際のフラグ

    // Use this for initialization
    void Start () {
        InitializePrivateSetting();

	}
    /// <summary>
    /// 内部の初期化
    /// </summary>
    protected virtual void InitializePrivateSetting()
    {
        enemyStatus = new Dictionary<string, MissionEnemyStateBase>
        {
            {EnemyState.Expedition.ToString(),new MissionEnemyExpeditionState() },
            {EnemyState.Encount.ToString(),new MissionEnemyEncountState() },
            {EnemyState.Battle.ToString(),new MissionEnemyBattleState() },
            {EnemyState.Death.ToString(),new MissionEnemyDeathState() },
        };
        foreach (var state in enemyStatus)
        {
            state.Value.Initialize(this);
        }
        ChangeState(EnemyState.Expedition);
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

    //以下、攻撃アニメーションのコルーチン。本当は別のスクリプトに用意したい
    public IEnumerator MoveToInitPos()
    {
        yield return null;
        //battleState = EnemyBattleState.Waiting;
        //transform.position = battleInitPos;
        //damageFlg = false;
    }

    protected IEnumerator AttackToPlayerAction()
    {
        //Debug.Log("EnemyAttack");
        Vector3 initPos = transform.position;
        Vector3 playerPos = MissionSceneManager.Instance.playerPosition;
        playerPos -= (initPos - playerPos).normalized * 1.2f;

        float actionTime = 1f;
        float elapsedTime = 0f;
        float distance = Vector3.Distance(initPos, playerPos);
        Vector3 direction = initPos - playerPos;
        while (elapsedTime < actionTime)
        {
            if (damageFlg)
            {
                elapsedTime = actionTime;
            }
            Vector3 currentPos = transform.position - direction * (Time.deltaTime / actionTime);
            transform.position = currentPos;
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= actionTime)
            {
                if (!damageFlg)
                {
                    AttackAction();
                    MissionSceneManager.Instance.Player.Damage(1);
                }
            }
            yield return null;
        }

        StartCoroutine(MoveToInitPos());
    }
}

public enum EnemyState
{
    Expedition,
    Encount,
    Battle,
    Death,
}