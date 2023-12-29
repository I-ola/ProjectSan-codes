using System;
using UnityEngine;
using UnityEngine.AI;

public class AiPatrolState : AiStates
{
    int index;
    float time = 0.0f;
    public AiStateId GetId()
    {
        return AiStateId.Patrol;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.patrolSpeed;
        agent.animSpeed = agent.config.patrolSpeed;
        agent.navMeshAgent.stoppingDistance = agent.config.normalStoppingDistance;
        CalculatePath(agent);
    }

    public void Update(AiAgent agent)
    {
       
        if (agent.navMeshAgent.remainingDistance < agent.navMeshAgent.stoppingDistance)
        {
            agent.animSpeed = agent.config.idleSpeed;
            time += Time.deltaTime;
            if (time > 5.0f)
            {
                SetWay(agent);
                time = 0.0f;

            }
        }
  
            
        
        CanSeePlayer(agent);
    }

    public void Exit(AiAgent agent)
    {
       
    }

   
   
    void CanSeePlayer(AiAgent agent)
    {
        if (agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.Pursue);
        }
    }
    
        

    

    public void SetWay(AiAgent agent)
    {   
        if (!agent.navMeshAgent.pathPending && agent.navMeshAgent.remainingDistance < agent.navMeshAgent.stoppingDistance)
        {
            if (index >= agent.pathObj.Length - 1)
            {
                index = 0;

            }
            else
            {
                index++;
            }
            agent.navMeshAgent.destination = agent.pathObj[index].transform.position;
            agent.animSpeed = agent.config.patrolSpeed;
        }
    }

    public void CalculatePath(AiAgent agent)
    {
        
        float otherDist = Mathf.Infinity;
        for (int i = 0; i < agent.pathObj.Length; i++)
        {
            GameObject wp = agent.pathObj[i];
            float distance = Vector3.Distance(agent.transform.position, wp.transform.position);
            if (distance < otherDist)
            {
            index = i - 1;
            otherDist = distance;
            }
        }



    }
}

  


