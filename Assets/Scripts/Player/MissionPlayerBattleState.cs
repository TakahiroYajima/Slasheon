using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionPlayerBattleState : MissionPlayerStateBase {

    private int slashTouchID = -1;
    private float slashRayDistance = 10f;
    private List<MissionActor> slashHitEnemyList = new List<MissionActor>();

    private List<Vector3> vertices = new List<Vector3>();
    private Vector3 prevVerticesSlashPosition;

    public override void StateBeginAction()
    {
        prevVerticesSlashPosition = Camera.main.transform.position;
        slashHitEnemyList.Clear();
        _playerController.SlashEffect.SetSlashEndCallback(() =>
        {
            slashHitEnemyList.Clear();
            vertices.Clear();
            _playerController.SlashCollider.RemoveCollider();
        });
        _playerController.SlashCollider.SetCollisionEnterCallback(ColliderEnterCallback);
    }

    public override void StateEndAction()
    {
        slashHitEnemyList.Clear();
        vertices.Clear();
        _playerController.SlashCollider.RemoveCollider();
    }

    public override void StateActionUpdate()
    {
        int touchID = InputManager.Instance.GetAnyTouchBeginID();
        if (touchID != -1)
        {
            if (!InputManager.Instance.IsUITouch(touchID))
            {
                slashTouchID = touchID;
            }
        }
        if (slashTouchID != -1)
        {
            _playerController.SlashCollider.RemoveCollider();
            _playerController.SlashEffect.UpdateAction();
            Vector2 touchPos = InputManager.Instance.GetTouchPosition(slashTouchID);
            Vector3 touchVertices = Camera.main.ScreenToWorldPoint(InputManager.Instance.GetTouchPosition(slashTouchID) + Camera.main.transform.forward * slashRayDistance);
            if (InputManager.Instance.IsTouchDown(slashTouchID))
            {
                vertices.Clear();
                //vertices.Add(Camera.main.transform.position);
                //vertices.Add(touchVertices);
                prevVerticesSlashPosition = touchVertices;
            }
            else if (InputManager.Instance.IsTouchEnd(slashTouchID))
            {
                slashTouchID = -1;
                vertices.Clear();
            }
            else
            {
                if(vertices.Count < 3)
                {
                    vertices.Clear();
                    vertices.Add(Camera.main.transform.position);
                    if (prevVerticesSlashPosition != Camera.main.transform.position)
                    {
                        vertices.Add(prevVerticesSlashPosition);
                    }
                    else
                    {
                        vertices.Add(touchVertices);
                    }
                }
                vertices.Add(touchVertices);
                prevVerticesSlashPosition = touchVertices;
                if (vertices[1] != vertices[2])
                {
                    _playerController.SlashCollider.SetPoint(vertices.ToArray());
                    _playerController.SlashCollider.CreateCollider();
                }
            }

            //Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetTouchPosition(slashTouchID));
            //Debug.DrawRay(ray.origin, ray.direction * slashRayDistance, Color.red, 1f, false);
            //RaycastHit hit = new RaycastHit();
            //if (Physics.Raycast(ray, out hit, slashRayDistance))
            //{
            //    if (SlasheonUtility.IsLayerNameMatch(hit.collider.gameObject, "Enemy"))
            //    {
            //        MissionActor hitActor = hit.collider.gameObject.GetComponent<MissionActor>();
            //        //一度の斬撃で複数回ダメージ判定しないように調整(linq適用前)
            //        //bool cantDamage = false;
            //        //for (int i = 0; i < slashHitEnemyList.Count; i++)
            //        //{
            //        //    if(slashHitEnemyList[i].transform.GetInstanceID() == hitActor.transform.GetInstanceID())
            //        //    {
            //        //        cantDamage = true;
            //        //        break;
            //        //    }
            //        //}
            //        //if (!cantDamage)
            //        //{
            //        //    hitActor.Damage(_playerController.PlayerActorState.attack);
            //        //    slashHitEnemyList.Add(hitActor);
            //        //}

            //        //一度の斬撃で複数回ダメージ判定しないように調整
            //        int hitedCount = slashHitEnemyList.Where(x => x.transform.GetInstanceID() == hitActor.transform.GetInstanceID()).Count();
            //        if(hitedCount == 0)
            //        {
            //            hitActor.Damage(_playerController.PlayerActorState.attack);
            //            slashHitEnemyList.Add(hitActor);
            //        }
            //    }
            //}

            //if (InputManager.Instance.IsTouchEnd(slashTouchID))
            //{
            //    slashTouchID = -1;
            //    vertices.Clear();
            //}
        }
    }

    public void ColliderEnterCallback(Collider collider)
    {
        Debug.Log("Hit :: " + collider.gameObject.name);
        if ((SlasheonUtility.IsLayerNameMatch(collider.gameObject, "Enemy")))
        {
            MissionActor hitActor = collider.gameObject.GetComponent<MissionActor>();
            //一度の斬撃で複数回ダメージ判定しないように調整
            int hitedCount = slashHitEnemyList.Where(x => x.transform.GetInstanceID() == hitActor.transform.GetInstanceID()).Count();
            if (hitedCount == 0)
            {
                hitActor.Damage(_playerController.PlayerActorState.attack);
                slashHitEnemyList.Add(hitActor);
            }
        }
    }
}
