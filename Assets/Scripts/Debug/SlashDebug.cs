#if Develop
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashDebug : MonoBehaviour {
    [SerializeField] private MeshSlashEffect slash = null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        slash.UpdateAction();
	}
}
#endif