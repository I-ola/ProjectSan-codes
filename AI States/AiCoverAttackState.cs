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
    float checkBulletPathTimer = 2f;
    float crouchDuration = 6f;
    bool crouched = true;
    Vector3 lastKnownLocation;
    bool set = false;


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
        if (checkBulletPathTimer > 0f)
        {
            Vector3 dir = agent.transform.position - agent.health.hitDir;
            //Debug.Log(dir);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
            checkBulletPathTimer -= Time.deltaTime;
        }
        //Debug.Log(checkBulletPathTimer);
        agent.sensor.CallOtherAi();
        ActionToPerform(agent);
        //Debug.Log(timer);
        
    }

    public void Exit(AiAgent agent)
    {
        agent.weaponIK.SetTargetTransform(null, agent);
        agent.weaponIK.SetAimTransform(null);
        agent.animator.SetBool("crouch", false);
        checkBulletPathTimer = 2f;
        timer = 20.0f;
        set = false;
    }

    void ActionToPerform(AiAgent agent)
    {
        if (agent.targeting.TargetInSight)
        {
           
            if (!set)
            {
                lastKnownLocation = agent.targeting.TargetPosition;
                set = true;
            }
            CrouchAttackPlayer(agent);
            Debug.Log(CompareDist(agent));
            
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
        bool locationChanged = (lastKnownLocation != agent.targeting.TargetPosition);
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
        if (crouchFloat > 0.4f)
        {
            AttackPlayer(agent);
        }else if(crouchFloat < 0.4f && CompareDist(agent) && locationChanged)
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
        }
        else
        {
            agent.weapon.StopFiring();
        }
    }

    bool CompareDist(AiAgent agent)
    {
        float disFromLastKnownPos = Vector3.Distance(agent.transform.position, lastKnownLocation);
        float disPresentPos = Vector3.Distance(agent.transform.position, agent.targeting.TargetPosition);

        if (disFromLastKnownPos > disPresentPos)
            return false;
        if(disFromLastKnownPos <= agent.sensor.proximityDist)
            return true;
        return false;
    }
}
