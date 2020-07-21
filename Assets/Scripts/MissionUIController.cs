using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MissionUIController : MonoBehaviour {

    [SerializeField] private Image hpGaugeImage = null;
    [SerializeField] private Image staminaGaugeImage = null;

    [SerializeField] private Image weaponButtonBase = null;
    [SerializeField] private Button weapon1Button = null;
    [SerializeField] private Button weapon2Button = null;
    private Vector2 weaponFixedPosition = Vector2.zero;//武器アイコンの定位置

    [SerializeField] private Image pointOfViewRotationImage = null;
    public float pointOfViewWidth { get { return pointOfViewRotationImage.rectTransform.sizeDelta.x; } }//視点回転UIの横幅の半分の長さ
    public const float pointOfViewMaxRotation = 90f;//一度に視点回転できる最大値

    //リザルトUI
    [SerializeField] private Image resultBaseImage = null;
    [SerializeField] private Button resultOKButton = null;
    [SerializeField] private GameObject resultBackground = null;

    //ダメージUI
    [SerializeField] private Image damageUI = null;

    //ゲームオーバーUI
    [SerializeField] private Image gameOverBackPanel = null;
    [SerializeField] private Text gameOverText = null;
    [SerializeField] private GameObject gameOverMenuUI = null;

    private MissionState nowMissionState = MissionState.Initialize;

    //コルーチン
    private IEnumerator damageUIActionCoroutine = null;

	// Use this for initialization
	void Start () {
        weaponButtonBase.gameObject.SetActive(false);
        weaponFixedPosition = weaponButtonBase.rectTransform.anchoredPosition;
        nowMissionState = MissionState.Initialize;
        MissionSceneManager.Instance.SetStateChangeCallback(ChangeState);//ステート切り替え時に処理させるコールバックを登録

        weaponButtonBase.rectTransform.anchoredPosition = new Vector2(weaponFixedPosition.x + weaponButtonBase.rectTransform.sizeDelta.x, weaponFixedPosition.y);
        //weaponButtonBase.gameObject.SetActive(false);
        //Debug.Log("size : " + weaponButtonBase.rectTransform.sizeDelta.x);

        //リザルトのOKボタンタップ時のアクション
        resultOKButton.onClick.RemoveAllListeners();
        resultOKButton.onClick.AddListener(() =>
        {
            MissionSceneManager.Instance.ChangeMissionState(MissionState.Expedition);
        });

        damageUI.gameObject.SetActive(false);
        gameOverBackPanel.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        gameOverMenuUI.SetActive(false);
    }

    /// <summary>
    /// ボタン1のコールバック登録
    /// </summary>
    /// <param name="callback"></param>
    public void SetWeapon1PushCallback(UnityAction callback)
    {
        weapon1Button.onClick.RemoveAllListeners();
        weapon1Button.onClick.AddListener(callback);
    }
    /// <summary>
    /// ボタン2のコールバック登録
    /// </summary>
    /// <param name="callback"></param>
    public void SetWeapon2PushCallback(UnityAction callback)
    {
        weapon2Button.onClick.RemoveAllListeners();
        weapon2Button.onClick.AddListener(callback);
    }

    public void ChangeState(MissionState state)
    {
        switch (state)
        {
            case MissionState.Expedition:
                if(nowMissionState == MissionState.Result)
                {
                    StartCoroutine(ToExpeditionAction());
                }
                break;
            case MissionState.Encount:
                if(nowMissionState == MissionState.Expedition)
                {
                    StopCoroutine(ToExpeditionAction());
                    StartCoroutine(ToEncountAction());
                }
                break;
            case MissionState.Battle:
                break;
            case MissionState.Result:
                StartCoroutine(ToResultAction());
                break;
            case MissionState.GameOver:
                StartCoroutine(GameOverUIAction());
                break;

        }
        nowMissionState = state;
    }
	
    /// <summary>
    /// 探索モードへ移行する際、武器アイコンのUIを右へスライドしてフレームアウト
    /// </summary>
    /// <returns></returns>
	public IEnumerator ToExpeditionAction()
    {
        //リザルトを非表示に
        resultOKButton.enabled = false;
        resultBaseImage.gameObject.SetActive(false);
        resultBackground.SetActive(false);

        Vector2 beginPosition = weaponButtonBase.rectTransform.anchoredPosition;
        float moveDistance = weaponButtonBase.rectTransform.sizeDelta.x;
        float moveSpeed = 2000f;
        float nowMoveDistance = 0f;
        while (nowMoveDistance < moveDistance)
        {
            if (nowMissionState == MissionState.Encount)
            {
                nowMoveDistance = moveDistance;
            }
            else
            {
                if (nowMoveDistance + Time.deltaTime * moveSpeed >= moveDistance)
                {
                    weaponButtonBase.rectTransform.anchoredPosition = new Vector2(beginPosition.x + moveDistance, beginPosition.y);
                    weaponButtonBase.gameObject.SetActive(false);
                }
                else
                {
                    weaponButtonBase.rectTransform.anchoredPosition = new Vector2(beginPosition.x + nowMoveDistance, weaponButtonBase.rectTransform.anchoredPosition.y);
                    yield return null;
                }
                nowMoveDistance += Time.deltaTime * moveSpeed;
            }
        }
        //weaponButtonBase.gameObject.SetActive(false);
    }

    /// <summary>
    /// プレイヤーが敵とエンカウントした際、武器アイコンのUIを左へスライドしてフレームイン
    /// </summary>
    /// <returns></returns>
	public IEnumerator ToEncountAction()
    {
        StopCoroutine(ToExpeditionAction());
        Vector2 beginPosition = weaponButtonBase.rectTransform.anchoredPosition;
        float moveDistance = Mathf.Abs(weaponFixedPosition.x - beginPosition.x);
        float moveSpeed = 2000f;
        float nowMoveDistance = 0f;
        weaponButtonBase.gameObject.SetActive(true);
        while (weaponButtonBase.rectTransform.anchoredPosition.x > weaponFixedPosition.x)
        {
            if (nowMoveDistance + Time.deltaTime * moveSpeed >= moveDistance)
            {
                weaponButtonBase.rectTransform.anchoredPosition = weaponFixedPosition;
            }
            else
            {
                weaponButtonBase.rectTransform.anchoredPosition = new Vector2(beginPosition.x - nowMoveDistance, weaponButtonBase.rectTransform.anchoredPosition.y);
                yield return null;
            }
            nowMoveDistance += Time.deltaTime * moveSpeed;
        }
    }

    public IEnumerator ToResultAction()
    {
        resultBackground.SetActive(true);
        resultOKButton.enabled = true;
        resultBaseImage.gameObject.SetActive(true);
        yield return null;

    }

    public void SetHP(float hp, float maxHP)
    {
        hpGaugeImage.fillAmount = hp / maxHP;
    }
    public void SetStamina(float stamina, float maxStamina)
    {
        staminaGaugeImage.fillAmount = stamina / maxStamina;
    }

    public IEnumerator DamageUIAction()
    {
        //if(damageUIActionCoroutine != null)
        //{
        //    StopCoroutine(damageUIActionCoroutine);
        //    yield return null;
        //}
        //damageUIActionCoroutine = DamageUIAnimation();
        //StartCoroutine(damageUIActionCoroutine);
        StopCoroutine(DamageUIAnimation());
        yield return null;
        StartCoroutine(DamageUIAnimation());
    }
    private IEnumerator DamageUIAnimation(float animationTime = 0.2f)
    {
        Color color = new Color(damageUI.color.r, damageUI.color.g, damageUI.color.b, 0f);
        damageUI.color = color;
        animationTime = animationTime / 2f;
        damageUI.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            if(elapsedTime + Time.deltaTime >= animationTime)
            {
                elapsedTime = animationTime;
            }
            float alpha = elapsedTime / animationTime;
            color.a = alpha;
            damageUI.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            if (elapsedTime + Time.deltaTime >= animationTime)
            {
                elapsedTime = animationTime;
            }
            float alpha = 1f - elapsedTime / animationTime;
            color.a = alpha;
            damageUI.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        damageUI.gameObject.SetActive(false);
    }

    public IEnumerator GameOverUIAction()
    {
        Color color = new Color(gameOverBackPanel.color.r, gameOverBackPanel.color.g, gameOverBackPanel.color.b, 0f);
        gameOverBackPanel.color = color;
        float animationTime = 1f;
        gameOverBackPanel.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            if (elapsedTime + Time.deltaTime >= animationTime)
            {
                elapsedTime = animationTime;
            }
            float alpha = elapsedTime / animationTime;
            color.a = alpha;
            gameOverBackPanel.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, 0f);
        gameOverText.color = color;
        animationTime = 2f;
        gameOverText.gameObject.SetActive(true);
        elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            if (elapsedTime + Time.deltaTime >= animationTime)
            {
                elapsedTime = animationTime;
            }
            float alpha = elapsedTime / animationTime;
            color.a = alpha;
            gameOverText.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        gameOverMenuUI.SetActive(true);
    }
}