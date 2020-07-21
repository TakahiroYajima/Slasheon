using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionStaminaManager : MonoBehaviour {

    [SerializeField] private Image staminaGaugeImage = null;
    [SerializeField] private Text staminaText = null;
    [SerializeField] private Text remainingTimeText = null;

	// Use this for initialization
	void Start () {
        UserStatusManager.Instance.SetUpdateTimeCallback(UpdateTime);
        UserStatusManager.Instance.SetStaminaRecoverCallback(UpdateStamina);

        UserStatusManager.Instance.RecoverStaminaSaveData(UpdateStamina);
        UpdateTime(UserStatusManager.Instance.nextRecoverTime);
        UpdateStamina(UserStatusManager.Instance.maxStamina, UserStatusManager.Instance.currentStamina);
	}
	
	// Update is called once per frame
	void Update () {
        UserStatusManager.Instance.UpdateAction();
	}
    
    public void UpdateTime(float remainingTime)
    {
        remainingTimeText.text = "あと " + remainingTime.ToString();
    }
    public void UpdateStamina(float max, float current)
    {
        staminaText.text = current.ToString() + "/" + max.ToString();
        staminaGaugeImage.fillAmount = current / max;
    }
}
