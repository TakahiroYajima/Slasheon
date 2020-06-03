using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Dictionary<string, MissionPlayerStateBase> playerStatus = null;
    private MissionPlayerStateBase nowPlayerState = null;

    private float raycastDistance = 30f;
    private Vector3 moveBeginPosition = Vector3.zero;//移動開始時の場所を保持
    private Vector3 moveTargetPos = Vector3.zero;//タッチした場所を保持
    private float moveSpeed = 7f;
    private float rotateSpeed = 1f;
    private bool isMoving = false;

    // Use this for initialization
    void Start () {
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
        ChangeState(playerStatus["Expedition"]);

        moveBeginPosition = transform.position;
        moveTargetPos = transform.position;
	}

    /// <summary>
    /// MissionSceneManagerより毎フレーム更新処理として呼び出される
    /// </summary>
    public void playerUpdate () {
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

    /// <summary>
    /// 探索モード時のアクション
    /// </summary>
    public void ExpeditionAction()
    {
        Vector3 targetPos = TouchActionOnField();
        if (targetPos != Vector3.zero)
        {
            moveTargetPos = targetPos;
        }
        if (isMoving)
        {
            MoveOnField(moveTargetPos);
        }
    }
    /// <summary>
    /// 敵とエンカウントした時のアクション
    /// </summary>
    public void EncountBeginAction()
    {
        isMoving = false;
    }
    /// <summary>
    /// エンカウントステータス時のアクション
    /// </summary>
    public void EncountAction()
    {
        //徐々に敵の方を向く
    }

    public Vector3 TouchActionOnField()
    {
        if (InputManager.Instance.IsTouchEnd())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetTouchPosition(1));
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(ray, out hit, raycastDistance))
            {
                if (SlasheonUtility.IsLayerNameMatch(hit.collider.gameObject, "Field"))
                {
                    moveBeginPosition = transform.position;
                    isMoving = true;
                    return hit.point;
                }
            }
        }
        return Vector3.zero;
    }
    /// <summary>
    /// 指定した場所まで移動
    /// </summary>
    /// <param name="targetPosition"></param>
    public void MoveOnField(Vector3 targetPosition)
    {
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        float fromBeginDistance = Vector3.Distance(transform.position, moveBeginPosition);
        float toTargetDistance = Vector3.Distance(targetPosition, moveBeginPosition);
        float moveX = targetPosition.x - moveBeginPosition.x;
        float moveZ = targetPosition.z - moveBeginPosition.z;
        Vector3 movingPos = new Vector3(moveX, 0f, moveZ).normalized;
        transform.position += movingPos * moveSpeed * Time.deltaTime;
        RotationToMoveTarget(targetPosition);

        //開始時の位置からターゲットの位置までの距離に達したら終了
        Debug.Log("distance :: " + fromBeginDistance + " : " + toTargetDistance +" :: "+ moveBeginPosition + " : " + targetPosition);
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
    public void RotationToMoveTarget(Vector3 targetPosition)
    {
        Vector3 targetDir = new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
