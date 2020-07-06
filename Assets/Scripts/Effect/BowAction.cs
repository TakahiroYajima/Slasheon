using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAction : MonoBehaviour {

    [SerializeField] private ArrowObject arrowPref = null;
    [SerializeField] private GameObject arrowShotPositionObj = null;
    [SerializeField] private GameObject mainCameraObj = null;

    private ArrowObject.ArrowHitCallback arrowHitCallback;
    // Use this for initialization
    void Start () {
		
	}

    public void SetArrowHitCallback(ArrowObject.ArrowHitCallback callback)
    {
        arrowHitCallback = callback;
    }

    public void ShotArrow(float power, Vector3 direction)
    {
        direction = direction.normalized;
        //direction = (direction + (mainCameraObj.transform.position - arrowShotPositionObj.transform.position)).normalized;
        ArrowObject arrow = Instantiate(arrowPref, arrowShotPositionObj.transform.position, arrowShotPositionObj.transform.rotation);
        arrow.InitCallbackSetting(arrowHitCallback);
        arrow.transform.LookAt(direction);
        arrow.ShotArrow(power, direction);
    }
}
