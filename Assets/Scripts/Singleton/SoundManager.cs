using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager> {

    [SerializeField] private List<AudioSource> bgmAudioSourceList = new List<AudioSource>();
    [SerializeField] private AudioSource seAudioSource = null;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
