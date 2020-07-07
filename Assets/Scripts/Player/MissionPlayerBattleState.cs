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

    public Weapon currentWeaponMode { get; private set; }

    public override void StateBeginAction()
    {
        prevVerticesSlashPosition = Camera.main.transform.position;
        slashHitEnemyList.Clear();

        //剣を振った時にスタミナを減らすコールバック
        _playerController.SlashEffect.SetSlashBeginCallback(() =>
        {
            if (_playerController.PlayerState.stamina > _playerController.PlayerState.consumptionStaminaSlash)
            {
                UpdateStamina(_playerController.PlayerState.consumptionStaminaSlash);
            }
        });
        _playerController.SlashEffect.SetSlashEndCallback(() =>
        {
            slashHitEnemyList.Clear();
            vertices.Clear();
            _playerController.SlashCollider.RemoveCollider();
        });
        _playerController.SlashCollider.SetCollisionEnterCallback(SlashColliderEnterCallback);

        //矢が衝突した際のコールバック
        _playerController.BowAction.SetArrowHitCallback(ArrowColliderEnterCallback);

        //各武器のボタンプッシュ時のコールバック
        _playerController.UIController.SetWeapon1PushCallback(() =>
        {
            currentWeaponMode = Weapon.Blade;
            slashTouchID = -1;
        });
        _playerController.UIController.SetWeapon2PushCallback(() =>
        {
            currentWeaponMode = Weapon.Bow;
            slashTouchID = -1;
        });

        //戦闘開始時のステートにする
        currentWeaponMode = Weapon.Blade;
    }

    public override void StateEndAction()
    {
        slashHitEnemyList.Clear();
        vertices.Clear();
        _playerController.SlashCollider.RemoveCollider();
    }

    public override void StateActionUpdate()
    {
        _playerController.RecoverStamina();
        _playerController.RotationViewAction();
        Debug.Log("PlayerBattle");
        switch (currentWeaponMode)
        {
            case Weapon.Blade:BladeModeAction();
                break;
            case Weapon.Bow:BowModeAction();
                break;
            default:break;
        }
    }

    private void BladeModeAction()
    {
        int touchID = InputManager.Instance.GetAnyTouchBeginID();
        if (touchID != -1)
        {
            if (!InputManager.Instance.IsUITouch(touchID))
            {
                slashTouchID = touchID;
            }
        }
        _playerController.SlashCollider.RemoveCollider();
        if (slashTouchID != -1)
        {
            //_playerController.SlashCollider.RemoveCollider();
            //スタミナが足りなかったら攻撃できない
            if (_playerController.PlayerState.stamina < _playerController.PlayerState.consumptionStaminaSlash + _playerController.PlayerState.consumptionStaminaSlashHit)
            {
                slashTouchID = -1;
                _playerController.SlashEffect.EndSlashEffect();
                return;
            }
            _playerController.SlashEffect.UpdateAction();

            //斬撃の当たり判定を作る
            Vector2 touchPos = InputManager.Instance.GetTouchPosition(slashTouchID);
            Vector3 touchVertices = Camera.main.ScreenToWorldPoint(InputManager.Instance.GetTouchPosition(slashTouchID) + Camera.main.transform.forward * slashRayDistance);
            if (InputManager.Instance.IsTouchDown(slashTouchID))
            {
                vertices.Clear();
                vertices.Add(Camera.main.transform.position);
                vertices.Add(touchVertices);
                prevVerticesSlashPosition = touchVertices;
            }
            else if (InputManager.Instance.IsTouchEnd(slashTouchID))
            {
                slashTouchID = -1;
                vertices.Clear();
            }
            else
            {
                if (vertices.Count == 0)
                {
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
                if (touchVertices != prevVerticesSlashPosition)
                {
                    vertices.Add(touchVertices);
                    prevVerticesSlashPosition = touchVertices;
                    //_playerController.SlashCollider.SetPoint(touchVertices);
                }
                if (vertices.Count >= 3)
                {
                    //_playerController.SlashCollider.SetPointsList(vertices);
                    //_playerController.SlashCollider.MakeTriangles(vertices.Count);
                    _playerController.SlashCollider.CreateCollider(vertices);
                }
            }
        }
    }
    /// <summary>
    /// 弓矢モードのアクション
    /// </summary>
    private void BowModeAction()
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
            Vector2 touchPos = InputManager.Instance.GetTouchPosition(slashTouchID);
            //タッチ中は力をためる
            if (InputManager.Instance.IsTouchMove(slashTouchID) || InputManager.Instance.IsTouch(slashTouchID))
            {
                
            }
            else if (InputManager.Instance.IsTouchEnd(slashTouchID))
            {
                slashTouchID = -1;
                //矢を放つ
                touchPos += new Vector2(0f, 50f);
                Ray ray = Camera.main.ScreenPointToRay(touchPos);
                Vector3 dir = ray.direction;
                _playerController.BowAction.ShotArrow(10000f, dir);
            }
        }
    }

    public void SlashColliderEnterCallback(Collider collider)
    {
        if(_playerController.PlayerState.stamina < _playerController.PlayerState.consumptionStaminaSlashHit)
        {
            _playerController.SlashCollider.RemoveCollider();
            return;
        }

        if ((SlasheonUtility.IsLayerNameMatch(collider.gameObject, "Enemy")))
        {
            MissionActor hitActor = collider.gameObject.GetComponent<MissionActor>();
            //一度の斬撃で複数回ダメージ判定しないように調整
            int hitedCount = slashHitEnemyList.Where(x => x.transform.GetInstanceID() == hitActor.transform.GetInstanceID()).Count();
            if (hitedCount == 0)
            {
                hitActor.Damage(_playerController.ActorState.attack);
                _playerController.InstanceSlashDamageEffect(collider, InputManager.Instance.GetTouchPosition(slashTouchID));
                slashHitEnemyList.Add(hitActor);
                UpdateStamina(_playerController.PlayerState.consumptionStaminaSlashHit);
            }
        }
    }

    public void ArrowColliderEnterCallback(Collider collider)
    {
        if ((SlasheonUtility.IsLayerNameMatch(collider.gameObject, "Enemy")))
        {
            MissionActor hitActor = collider.gameObject.GetComponent<MissionActor>();
            hitActor.Damage(_playerController.ActorState.attack);
            _playerController.InstanceArrowDamageEffect(collider);
        }
    }

    public void UpdateStamina(float minus)
    {
        _playerController.PlayerState.stamina -= minus;
        _playerController.UIController.SetStamina(_playerController.PlayerState.stamina, _playerController.InitPlayerState.stamina);
    }
}

public enum Weapon
{
    None,//初期化用
    Blade,
    Bow,
}