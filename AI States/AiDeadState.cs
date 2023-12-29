using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class AiDeadState : AiStates
{
    public AiStateId GetId()
    {
        return AiStateId.Dead;
    }
    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = true;
    }
    public void Update(AiAgent agent)
    {
        DropWeapon(agent);
        DropKey(agent);
        agent.CallOnDestroyAi();
    }

    public void Exit(AiAgent agent)
    {
       
    }
    public void DropWeapon(AiAgent agent)
    {
        var currentWeapon = agent.weapon;
        if (currentWeapon)
        {
            if(currentWeapon.availableAmmo > 300)
            {
                currentWeapon.availableAmmo = 300;
            }
            currentWeapon.isDropped = true;
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            if (currentWeapon.gameObject.GetComponent<Rigidbody>() == false)
            {
                currentWeapon.gameObject.AddComponent<Rigidbody>();
            }
            currentWeapon.gameObject.layer = agent.weaponLayer;
            currentWeapon.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void DropKey(AiAgent agent)
    {
        if(agent.key != null)
        {
            agent.key.gameObject.transform.SetParent(null);
            agent.key.gameObject.GetComponent<BoxCollider>().enabled = true;
            if (agent.key.gameObject.GetComponent<Rigidbody>() == null)
            {
                agent.key.gameObject.AddComponent<Rigidbody>();
            }
        }
    }

}
