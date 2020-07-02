using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAction : MonoBehaviour {

    [SerializeField] private ArrowObject arrowPref = null;
    [SerializeField] private GameObject arrowShotPositionObj = null;
	// Use this for initialization
	void Start () {
		
	}
	
	
    public void ShotArrow(float power, Vector3 direction)
    {
        ArrowObject arrow = Instantiate(arrowPref);
        arrow.transform.position = arrowShotPositionObj.transform.position;
        arrow.ShotArrow(power, direction);
    }
}
