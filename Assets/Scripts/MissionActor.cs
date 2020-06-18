using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionActor : MonoBehaviour {

    protected ActorState actorState = new ActorState();

    private void Awake()
    {
        actorState.hp = 3;
        actorState.attack = 1;
        actorState.defence = 0;
    }

    public virtual void Damage(int damage)
    {
        int finalDamage = damage - actorState.defence;
        if(finalDamage <= 0)
        {
            finalDamage = 1;
        }
        actorState.hp -= finalDamage;
        Debug.Log("damage :: " + finalDamage + " : hp :: " + actorState.hp);
        if (actorState.hp <= 0)
        {
            Death();
        }
    }

    public abstract void Death();

}

public struct ActorState
{
    public int hp;
    public int attack;
    public int defence;
}