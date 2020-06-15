using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUIController : MonoBehaviour {

    [SerializeField] private Image hpGaugeImage = null;
    [SerializeField] private Image staminaGaugeImage = null;

    [SerializeField] private Image weaponButtonBase = null;
    [SerializeField] private Button weapon1Button = null;
    [SerializeField] private Button weapon2Button = null;
    private Vector2 weaponFixedPosition = Vector2.zero;//武器アイコンの定位置

    [SerializeField] private Image pointOfViewRotationImage = null;

    private MissionState nowMissionState = MissionState.Initialize;

	// Use this for initialization
	void Start () {
        weaponButtonBase.gameObject.SetActive(false);
        weaponFixedPosition = weaponButtonBase.rectTransform.anchoredPosition;
        nowMissionState = MissionState.Initialize;
        MissionSceneManager.Instance.SetStateChangeCallback(ChangeState);//ステート切り替え時に処理させるコールバックを登録

        weaponButtonBase.rectTransform.anchoredPosition = new Vector2(weaponFixedPosition.x + weaponButtonBase.rectTransform.sizeDelta.x, weaponFixedPosition.y);
        //weaponButtonBase.gameObject.SetActive(false);
        Debug.Log("size : " + weaponButtonBase.rectTransform.sizeDelta.x);
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
                break;
            case MissionState.GameOver:
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
        Vector2 beginPosition = weaponButtonBase.rectTransform.anchoredPosition;
        float moveDistance = weaponButtonBase.rectTransform.sizeDelta.x;
        float moveSpeed = 2000f;
        while(Mathf.Abs(Mathf.Abs(beginPosition.x) - Mathf.Abs(weaponButtonBase.rectTransform.anchoredPosition.x)) < moveDistance)
        {
            if (weaponButtonBase.rectTransform.anchoredPosition.x + Time.deltaTime * moveSpeed > moveDistance)
            {
                weaponButtonBase.rectTransform.anchoredPosition = new Vector2(beginPosition.x + moveDistance, beginPosition.y);
            }
            else
            {
                weaponButtonBase.rectTransform.anchoredPosition += new Vector2(Time.deltaTime * moveSpeed, 0f);
                yield return null;
            }
        }
        weaponButtonBase.gameObject.SetActive(false);
    }

    /// <summary>
    /// プレイヤーが敵とエンカウントした際、武器アイコンのUIを左へスライドしてフレームイン
    /// </summary>
    /// <returns></returns>
	public IEnumerator ToEncountAction()
    {
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
                nowMoveDistance += Time.deltaTime * moveSpeed;
                weaponButtonBase.rectTransform.anchoredPosition = new Vector2(beginPosition.x - nowMoveDistance, weaponButtonBase.rectTransform.anchoredPosition.y);
                yield return null;
            }
        }
    }
}
