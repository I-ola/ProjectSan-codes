using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AiCoverAttackState : AiStates
{
    float crouchFloat = 0.0f;
    float attackTimer = 0.0f;
    float timer = 20.0f;
    float crouchDuration = 6f;
    bool crouched = true;


    public AiStateId GetId()
    {
        return AiStateId.CoverAttack;
    }

    public void Enter(AiAgent agent)
    {
      
        agent.animator.SetBool("crouch", true);
        agent.navMeshAgent.speed = agent.config.crouchIdleSpeed;
      

    }

    public void Update(AiAgent agent)
    {
        if (!agent.targeting.HasTarget)
        {
            Vector3 dir = agent.transform.position - agent.health.hitDir;
            //Debug.Log(dir);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
        agent.sensor.CallOtherAi();
        ActionToPerform(agent);

        
    }

    public void Exit(AiAgent agent)
    {
        agent.weaponIK.SetTargetTransform(null, agent);
        agent.weaponIK.SetAimTransform(null);
        agent.animator.SetBool("crouch", false);
        timer = 20.0f;
     
    }

    void ActionToPerform(AiAgent agent)
    {
        if (agent.targeting.TargetInSight)
        {
            CrouchAttackPlayer(agent);
            //Debug.Log(CheckDist(agent));
            
        }else if (agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.Pursue);
        }
        else
        {
            timer -= Time.deltaTime;
            if(timer < 0.0f)
            {
                agent.stateMachine.ChangeState(AiStateId.Patrol);
            }
        }
    }

    private void CrouchAttackPlayer(AiAgent agent)
    {
        if(!crouched)
        {
            attackTimer -= Time.deltaTime;
            crouchFloat = 0.0f;
            if (attackTimer <= 0.0f)
            {
                crouched = true;
                attackTimer = 0.0f;
               
            }

        }
        else
        {
            attackTimer += Time.deltaTime;
            crouchFloat = Random.Range(0.4f, 1.0f);
            if (attackTimer >= crouchDuration)
            {
                crouched = false;
                attackTimer = crouchDuration;
                

            }
        }

        agent.animator.SetFloat("CrouchToStand", crouchFloat);

        //Debug.Log(crouchFloat);
        if (crouchFloat >= 0.4f && !CheckDist(agent))
        {
            AttackPlayer(agent);
        }
        
        else if (crouchFloat >= 0.4f && CheckDist(agent))
        {
            agent.stateMachine.ChangeState(AiStateId.Attack);
        }
        
        else if(crouchFloat < 0.4f && CheckDist(agent))
        {
            agent.stateMachine.ChangeState(AiStateId.Attack);
        }
        else
        {
            agent.target = agent.targeting.Target.transform;
            Vector3 dir = agent.targeting.TargetPosition - agent.transform.position;
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }
    private void AttackPlayer(AiAgent agent)
    {
  
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
            position.y = targetColliderHeight + agent.transform.position.y;
            position += Random.insideUnitSphere * 0.4f;

            agent.weapon.UpdateWeapon(Time.deltaTime, position);

        }
    
    }
    public void SetFiring(bool enabled, AiAgent agent)
    {
        if (enabled)
        {
            agent.weapon.StartFiring();
        }
        else
        {
            agent.weapon.StopFiring();
        }
    }

    bool CheckDist(AiAgent agent)
    {
        float disPresentPos = Vector3.Distance(agent.transform.position, agent.targeting.TargetPosition);

        if(disPresentPos <= agent.sensor.proximityDist)
            return true;

        return false;
    }
}
