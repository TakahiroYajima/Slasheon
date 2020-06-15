#if Develop
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, Time.deltaTime * 10f, 0);
    }
}
#endif