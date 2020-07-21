using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : SingletonMonoBehaviour<InputManager> {

    //private int doubleTouchID = -1;
    //private float doubleTouchJudgeTime = 0.2f;//ダブルタッチと判定するタイム

    private Vector2 prevFrameMousePos;
    private float prevMovePosResetTime = 0.3f;
    private float movePosResetTimeProgress = 0f;

    private TouchPhase[] touchPhases;

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }
    // Use this for initialization
    void Start () {
        prevFrameMousePos = Vector2.zero;
        touchPhases = new TouchPhase[Input.touches.Length];
        for(int i = 0; i < touchPhases.Length; i++)
        {
            touchPhases[i] = Input.touches[i].phase;
        }
    }
	
    private void LateUpdate()
    {
        touchPhases = new TouchPhase[Input.touches.Length];
        for (int i = 0; i < touchPhases.Length; i++)
        {
            touchPhases[i] = Input.touches[i].phase;
        }

        if (movePosResetTimeProgress >= prevMovePosResetTime)
        {
            prevFrameMousePos = Input.mousePosition;
            movePosResetTimeProgress = 0f;
        }
        else
        {
            movePosResetTimeProgress += Time.deltaTime;
        }
        //全てのタッチ情報を更新
        for (int i = 0; i < touchPhases.Length; i++)
        {
            touchPhases[i] = Input.touches[i].phase;
        }
    }

    public int GetAnyTouchBeginID()
    {
#if UNITY_IOS || UNITY_ANDROID
        Debug.Log("GetAnyTouchBeginID : touchLength :: " + touchPhases.Length);
        for (int i = 0; i < touchPhases.Length; i++)
        {
            if(touchPhases[i] != Input.touches[i].phase && touchPhases[i] == TouchPhase.Began)
            {
                return Input.touches[i].fingerId;
            }
        }
        return -1;
#else
        if (Input.GetMouseButtonDown(0))
        {
            return 0;
        }
        return -1;
#endif
    }

    /// <summary>
    /// タッチ開始時かを返す
    /// </summary>
    /// <param name="touchID"></param>
    /// <returns></returns>
    public bool IsTouchDown(int touchID = -1)
    {
#if UNITY_IOS || UNITY_ANDROID
        Debug.Log("isTouchDown :: id : " + touchID);
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
#elif UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
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
#if UNITY_IOS || UNITY_ANDROID
        Debug.Log("isTouch :: id : " + touchID);
        if (touchID == -1){ return false;}

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
#elif UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        return false;
#endif
    }
    public bool IsTouchMove(int touchID = -1)
    {
#if UNITY_IOS || UNITY_ANDROID
        Debug.Log("isTouchMove :: id : " + touchID);
        if (touchID == -1) { return false; }

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
#elif UNITY_EDITOR
        return Input.GetMouseButton(0) && Vector2.Distance(Input.mousePosition, prevFrameMousePos) >= 0.03f;
#endif
    }
    /// <summary>
    /// タッチ終了時かを返す
    /// </summary>
    /// <param name="touchID"></param>
    /// <returns></returns>
    public bool IsTouchEnd(int touchID = -1)
    {
#if UNITY_IOS || UNITY_ANDROID
        Debug.Log("isTouchEnd :: id : " + touchID);
        if (touchID == -1){ return false;}

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
#elif UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#else
        return false;
#endif

    }

    public int TouchCount
    {
        get
        {
#if UNITY_IOS || UNITY_ANDROID
            Debug.Log("touchCount : " + Input.touchCount);
            return Input.touchCount;
#elif UNITY_EDITOR
            return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) ? 1 : 0;
#else
        return false;
#endif
        }
    }

    public Vector3 GetTouchPosition(int touchID)
    {
#if UNITY_IOS || UNITY_ANDROID
            for(int i = 0; i < Input.touches.Length; i++)
            {
                if(Input.touches[i].fingerId == touchID)
                {
                    return Input.touches[i].position;
                }
            }
        return Vector3.zero;
#elif UNITY_EDITOR
        return Input.mousePosition;
#else
        return false;
#endif
    }
    /// <summary>
    /// UIをタッチしているかを返す
    /// </summary>
    /// <param name="touchID"></param>
    /// <returns></returns>
    public bool IsUITouch(int touchID)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
#if UNITY_IOS || UNITY_ANDROID
        Debug.Log("IsUITouch :: id : " + touchID);
        Touch touch = new Touch();
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].fingerId == touchID)
            {
                touch = Input.touches[i];
                pointer.position = touch.position;
                List<RaycastResult> result = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, result);

                //レイヤーでUIかを判定して整理する
                for (int j = 0; j < result.Count; j++)
                {
                    if (SlasheonUtility.IsAnyLayerNameMatch(result[i].gameObject, SlasheonUtility.UILayer))
                    {
                        Debug.Log("UITouch:True  id : " + touchID);
                        return true;
                    }
                }
            }
        }
        return false;
#else
        pointer.position = Input.mousePosition;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, result);

        //レイヤーでUIかを判定して整理する
        for (int i = 0; i < result.Count; i++)
        {
            //一致していなければリストから除く
            if (SlasheonUtility.IsAnyLayerNameMatch(result[i].gameObject, SlasheonUtility.UILayer))
            {
                return true;
            }
        }
        return false;
#endif
    }

    /// <summary>
    /// Raycastで取得したUIを返す
    /// </summary>
    /// <returns></returns>
    public RaycastResult GetRaycastResult(int touchID)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
#if UNITY_IOS || UNITY_ANDROID
        Touch touch = new Touch();
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].fingerId == touchID)
            {
                touch = Input.touches[i];
                pointer.position = touch.position;
                List<RaycastResult> result = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, result);

                List<int> removeNums = new List<int>();
                //レイヤーでUIかを判定して整理する
                for (int j = 0; j < result.Count; j++)
                {
                    //一致していなければリストから除く
                    if (SlasheonUtility.IsAnyLayerNameMatch(result[j].gameObject, SlasheonUtility.UILayer))
                    {
                        removeNums.Add(i);
                    }
                }
                for (int k = removeNums.Count - 1; k > 0; k--)
                {
                    result.RemoveAt(k);
                }

                if (result.Count > 0)
                    return result[0];
                else
                    return new RaycastResult();
            }
        }
        return new RaycastResult();
#else
        pointer.position = Input.mousePosition;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, result);

        List<int> removeNums = new List<int>();
        //レイヤーでUIかを判定して整理する
        for (int i = 0; i < result.Count; i++)
        {
            //一致していなければリストから除く
            if (SlasheonUtility.IsAnyLayerNameMatch(result[i].gameObject, SlasheonUtility.UILayer))
            {
                removeNums.Add(i);
            }
        }
        for (int i = removeNums.Count - 1; i > 0; i--)
        {
            //Debug.Log("取り除きます " + i);
            result.RemoveAt(i);
        }

        if (result.Count > 0)
            return result[0];
        else
            return new RaycastResult();
#endif
    }
}
