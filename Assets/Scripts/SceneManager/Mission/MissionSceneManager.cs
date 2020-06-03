using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSceneManager : SingletonMonoBehaviour<MissionSceneManager> {

    private MissionState nowMissionState = MissionState.Initialize;
    public MissionState NowMissionState { get { return nowMissionState; } }

    private Dictionary<string, MissionSceneStateBase> missionStatus = null;
    private MissionSceneStateBase nowActionState = null;

    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private SceneControllManager sceneControllManager = null;

    [SerializeField] private PlayerController player = null;

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
    }

    public void StartAction()
    {
        ChangeMissionState(MissionState.Expedition);
    }

    public void ExpeditionAction()
    {
        player.playerUpdate();
    }
}
