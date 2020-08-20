using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : SingletonMonoBehaviour<SoundManager> {

    [SerializeField] private List<AudioSource> bgmAudioSourceList = new List<AudioSource>();
    [SerializeField] private AudioSource mainBGMAudio = null;
    [SerializeField] private AudioSource seAudioSource = null;
    [SerializeField] private SoundScriptable soundScriptable = null;

    private bool isInitialized = false;

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(ReadAsset());
	}
	
	public IEnumerator ReadAsset()
    {
        ResourceRequest request = Resources.LoadAsync<SoundScriptable>("EachChapterVoice");
        while (!request.isDone)
        {
            yield return null;
        }
        if (request.asset != null)
        {
            soundScriptable = request.asset as SoundScriptable;
        }
        if (soundScriptable == null)
        {
            yield return StartCoroutine(ReadAsset());
        }
        
        isInitialized = true;
    }

    public void PlayBGM(string soundName, bool isFade = false)
    {
        if (isInitialized)
        {
            var findData = soundScriptable.bgmDatas.Find(x => x.name == soundName);
            if(findData != null)
            {
                bgmAudioSourceList[0].loop = findData.isLoop;
                bgmAudioSourceList[0].clip = findData.audio;
                bgmAudioSourceList[0].Play();
                //for(int i = 0; i < bgmAudioSourceList.Count; i++)
                //{
                //    if (!bgmAudioSourceList[i].isPlaying)
                //    {
                //        bgmAudioSourceList[i].clip = findData.audio;
                //        bgmAudioSourceList[i].Play();
                //        break;
                //    }
                //}
            }
        }
        else
        {
            Debug.Log("初期化が完了していません");
        }
    }
    public void StopBGM()
    {
        if (isInitialized)
        {
            bgmAudioSourceList[0].Stop();
        }
    }

    public IEnumerator PlayBGMFade(AudioSource playAudio,AudioSource backAudio, AudioClip clip,float fadeTime = 1f)
    {

        float elapsedTime = 0f;
        while(elapsedTime < fadeTime)
        {

            yield return null;
        }
    }

    public void PlaySE(string soundName)
    {
        if (isInitialized)
        {
            var findData = soundScriptable.seDatas.Find(x => x.name == soundName);
            if(findData != null)
            {
                if (findData.isPlayOneShot)
                {
                    seAudioSource.PlayOneShot(findData.audio);
                }
                else
                {
                    for (int i = 0; i < bgmAudioSourceList.Count; i++)
                    {
                        if (!bgmAudioSourceList[i].isPlaying)
                        {
                            bgmAudioSourceList[i].clip = findData.audio;
                            bgmAudioSourceList[i].Play();
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log(soundName + "の音声が存在しません");
            }
        }
        else
        {
            Debug.Log("初期化が完了していません");
        }
    }
}

[System.Serializable]
public class BGMData
{
    public string name;
    public AudioClip audio;
    public bool isLoop;
    public float loopBeginTime;//ループして再生を開始する地点
    public float loopEndTime;//ループするタイミング
}

[System.Serializable]
public class SEData
{
    public string name;
    public AudioClip audio;
    public bool isPlayOneShot;
}