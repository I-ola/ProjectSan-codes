using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiDeadState : AiStates
{
    bool hasReset = false;
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
        agent.sensor.CallOtherAi();
        DropWeapon(agent);
        DropKey(agent);
        if(agent.gameObject.activeSelf) 
            agent.CallOnDestroyAi();
        //Debug.Log(agent.weapon.availableAmmo);
    }

    public void Exit(AiAgent agent)
    {
       
    }
    public void DropWeapon(AiAgent agent)
    {
   
        if(agent.weapon != null)
        {
            if (agent.weapon.availableAmmo > 300 && !hasReset)
            {
                agent.weapon.availableAmmo = 300;
             
                hasReset = true;
            }

            if(agent.weapon.availableAmmo <= 300)
            {
                agent.weapon.isDropped = true;
                agent.weapon.transform.SetParent(null);   
                agent.weapon.gameObject.GetComponent<BoxCollider>().enabled = true;
                if (agent.weapon.gameObject.GetComponent<Rigidbody>() == false)
                {
                    agent.weapon.gameObject.AddComponent<Rigidbody>();
                }
                agent.weapon.gameObject.layer = agent.weaponLayer;
                agent.weapon.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                agent.weapon = null;
            }
   
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
            agent.key = null;
        }
    }

}
