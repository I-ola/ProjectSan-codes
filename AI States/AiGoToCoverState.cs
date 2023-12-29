using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class AiGoToCoverState : AiStates
{
    bool checkedCover = false;
    Collider nearestCover;
    float checkBulletPathTimer = 10f;

    public AiStateId GetId()
    {
        return AiStateId.GoToCover;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.goToCoverSpeed;
        agent.animSpeed = agent.config.goToCoverSpeed;
        nearestCover = agent.sensor.GetCovers(agent);
    }

    public void Update(AiAgent agent)
    {
       // Debug.Log(nearestCover.gameObject.name);
        if (nearestCover != null)
        {
            if (!checkedCover)
            {
                Vector3 hideDir = nearestCover.transform.position - agent.gameObject.transform.position;
                Vector3 hidePosition = nearestCover.transform.position + hideDir.normalized * 10f;

                Ray backray = new Ray(hidePosition, -hideDir.normalized * 5f);

                RaycastHit info;

                if (nearestCover.Raycast(backray, out info, 20))
                {
                    agent.navMeshAgent.SetDestination(info.point + hideDir.normalized);
                    checkedCover = true;
                }

            }
           
            if (!agent.navMeshAgent.pathPending && agent.navMeshAgent.remainingDistance < agent.navMeshAgent.stoppingDistance)
            {
                agent.animSpeed = agent.config.idleSpeed;
                agent.stateMachine.ChangeState(AiStateId.CoverAttack);

            }

          
        }
        else 
        {
            if(checkBulletPathTimer > 0)
            {
                agent.animSpeed = agent.config.idleSpeed;
                Vector3 dir = agent.transform.position - agent.health.hitDir;
                //Debug.Log(dir);
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
                if (agent.targeting.HasTarget)
                {
                    
                    agent.stateMachine.ChangeState(AiStateId.Pursue);
                }

                checkBulletPathTimer -= Time.deltaTime;
            }
            else
            {
                agent.stateMachine.ChangeState(AiStateId.Idle);
            }
       
               
        }

       


    }

    public void Exit(AiAgent agent)
    {
        checkBulletPathTimer = 10f;
        checkedCover = false;
    }
    
}
