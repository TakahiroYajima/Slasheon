using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlashDamageEffect : MonoBehaviour {

    [SerializeField] private Image animationImage = null;
    private float animationTime = 0.25f;
    private float scaleUpTime = 0.05f;
    private float maxScale = 3.5f;
	// Use this for initialization
	public IEnumerator StartAction (Quaternion rotation) {
        animationImage.rectTransform.localRotation = rotation;
        float elapsedTime = 0f;
        while(elapsedTime < animationTime)
        {
            float scale = 0f;
            if(elapsedTime < scaleUpTime)
            {
                scale = elapsedTime / scaleUpTime * maxScale;
            }
            else
            {
                scale = maxScale - (elapsedTime - scaleUpTime) / (animationTime - scaleUpTime) * maxScale;
            }
            animationImage.rectTransform.localScale = new Vector3(1f,scale,1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
	}
	
	
}
