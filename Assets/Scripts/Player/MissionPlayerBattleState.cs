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
        _playerController.SlashCollider.SetCollisionEnterCallback(ColliderEnterCallback);

        //各武器のボタンプッシュ時のコールバック
        _playerController.UIController.SetWeapon1PushCallback(() =>
        {
            currentWeaponMode = Weapon.Blade;
        });
        _playerController.UIController.SetWeapon2PushCallback(() =>
        {
            currentWeaponMode = Weapon.Bow;
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
                if (vertices.Count < 3)
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
        }
    }

    private void BowModeAction()
    {
        Debug.Log("bow");
        int touchID = InputManager.Instance.GetAnyTouchBeginID();
        if (touchID != -1)
        {
            if (!InputManager.Instance.IsUITouch(touchID))
            {
                slashTouchID = touchID;
            }
            if (slashTouchID != -1)
            {
                Vector2 touchPos = InputManager.Instance.GetTouchPosition(slashTouchID);
                //タッチ中は力をためる
                if(InputManager.Instance.IsTouchMove(slashTouchID) || InputManager.Instance.IsTouch(slashTouchID))
                {

                }else if (InputManager.Instance.IsTouchEnd(slashTouchID))
                {
                    //矢を放つ
                    Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetTouchPosition(slashTouchID));
                    Vector3 dir = ray.direction;
                    _playerController.ArrowObject.ShotArrow(100f, dir.normalized);
                }
            }
        }
    }

    public void ColliderEnterCallback(Collider collider)
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
                _playerController.InstanceSlashDamageEffect(collider);
                slashHitEnemyList.Add(hitActor);
                UpdateStamina(_playerController.PlayerState.consumptionStaminaSlashHit);
            }
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