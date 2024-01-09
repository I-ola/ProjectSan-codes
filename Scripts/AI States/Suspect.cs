using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiSuspectState : AiStates
{

    public AiStateId GetId()
    {
        return AiStateId.Suspect;
    }

    public void Enter(AiAgent agent)
    {
        agent.soundManager.checkedLocation = true;
        agent.navMeshAgent.speed = agent.config.suspectWalkingSpeed;
        agent.animSpeed = agent.config.suspectWalkingSpeed;
    }

    public void Update(AiAgent agent)
    {
        if (agent.soundManager.intensity >= agent.config.checkLocationIntensity)
        {
           // Debug.Log("Going to check out Suspect Location");
            agent.navMeshAgent.SetDestination(agent.soundManager.noticedSoundLocation);

            if (agent.targeting.HasTarget)
            {
                agent.stateMachine.ChangeState(AiStateId.Pursue);
            }
            else if(!agent.navMeshAgent.hasPath)
            {
                agent.stateMachine.ChangeState(AiStateId.Idle);
            }
        }
        else if (agent.soundManager.intensity >= agent.config.scanLocationIntensity)
        {
            //Debug.Log("Scan Suspect Location");
            agent.animSpeed = agent.config.idleSpeed;
            Vector3 dir = agent.soundManager.noticedSoundLocation - agent.transform.position;
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

            if (agent.targeting.HasTarget)
            {
                agent.stateMachine.ChangeState(AiStateId.Pursue);
            }
            else
            {
                agent.stateMachine.ChangeState(AiStateId.Idle);
            }
        }
        
     

     
    }

    public void Exit(AiAgent agent)
    {
        agent.soundManager.checkedLocation = false;
    }
  

   
}
