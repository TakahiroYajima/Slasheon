using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public float stamina = 0f;
    public float recoverStaminaOnSecond = 5f;//1秒に回復するスタミナ量
    public float consumptionStaminaSlash = 1f;//剣を振った時の消費スタミナ
    public float consumptionStaminaSlashHit = 1f;//剣でダメージを与えた時の消費スタミナ
    public float consumptionStaminaPullArrowOnSecond = 3f;//弓を引いている時の消費スタミナ（秒）
}

public class PlayerController : MissionActor {

    //各クラスのインスタンスとプロパティ
    [SerializeField] private MissionUIController uiController = null;
    public MissionUIController UIController { get { return uiController; } }

    [SerializeField] private MeshSlashEffect slashEffect = null;
    public MeshSlashEffect SlashEffect { get { return slashEffect; } }
    [SerializeField] private SlashCollider slashCollider = null;
    public SlashCollider SlashCollider { get { return slashCollider; } }

    public PlayerRotation PlayerRotation { get; private set; }

    [SerializeField] private Transform effectParentTransform = null;
    public Transform EffectParentTrans { get { return effectParentTransform; } }
    [SerializeField] private SlashDamageEffect slashDamageEffect = null;
    public SlashDamageEffect slashDamagePref { get { return slashDamageEffect; } }
    [SerializeField] private ParticleSystem slashDamageParticle = null;

    //弓矢攻撃用のスクリプト
    [SerializeField] private BowAction bowAction = null;
    public BowAction BowAction { get { return bowAction; } }

    //Stateパターン管理
    private Dictionary<string, MissionPlayerStateBase> playerStatus = null;
    private MissionPlayerStateBase nowPlayerState = null;

    //戦闘のステータス
    private PlayerState playerState = new PlayerState();
    private PlayerState initPlayerState = new PlayerState();
    public ActorState ActorState { get { return actorState; } }
    public ActorState InitActorState { get { return initActorState; } }
    public PlayerState PlayerState { get { return playerState; } }
    public PlayerState InitPlayerState { get { return initPlayerState; } }

    //探索時の移動に使用する
    private float raycastDistance = 30f;
    public Vector3 moveBeginPosition { get; set; }//移動開始時の場所を保持
    public Vector3 moveTargetPos { get; set; }//タッチした場所を保持
    private float moveSpeed = 7f;
    private float rotateSpeed = 1f;
    public bool isMoving { get; set; }

    // Use this for initialization
    void Start () {
        PlayerRotation = GetComponent<PlayerRotation>();

        MissionSceneManager.Instance.SetStateChangeCallback(ChangeMissionState);

        playerStatus = new Dictionary<string, MissionPlayerStateBase>
        {
            {"Expedition", new MissionPlayerExpeditionState() },
            {"Encount", new MissionPlayerEncountState() },
            {"Battle", new MissionPlayerBattleState() },
            {"Result", new MissionPlayerResultState() },
            {"GameOver", new MissionPlayerGameOverState() },
        };
        //各ステートの初期設定
        foreach(var state in playerStatus)
        {
            state.Value.Initialize(this);
        }
        ChangeMissionState(MissionState.Expedition);

        moveBeginPosition = transform.position;
        moveTargetPos = transform.position;
        isMoving = false;

        //デバッグ
        initPlayerState.stamina = 30;
        playerState.stamina = 30;
	}

    /// <summary>
    /// MissionSceneManagerより毎フレーム更新処理として呼び出される
    /// </summary>
    public void playerUpdate () {
        Debug.Log(nowPlayerState);
        //現在のステートの更新処理
        nowPlayerState.StateActionUpdate();

	}

    /// <summary>
    /// ステート切り替え
    /// </summary>
    /// <param name="nextState"></param>
    private void ChangeState(MissionPlayerStateBase nextState)
    {
        if (nowPlayerState != null)
        {
            nowPlayerState.StateEndAction();//前のステートの終了処理
        }
        nowPlayerState = nextState;
        nowPlayerState.StateBeginAction();//現在のステートの開始処理
    }

    public void ChangeMissionState(MissionState nextState)
    {
        if (nowPlayerState != null)
        {
            nowPlayerState.StateEndAction();//前のステートの終了処理
        }
        nowPlayerState = playerStatus[nextState.ToString()];
        nowPlayerState.StateBeginAction();//現在のステートの開始処理
    }

    public override void Damage(int damage)
    {
        if (actorState.hp > 0)
        {
            base.Damage(damage);
            Debug.Log("ダメージ：残りHP : " + actorState.hp);
            uiController.SetHP(actorState.hp, initActorState.hp);
        }
    }
    public override void Death()
    {
        MissionSceneManager.Instance.ChangeMissionState(MissionState.GameOver);
    }

    /// <summary>
    /// 移動をストップ
    /// </summary>
    public void MovePropertyReset()
    {
        moveBeginPosition = transform.position;
        moveTargetPos = transform.position;
        isMoving = false;
    }

    /// <summary>
    /// 指定した場所まで移動
    /// </summary>
    /// <param name="targetPosition"></param>
    public void MoveOnField(Vector3 targetPosition, float quickMoveSpeed = 1f)
    {
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        float fromBeginDistance = Vector3.Distance(transform.position, moveBeginPosition);
        float toTargetDistance = Vector3.Distance(targetPosition, moveBeginPosition);
        float moveX = targetPosition.x - moveBeginPosition.x;
        float moveZ = targetPosition.z - moveBeginPosition.z;
        Vector3 movingPos = new Vector3(moveX, 0f, moveZ).normalized;
        transform.position += movingPos * moveSpeed * quickMoveSpeed * Time.deltaTime;
        RotationToMoveTarget(targetPosition);

        //開始時の位置からターゲットの位置までの距離に達したら終了
        //Debug.Log("distance :: " + fromBeginDistance + " : " + toTargetDistance +" :: "+ moveBeginPosition + " : " + targetPosition);
        if (fromBeginDistance >= toTargetDistance)
        {
            transform.position = targetPosition;
            moveBeginPosition = transform.position;
            moveTargetPos = transform.position;
            isMoving = false;
        }
    }
    /// <summary>
    /// 指定した地点の方向に徐々に向きを変える
    /// </summary>
    /// <param name="targetPosition"></param>
    public void RotationToMoveTarget(Vector3 targetPosition, float quickRotateSpeed = 1f)
    {
        Vector3 targetDir = new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateSpeed * quickRotateSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    /// <summary>
    /// 指定した秒数でターゲットの方を向くコルーチン
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="rotationTime"></param>
    /// <returns></returns>
    public IEnumerator RotationToTargetInTime(Vector3 targetPosition, float rotationTime, UnityEngine.Events.UnityAction callback = null)
    {
        Vector3 targetDir = new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position;
        Vector3 axis = Vector3.Cross(transform.forward, targetDir);
        float targetAngle = Vector3.Angle(transform.forward, targetDir) * (axis.y < 0 ? -1 : 1);
        float firstRotation = transform.rotation.y;

        float variation = targetAngle / rotationTime;
        float rotationTotal = 0f;
        float elapsedTime = 0;
        while(Mathf.Abs(rotationTotal) < Mathf.Abs(targetAngle))
        {
            float rotateY = Mathf.SmoothStep(firstRotation, targetAngle, elapsedTime * rotationTime);
            rotationTotal += rotateY;
            if (Mathf.Abs(rotationTotal) + Mathf.Abs(rotateY) >= Mathf.Abs(targetAngle))
            {

            }
            else
            {
                transform.Rotate(0, rotateY, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        if(callback != null)
        {
            callback();
        }
    }

    /// <summary>
    /// 視点回転
    /// </summary>
    public void RotationViewAction()
    {
        int touchID = InputManager.Instance.GetAnyTouchBeginID();
        if (InputManager.Instance.IsTouchDown(touchID))
        {
            if (InputManager.Instance.IsUITouch(touchID))
            {
                if (!PlayerRotation.isRotationMoving)
                {
                    if (LayerMask.LayerToName(InputManager.Instance.GetRaycastResult(touchID).gameObject.layer) == "CameraRotationUI")
                    {
                        PlayerRotation.OnPointerDown(touchID);
                    }
                }
            }
        }
        if (PlayerRotation.isRotationMoving)
        {
            if (InputManager.Instance.IsTouchEnd(PlayerRotation.touchID))
            {
                PlayerRotation.OnPointerUP();
            }
            else
            {
                PlayerRotation.OnPointerMove();
            }
        }
    }

    /// <summary>
    /// 毎フレームでスタミナ回復
    /// </summary>
    public void RecoverStamina()
    {
        if (playerState.stamina == initPlayerState.stamina) return;//Max値なら回復しない

        playerState.stamina += playerState.recoverStaminaOnSecond * Time.deltaTime;
        if(playerState.stamina > initPlayerState.stamina)
        {
            playerState.stamina = initPlayerState.stamina;
        }
        uiController.SetStamina(playerState.stamina, initPlayerState.stamina);
    }

    /// <summary>
    /// 斬撃でダメージを与えた際のエフェクト再生
    /// </summary>
    /// <param name="collider"></param>
    public void InstanceSlashDamageEffect(Collider collider)
    {
        SlashDamageEffect effect = Instantiate(slashDamageEffect, effectParentTransform);
        RectTransform rectTransform = slashDamageEffect.gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        Vector3 hitPoint = collider.ClosestPoint(collider.transform.position);

        Debug.Log("slashEffect :: " + rectTransform.localRotation);
        StartCoroutine(effect.StartAction(Quaternion.Euler(0f, 0f, slashEffect.GetCurrentSlashAngle())));
        slashDamageParticle.gameObject.transform.position = hitPoint;
        slashDamageParticle.Play();
    }

    public void InstanceArrowDamageEffect(Collider collider)
    {
        slashDamageParticle.gameObject.transform.position = collider.transform.position;
        slashDamageParticle.Play();
    }
}
