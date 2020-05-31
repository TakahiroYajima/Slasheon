using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneManager : SingletonMonoBehaviour<BootSceneManager> {
    [SerializeField] private GameObject debugObject = null;
	// Use this for initialization
	void Start () {
#if Develop
        GameObject instanceDebug = Instantiate(debugObject) as GameObject;
#endif
        GameManager.Instance.Initialize();

        //Developモードの時、ブートシーン以外のシーンから開始した際はGameSceneではなく、開始したシーンへ遷移する
        if (!SceneControllManager.Instance.IsDebugSceneLoadInitializeAction)
        {
            SceneControllManager.Instance.ChangeSceneAsync("GameScene", true, false, true);
        }
    }
#if Develop
    public void BootSceneInitAndBackScene(string backSceneName)
    {
        SceneControllManager.Instance.ChangeSceneAsync(backSceneName, true, false, true);
    }
#endif
}
