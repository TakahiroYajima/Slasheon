#if Develop
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : SingletonMonoBehaviour<DebugScript> {
    [SerializeField] private bool isDebugMode = false;
    private int frameCount = 0;
    private float prevTime = 0f;

    [SerializeField] private Text fpsText = null;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    // Use this for initialization
    void Start () {
        frameCount = 0;
        prevTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (isDebugMode)
        {
            DisplayFPS();
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("mousePos :: " + Input.mousePosition + " : angle :: " + (Mathf.Atan2(Input.mousePosition.x - (Screen.width / 2), Input.mousePosition.y - (Screen.height / 2)) * Mathf.Rad2Deg));
        //}
	}

    private void DisplayFPS()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;
        if(time >= 0.5f)
        {
            float fps = (float)frameCount / time;
            fpsText.text = "FPS : " + Mathf.FloorToInt(fps).ToString();
            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}
#endif