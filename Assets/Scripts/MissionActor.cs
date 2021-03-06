﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionActor : MonoBehaviour {

    protected ActorState actorState = new ActorState();//現在のステータス
    protected ActorState initActorState = new ActorState();//本来のステータス

    private void Awake()
    {
        actorState.hp = 30;
        actorState.attack = 1;
        actorState.defence = 0;
        initActorState.hp = 30;
        initActorState.attack = 1;
        initActorState.defence = 0;
    }

    public virtual void Damage(float damage)
    {
        float finalDamage = damage - actorState.defence;
        if(finalDamage <= 0)
        {
            finalDamage = 0;
        }
        actorState.hp -= finalDamage;
        //Debug.Log("damage :: " + finalDamage + " : hp :: " + actorState.hp);
        if (actorState.hp <= 0)
        {
            Death();
        }
    }

    public abstract void Death();

}

public struct ActorState
{
    public float hp;
    public float attack;
    public float defence;
}