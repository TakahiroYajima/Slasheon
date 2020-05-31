using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSceneManager : SingletonMonoBehaviour<MissionSceneManager> {

    private MissionState nowMissionState = MissionState.Initialize;
    public MissionState NowMissionState { get { return nowMissionState; } }

    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private SceneControllManager sceneControllManager = null;

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
        nowMissionState = MissionState.Start;
    }
	
	// Update is called once per frame
	void Update () {
        switch (nowMissionState)
        {
            case MissionState.Start:
                nowMissionState = MissionState.Expedition;
                break;
            case MissionState.Expedition:
                break;
            case MissionState.Encount:

                break;
            case MissionState.Battle:

                break;
            case MissionState.Result:

                break;
            case MissionState.GameOver:

                break;
            default: break;
        }
	}
    public void ChangeMissionState(MissionState state)
    {
        nowMissionState = state;
    }
}
