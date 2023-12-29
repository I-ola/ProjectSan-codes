using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AiAttackState : AiStates
{
    public AiStateId GetId()
    {
        return AiStateId.Attack;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = agent.config.attackStoppingDistance;
        agent.navMeshAgent.speed = agent.config.attackSpeed;
        agent.animSpeed = agent.config.attackSpeed;


    }

    public void Update(AiAgent agent)
    {
        //agent.sensor.CallOtherAi();

        ActionToPerform(agent);

    }

    void ActionToPerform(AiAgent agent)
    {
        if (agent.targeting.TargetInSight)
        {
            //there is a present target that can be attacked
           // Debug.Log(agent.gameObject.name + "HAS A TARGET" + agent.targeting.Target.gameObject.name);
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
            AttackPlayer(agent);
            if(agent.navMeshAgent.remainingDistance < agent.navMeshAgent.stoppingDistance)
                agent.animSpeed = agent.config.stopSpeed;
            else
                agent.animSpeed = agent.config.attackSpeed;
        }
        else if (agent.targeting.HasTarget)
        {
            //Debug.Log(agent.gameObject.name + "HAS TO CHASE TARGET" + agent.targeting.Target.gameObject.name);
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
            if (agent.navMeshAgent.remainingDistance < agent.navMeshAgent.stoppingDistance)
                agent.animSpeed = agent.config.stopSpeed;
            else
                agent.animSpeed = agent.config.attackSpeed;
            return;

        }
        else
        {
            agent.stateMachine.ChangeState(AiStateId.Patrol);
        }
    }
    public void Exit(AiAgent agent)
    {
        agent.weaponIK.SetTargetTransform(null, agent);
        agent.weaponIK.SetAimTransform(null);
        agent.navMeshAgent.stoppingDistance = agent.config.normalStoppingDistance;
    }


    private void AttackPlayer(AiAgent agent)
    {
        //agent.sensor.KeepDistance();
        agent.target = agent.targeting.Target.transform;
        Vector3 dir = agent.targeting.TargetPosition - agent.transform.position;
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        Vector3 position;
        agent.weaponIK.SetTargetTransform(agent.target, agent);
        agent.weaponIK.SetAimTransform(agent.weapon.bulletPathRaycast);
        if (agent.weapon.ammoCount > 0)
        {
            SetFiring(true, agent);
            float targetColliderHeight = agent.sensor.GetColliderHeight(agent.target.GetComponent<Collider>());
            position = agent.target.position;
            position.y = targetColliderHeight;

            position += Random.insideUnitSphere * 0.4f;

            agent.weapon.UpdateWeapon(Time.deltaTime, position);

        }
    }
    public void SetFiring(bool enabled, AiAgent agent)
    {
        if (enabled)
        {
            agent.weapon.StartFiring();
            //Debug.Log("presently shooting");
        }
        else
        {
            agent.weapon.StopFiring();
           // Debug.Log("Not shooting again");
        }
    }


}
