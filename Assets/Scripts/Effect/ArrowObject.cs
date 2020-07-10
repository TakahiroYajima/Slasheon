using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowObject : MonoBehaviour {

    [SerializeField] private Rigidbody myRigidbody = null;

    public delegate void ArrowHitCallback(Collider collider, float attackPower);
    private ArrowHitCallback arrowHitCallback;
    private Vector3 forwardDirection = Vector3.zero;
    private float attackPower = 0f;

    private float elapsedTime = 0f;

    public void InitCallbackSetting(ArrowHitCallback callback)
    {
        arrowHitCallback = callback;
    }
    public void ShotArrow(float power, Vector3 direction, float arrowAttackPower)
    {
        Debug.Log("shotArrow");
        attackPower = arrowAttackPower;
        forwardDirection = new Vector3(direction.x, 0f, direction.z);

        Vector3 shotDir = direction * power;
        myRigidbody.AddForce(shotDir);
    }

    public void Update()
    {
        if (forwardDirection != Vector3.zero)
        {
            if (elapsedTime < 2f)
            {
                myRigidbody.AddForce(forwardDirection * 200f);
            }
            else if(elapsedTime >= 2f)
            {
                myRigidbody.AddForce(Vector3.down * 200f);
            }
            if(elapsedTime > 5f)
            {
                Destroy(this.gameObject);
            }
            elapsedTime += Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        arrowHitCallback(other, attackPower);
        Destroy(this.gameObject);
    }
}
