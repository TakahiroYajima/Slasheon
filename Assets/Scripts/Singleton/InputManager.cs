using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonoBehaviour<InputManager> {

    private int doubleTouchID = -1;
    private float doubleTouchJudgeTime = 0.2f;//ダブルタッチと判定するタイム

    private Vector2 prevFrameMousePos;
    private float prevMovePosResetTime = 0.3f;
    private float movePosResetTimeProgress = 0f;

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }
    // Use this for initialization
    void Start () {
        prevFrameMousePos = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {

    }
    private void LateUpdate()
    {
        if (movePosResetTimeProgress >= prevMovePosResetTime)
        {
            prevFrameMousePos = Input.mousePosition;
            movePosResetTimeProgress = 0f;
        }
        else
        {
            movePosResetTimeProgress += Time.deltaTime;
        }
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
                isTouch = Input.touches[i].phase == TouchPhase.Stationary;
                break;
            }
        }
        return isTouch;
#else
        return false;
#endif
    }
    public bool IsTouchMove(int touchID = -1)
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0) && Vector2.Distance(Input.mousePosition, prevFrameMousePos) >= 0.03f;
#elif UNITY_IOS || UNITY_ANDROID
        if(touchID == -1) { return false; }

        bool isTouch = false;
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].fingerId == touchID)
            {
                isTouch = Input.touches[i].phase == TouchPhase.Moved;
                break;
            }
        }
        return isTouch;
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

    public Vector3 GetTouchPosition(int touchID)
    {
#if UNITY_EDITOR
            return Input.mousePosition;
#elif UNITY_IOS || UNITY_ANDROID
            for(int i = 0; i < Input.touches.Length; i++)
            {
                if(Input.touches[i].fingerId == touchID)
                {
                    return Input.touches[i].position;
                }
            }
        return Vector3.zero;
#else
        return false;
#endif
    }
}
