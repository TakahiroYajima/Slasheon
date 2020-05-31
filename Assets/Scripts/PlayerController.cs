using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private float raycastDistance = 30f;
    private Vector3 moveBeginPosition = Vector3.zero;//移動開始時の場所を保持
    private Vector3 moveTargetPos = Vector3.zero;//タッチした場所を保持
    private float moveSpeed = 7f;
    private float rotateSpeed = 1f;
    private bool isMoving = false;

    // Use this for initialization
    void Start () {
        moveBeginPosition = transform.position;
        moveTargetPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        switch (MissionSceneManager.Instance.NowMissionState)
        {
            case MissionState.Expedition:
                Vector3 targetPos = TouchActionOnField();
                if(targetPos != Vector3.zero)
                {
                    moveTargetPos = targetPos;
                }
                if(isMoving)
                {
                    MoveOnField(moveTargetPos);
                }
                break;
            case MissionState.Encount:

                break;
            case MissionState.Battle:

                break;
            case MissionState.Result:

                break;
            case MissionState.GameOver:

                break;
            default:break;
        }
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

    public void RotationToMoveTarget(Vector3 targetPosition)
    {
        Vector3 targetDir = new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
