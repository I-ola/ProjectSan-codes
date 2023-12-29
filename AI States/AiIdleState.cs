using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiIdleState : AiStates
{
    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.idleSpeed;
        agent.animSpeed = agent.config.idleSpeed;
    }

    public void Update(AiAgent agent)
    { 
       if(Random.Range(0, 100) < 10)
        {
            agent.stateMachine.ChangeState(AiStateId.Patrol);
        }
    }

    public void Exit(AiAgent agent)
    {
        
    }

   

   
}
