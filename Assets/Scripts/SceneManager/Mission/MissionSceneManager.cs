using System.Collections;
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

    [SerializeField] private Camera uiCamera = null;
    public Camera UICamera { get { return uiCamera; } }
    [SerializeField] private Camera effectCamera = null;
    public Camera EffectCamera { get { return effectCamera; } }

    //ステージのScriptableObject
    private StageScriptable stageScriptable = null;
    private StageManager instancedStage = null;
    public string currentStageID { get; private set; }
    public string prevStageID { get; private set; }

    public bool IsEnemyActionable {
        get
        {
            return nowMissionState != MissionState.Initialize &&
                    nowMissionState != MissionState.Start &&
                    nowMissionState != MissionState.Result &&
                    nowMissionState != MissionState.GameOver;
        }
    }

    //バトルに使用する変数
    public List<MissionEnemyController> encountEnemyList { get; private set; }//エンカウントした敵
    private MissionEnemyController lastEnemy = null;//最後の敵の死亡アクション終了待ち用
    public MissionEnemyController LastEnemy { get { return lastEnemy; } }

    //コールバック
    public delegate void StateChangeCallback(MissionState state);
    private StateChangeCallback stateChangeCallback = (state) => { };

    // Use this for initialization
    void Start () {
        encountEnemyList = new List<MissionEnemyController>();
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
        currentStageID = SceneControllManager.Instance.loadStageID;
        LoadStage(currentStageID, () =>
         {
             ChangeMissionState(MissionState.Start);
             player.Initialize();
             SceneControllManager.Instance.FadePanel(FadeMode.In);
         });
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

    public void ReadAsset(string stageID, UnityAction callback)
    {
        StartCoroutine(ResourceManager.Instance.LoadScriptableObject("ScriptableObject/StageDatas/" + stageID, (sObj) =>
         {
             stageScriptable = sObj.asset as StageScriptable;
             callback();
         }));
    }

    /// <summary>
    /// ステージを生成
    /// </summary>
    /// <param name="stageID"></param>
    public void LoadStage(string stageID, UnityAction callback)
    {
        StageData find = null;
        ReadAsset(stageID, () =>
         {
             find = stageScriptable.stageData;
             if (find != null)
             {
                //ステージ生成
                SceneControllManager.Instance.loadStageID = "";

                 var obj = Instantiate(find.prefab) as GameObject;
                 instancedStage = obj.GetComponent<StageManager>();
                 instancedStage.Initialize();
                 Debug.Log("ステージ生成成功 : " + stageID);
             }
             else
             {
                 Debug.LogError("ステージがありません : " + stageID);
             }
             callback();
         });
    }
    /// <summary>
    /// ステージ切り替え
    /// </summary>
    /// <param name="stageID"></param>
    /// <returns></returns>
    public void ChangeStage(string stageID)
    {
        SceneControllManager.Instance.FadePanel(FadeMode.Out, -1, () =>
        {
            Destroy(instancedStage.gameObject);
            LoadStage(stageID, null);
            SceneControllManager.Instance.FadePanel(FadeMode.In);
        });
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
        SoundManager.Instance.PlaySE("Encount");
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
            StartCoroutine(EnemyAllDestroyedAction());
        }
    }
    private IEnumerator EnemyAllDestroyedAction()
    {
        yield return new WaitForSeconds(1f);
        if (encountEnemyList.Count == 0)
        {
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
