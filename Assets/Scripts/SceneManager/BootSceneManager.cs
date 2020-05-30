using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneManager : MonoBehaviour {
    [SerializeField] private GameObject debugObject = null;
	// Use this for initialization
	void Start () {
#if Develop
        GameObject instanceDebug = Instantiate(debugObject) as GameObject;
#endif
        GameManager.Instance.Initialize();
        SceneControllManager.Instance.ChangeSceneAsync("GameScene", true, false, true);
    }
}
