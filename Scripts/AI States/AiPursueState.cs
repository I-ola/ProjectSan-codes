using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiPursueState : AiStates
{
   
    public AiStateId GetId()
    {
        return AiStateId.Pursue;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.pursueSpeed;
        agent.animSpeed = agent.config.pursueSpeed;
    }

    public void Update(AiAgent agent)
    {
        //agent.sensor.CallOtherAi();
        if (agent.targeting.HasTarget)
        {
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
            if (agent.targeting.TargetInSight)
            {
                agent.stateMachine.ChangeState(AiStateId.Attack);
            }
        }
        

      
    }

    public void Exit(AiAgent agent)
    {
        
    }

    
}
