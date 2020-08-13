using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllManager : SingletonMonoBehaviour<SceneControllManager> {

    private bool isDebugSceneLoadInitializeAction = false;//デバッグ時、ブートシーンをロードせずに他のシーンから開始した場合、ブートシーンに戻ってからそのシーンに戻るためのフラグ
    public bool IsDebugSceneLoadInitializeAction { get { return isDebugSceneLoadInitializeAction; } }

    [SerializeField] private GameObject darkScreenCanvas = null;//フェード、ローディングの親Canvas
    [SerializeField] private GameObject loadingObject = null;//Loadingの親Object
    [SerializeField] private Image fadePanelImage = null;//フェード用パネル
    [SerializeField] private Image loadingGauge = null;//ロード中のアニメーションゲージ
    [SerializeField] private Text loadingAnimationText = null;//ロード中テキスト

    [SerializeField] private float normalFadeTime = 1f;//通常のフェード時間

    private bool isLoading = false;
    public bool IsLoading { get { return isLoading; } }

    public string loadStageID = "";

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }
    // Use this for initialization
    void Start () {
		
	}
	
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ChangeSceneAsync(string sceneName, bool isActiveLoadUI = false, bool isFadeOut = false, bool isFadeIn = false)
    {
        StartCoroutine(LoadSceneAsync(sceneName, isActiveLoadUI, isFadeOut, isFadeIn));
    }
    private IEnumerator LoadSceneAsync(string sceneName, bool isActiveLoadUI = false, bool isFadeOut = false, bool isFadeIn = false)
    {
        isLoading = true;

        float elapsedTime = 0f;//ロード中の経過時間

        darkScreenCanvas.gameObject.SetActive(isActiveLoadUI || isFadeIn || isFadeOut);
        
        if (isFadeOut)
        {
            yield return StartCoroutine(FadeImage(FadeMode.Out));
        }

        loadingObject.SetActive(isActiveLoadUI);
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
        while (!loadAsync.isDone)
        {
            Debug.Log("loading");
            if (isActiveLoadUI)
            {
                //Loadingの右の「...」の文字数が増えていくアニメーション
                if (elapsedTime < 0.3f)
                {
                    loadingAnimationText.text = "Loading.";
                }else if(elapsedTime < 0.6f)
                {
                    loadingAnimationText.text = "Loading..";
                }else if(elapsedTime < 0.9f)
                {
                    loadingAnimationText.text = "Loading...";
                }else
                {
                    elapsedTime = 0f;
                }
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(loadAsync.progress / 0.9f);
                loadingGauge.fillAmount = progress;
            }
            yield return null;
        }
        loadingObject.SetActive(false);

        if (isFadeIn)
        {
            yield return StartCoroutine(FadeImage(FadeMode.In));
        }
        darkScreenCanvas.SetActive(false);
        isLoading = false;
    }

    public IEnumerator FadeImage(FadeMode mode, float exceptionFadeTime = -1f)
    {
        Color color = new Color();
        float time = 0f;
        float fadeTime = 0f;
        if(exceptionFadeTime == -1f)
        {
            fadeTime = normalFadeTime;
        }
        else
        {
            fadeTime = exceptionFadeTime;
        }
        if(mode == FadeMode.In)
        {
            fadePanelImage.color = new Color(0f, 0f, 0f, 1f);
            fadePanelImage.gameObject.SetActive(true);
            color = fadePanelImage.color;
            while (color.a > 0f)
            {
                color.a = 1f - time / fadeTime;
                fadePanelImage.color = color;
                time += Time.deltaTime;

                yield return null;
            }
            fadePanelImage.gameObject.SetActive(false);
        }
        else
        {
            fadePanelImage.color = new Color(0f, 0f, 0f, 0f);
            fadePanelImage.gameObject.SetActive(true);
            color = fadePanelImage.color;
            while (color.a < 1f)
            {
                color.a = time / fadeTime;
                fadePanelImage.color = color;
                time += Time.deltaTime;
                yield return null;
            }
        }
    }

#if Develop
    /// <summary>
    /// 開発用、ブートシーン以外から開始した際、ブートシーンをロードしてから戻るアクション
    /// </summary>
    /// <param name="backSceneName"></param>
    public void LoadBootSceneAndBackScene(string backSceneName)
    {
        isDebugSceneLoadInitializeAction = true;
        SceneManager.LoadScene("BootScene");
        StartCoroutine(BootAction(backSceneName));
    }
    private IEnumerator BootAction(string backSceneName)
    {
        yield return null;
        BootSceneManager.Instance.BootSceneInitAndBackScene(backSceneName);
        isDebugSceneLoadInitializeAction = false;
    }
#endif
}

public enum FadeMode
{
    In,
    Out,
}