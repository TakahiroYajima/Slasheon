using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSceneManager : SingletonMonoBehaviour<HomeSceneManager>
{

    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private SceneControllManager sceneControllManager = null;

    // Use this for initialization
    void Start()
    {
#if Develop
        Instantiate(gameManager);
        if (!GameManager.Instance.IsBootInitialized)
        {
            SceneControllManager manager = Instantiate(sceneControllManager);
            manager.LoadBootSceneAndBackScene("HomeScene");
            return;
        }
#endif

        
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PushStageStartButton()
    {
        UserStatusManager.Instance.ConsumptionStaminaOrError(10f, () =>
        {
            SceneControllManager.Instance.ChangeSceneAsync("MissionScene", true, true, true);
        });
    }

    public void PushBackToTitleButton()
    {
        SceneControllManager.Instance.ChangeSceneAsync("TitleScene", true, true, true);
    }
}
