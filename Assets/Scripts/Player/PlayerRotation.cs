using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 専用のUIをタッチしてドラッグすることで視点を回転する
/// </summary>
public class PlayerRotation : MonoBehaviour {

    public Vector2 initTouchPos { get; private set; }
    public Vector2 currentTouchPos { get; private set; }
    public Vector3 initRotation { get; private set; }
    [SerializeField] private Image rotationUI = null;
    public const float maxRotation = 90f;
    public float nowDirection { get; private set; }
    public bool isRotationMoving { get { return touchID != -1; } }

    public int touchID { get; private set; }

    private void Start()
    {
        initTouchPos = Vector2.zero;
        currentTouchPos = Vector2.zero;
        initRotation = transform.rotation.eulerAngles;
        nowDirection = 0f;
        touchID = -1;
    }

    public void OnPointerDown(int touch_id)
    {
        touchID = touch_id;
        initTouchPos = InputManager.Instance.GetTouchPosition(touchID);
        initRotation = transform.rotation.eulerAngles;
    }

    public void OnPointerMove()
    {
        if (touchID != -1)
        {
            currentTouchPos = InputManager.Instance.GetTouchPosition(touchID);
            nowDirection = currentTouchPos.x - initTouchPos.x;
            float angle = nowDirection / (rotationUI.rectTransform.sizeDelta.x * 0.5f) * maxRotation;
            //Debug.Log("angle : " + angle + " : " + nowDirection + " : " + rotationUI.rectTransform.sizeDelta.x + " :: " + currentTouchPos.x + " : " + initTouchPos.x);
            transform.rotation = Quaternion.Euler(transform.rotation.x, initRotation.y + angle, transform.rotation.z);
        }
    }

    public void OnPointerUP()
    {
        touchID = -1;
        nowDirection = 0;
        initTouchPos = Vector2.zero;
        initRotation = transform.rotation.eulerAngles;
    }
}
