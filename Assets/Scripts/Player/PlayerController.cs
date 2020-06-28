using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MissionActor {

    [SerializeField] private MeshSlashEffect slashEffect = null;
    public MeshSlashEffect SlashEffect { get { return slashEffect; } }
    [SerializeField] private SlashCollider slashCollider = null;
    public SlashCollider SlashCollider { get { return slashCollider; } }

    [SerializeField] private Transform effectParentTransform = null;
    public Transform EffectParentTrans { get { return effectParentTransform; } }
    [SerializeField] private SlashDamageEffect slashDamageEffect = null;
    public SlashDamageEffect slashDamagePref { get { return slashDamageEffect; } }
    [SerializeField] private ParticleSystem slashDamageParticle = null;

    private Dictionary<string, MissionPlayerStateBase> playerStatus = null;
    private MissionPlayerStateBase nowPlayerState = null;

    public ActorState PlayerActorState { get { return actorState; } }

    private float raycastDistance = 30f;
    public Vector3 moveBeginPosition { get; set; }//移動開始時の場所を保持
    public Vector3 moveTargetPos { get; set; }//タッチした場所を保持
    private float moveSpeed = 7f;
    private float rotateSpeed = 1f;
    public bool isMoving { get; set; }

    //public struct Data
    //{
    //    public float raycastDistance;
    //    public Vector3 moveBeginPosition;//移動開始時の場所を保持
    //    public Vector3 moveTargetPos;//タッチした場所を保持
    //    public float moveSpeed;
    //    public float rotateSpeed;
    //    public bool isMoving;
    //}

    // Use this for initialization
    void Start () {
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
        base.Damage(damage);
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

    public void InstanceSlashDamageEffect(Vector3 position)
    {
        SlashDamageEffect effect = Instantiate(slashDamageEffect, effectParentTransform);
        RectTransform rectTransform = slashDamageEffect.gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        //rectTransform.anchoredPosition = position;
        //rectTransform.localRotation = Quaternion.Euler(0f, 0f, slashEffect.GetCurrentSlashAngle());
        Debug.Log("slashEffect :: " + rectTransform.localRotation);
        StartCoroutine(effect.StartAction(Quaternion.Euler(0f, 0f, slashEffect.GetCurrentSlashAngle())));
        slashDamageParticle.gameObject.transform.position = position;
        slashDamageParticle.Play();
    }
}
