﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MissionSceneManager : SingletonMonoBehaviour<MissionSceneManager> {

    private MissionState nowMissionState = MissionState.Initialize;
    public MissionState NowMissionState { get { return nowMissionState; } }

    private Dictionary<string, MissionSceneStateBase> missionStatus = null;
    private MissionSceneStateBase nowActionState = null;

    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private SceneControllManager sceneControllManager = null;

    [SerializeField] private PlayerController player = null;
    public PlayerController Player { get { return player; } }
    public Vector3 playerPosition { get { return player.transform.position; } }

    [SerializeField] private MissionUIController missionUIController = null;
    public MissionUIController MissionUIController { get { return missionUIController; } }

    //バトルに使用する変数
    private List<MissionEnemyController> encountEnemyList = new List<MissionEnemyController>();//エンカウントした敵
    private MissionEnemyController lastEnemy = null;//最後の敵の死亡アクション終了待ち用
    public MissionEnemyController LastEnemy { get { return lastEnemy; } }

    //コールバック
    public delegate void StateChangeCallback(MissionState state);
    private StateChangeCallback stateChangeCallback = (state) => { };

    // Use this for initialization
    void Start () {
#if Develop
        Instantiate(gameManager);
        if (!GameManager.Instance.IsBootInitialized)
        {
            SceneControllManager manager = Instantiate(sceneControllManager);
            manager.LoadBootSceneAndBackScene("MissionScene");
            return;
        }
#endif
        missionStatus = new Dictionary<string, MissionSceneStateBase>
        {
            {MissionState.Start.ToString(),new MissionSceneStartState() },
            {MissionState.Expedition.ToString(), new MissionSceneExpeditionState() },
            {MissionState.Encount.ToString(), new MissionSceneEncountState() },
            {MissionState.Battle.ToString(),new MissionSceneBattleState() },
            {MissionState.Result.ToString(), new MissionSceneResultState() },
            {MissionState.GameOver.ToString(), new MissionSceneGameOverState() },
        };
        foreach(var state in missionStatus)
        {
            state.Value.Initialize();
        }
        ChangeMissionState(MissionState.Start);
    }
	
	// Update is called once per frame
	void Update () {
        if (nowActionState != null)
        {
            nowActionState.StateUpdateAction();
        }
	}
    /// <summary>
    /// ミッションのステータス変更
    /// </summary>
    /// <param name="state"></param>
    public void ChangeMissionState(MissionState state)
    {
        nowMissionState = state;
        string key = state.ToString();
        if(nowActionState != null)
        {
            nowActionState.StateEndAction();
        }
        nowActionState = missionStatus[key];
        nowActionState.StateBeginAction();
        stateChangeCallback(state);
    }

    public void SetStateChangeCallback(StateChangeCallback action)
    {
        stateChangeCallback += action;
    }

    public void PlayerUpdate()
    {
        player.playerUpdate();
    }
    /// <summary>
    /// プレイヤーとエンカウントした敵を追加
    /// </summary>
    /// <param name="enemy"></param>
    public void SetEncountEnemy(MissionEnemyController enemy)
    {
        encountEnemyList.Add(enemy);
        if(nowMissionState == MissionState.Expedition)
        {
            ChangeMissionState(MissionState.Encount);
        }
    }

    public void DeleteDeathEnemy(MissionEnemyController deathEnemy)
    {
        for(int i = 0; i < encountEnemyList.Count; i++)
        {
            if(encountEnemyList[i] == deathEnemy)
            {
                encountEnemyList.RemoveAt(i);
            }
        }
        if(encountEnemyList.Count == 0)
        {
            lastEnemy = deathEnemy;
            ChangeMissionState(MissionState.Result);
        }
    }
    /// <summary>
    /// 最初にエンカウントした敵情報を返す
    /// </summary>
    /// <returns></returns>
    public MissionEnemyController GetFirstEncountEnemy()
    {
        if (encountEnemyList.Count > 0)
        {
            return encountEnemyList[0];
        }
        else { return null; }
    }
}