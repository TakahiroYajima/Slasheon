using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonoBehaviour<InputManager> {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// タッチ開始時かを返す
    /// </summary>
    /// <param name="touchID"></param>
    /// <returns></returns>
    public bool IsTouchDown(int touchID = -1)
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#elif UNITY_IOS || UNITY_ANDROID
        if(touchID == -1){ return false;}

        bool isTouchDown = false;
        for(int i = 0; i < Input.touches.Length; i++)
        {
            if(Input.touches[i].fingerId == touchID)
            {
                isTouchDown = Input.touches[i].phase == TouchPhase.Began;
                break;
            }
        }
        return isTouchDown;
#else
        return false;
#endif
    }
    /// <summary>
    /// タッチ中かを返す
    /// </summary>
    /// <param name="touchID"></param>
    /// <returns></returns>
    public bool IsTouch(int touchID = -1)
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#elif UNITY_IOS || UNITY_ANDROID
        if(touchID == -1){ return false;}

        bool isTouch = false;
        for(int i = 0; i < Input.touches.Length; i++)
        {
            if(Input.touches[i].fingerId == touchID)
            {
                isTouch = Input.touches[i].phase == TouchPhase.Moved || Input.touches[i].phase == TouchPhase.Stationary;
                break;
            }
        }
        return isTouch;
#else
        return false;
#endif
    }
    /// <summary>
    /// タッチ終了時かを返す
    /// </summary>
    /// <param name="touchID"></param>
    /// <returns></returns>
    public bool IsTouchEnd(int touchID = -1)
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#elif UNITY_IOS || UNITY_ANDROID
        if(touchID == -1){ return false;}

        bool isEnd = false;
        for(int i = 0; i < Input.touches.Length; i++)
        {
            if(Input.touches[i].fingerId == touchID)
            {
                isEnd = Input.touches[i].phase == TouchPhase.Moved || Input.touches[i].phase == TouchPhase.Ended;
                break;
            }
        }
        return isEnd;
#else
        return false;
#endif

    }

    public int TouchCount
    {
        get
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) ? 1 : 0;
#elif UNITY_IOS || UNITY_ANDROID
            return Input.touchCount;
#else
        return false;
#endif
        }
    }
}
