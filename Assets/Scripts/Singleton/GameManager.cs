using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager> {

    private bool isBootInitialezed = false;
    public bool IsBootInitialized { get { return isBootInitialezed; } }

    /// <summary>
    /// 初期化　BootSceneManagerのみ呼び出し許可
    /// </summary>
    public void Initialize()
    {
        isBootInitialezed = true;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
