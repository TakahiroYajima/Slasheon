using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowObject : MonoBehaviour {

    [SerializeField] private Rigidbody myRigidbody = null;

    public delegate void ArrowHitCallback(Collider collider);
    private ArrowHitCallback arrowHitCallback;
	
    public void InitCallbackSetting(ArrowHitCallback callback)
    {
        arrowHitCallback = callback;
    }
    public void ShotArrow(float power, Vector3 direction)
    {
        Vector3 shotDir = direction * power;
        myRigidbody.AddForce(shotDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        arrowHitCallback(other);
        Destroy(this.gameObject);
    }
}
