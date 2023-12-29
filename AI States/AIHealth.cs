using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHealth : Health
{
    public bool isDead;
    AiAgent agent;
    // Ragdoll ragdoll;

    protected override void OnStart()
    {
        //ragdoll = GetComponent<Ragdoll>();
        agent = GetComponent<AiAgent>();
       
    }

    protected override void OnDamage(Vector3 direction, float newDamage, Weapon weapon)
    {
        if(weapon.gameObject.GetComponentInParent<Player>() != null)
        {
           // Debug.Log("taking damage from player");
            currentHealth -= newDamage;
            if (healthBar != null)
            {
                healthBar.SetHealthBar(currentHealth / maxHealth);
            }
            bool neededStates = agent.stateMachine.currentState != AiStateId.GoToCover && agent.stateMachine.currentState != AiStateId.CoverAttack;
            if (!agent.targeting.HasTarget && neededStates)
            {
                agent.stateMachine.ChangeState(AiStateId.GoToCover);
            }
        }
       
    }

    protected override void OnDeath(Vector3 direction)
    {
        agent.stateMachine.ChangeState(AiStateId.Dead);
       
        
        // ragdoll.ActivateRagdoll();
        //direction.y = 0;
        //ragdoll.ApplyForce(direction * force);

    }

    protected override void OnHeal(float amount)
    {
        // takingDamage = false;
        if (healthBar != null)
        {
            healthBar.SetHealthBar(currentHealth / maxHealth);
        }
    }



 
}
