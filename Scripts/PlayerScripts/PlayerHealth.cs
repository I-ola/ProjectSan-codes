using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerHealth : Health
{
    public PlayerHealthUi playerHealthUi;
    Animator animator;
    Player player;
    PlayerWeaponManager playerWeaponManager;
    protected override void OnStart()
    {
        animator = GetComponent<Animator>();
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
        player = GetComponent<Player>();

        

        
    }

    protected override void OnDamage(Vector3 direction, float newDamage, Weapon weapon)
    {
        currentHealth -= newDamage;
        if (playerHealthUi != null)
        {
            playerHealthUi.SetHealthBarPlayer(currentHealth / maxHealth);
        }
    }

    protected override void OnHeal(float amount)
    {
        if (playerHealthUi != null)
        {
            playerHealthUi.SetHealthBarPlayer(currentHealth / maxHealth);
        }
    }
    protected override void OnDeath(Vector3 direction)
    {
        
        player.isDead = true;
        GameOver.instance.Playerdied();
        animator.enabled = false;
        // var currentWeapon = activeWeapon.GetActiveWeapon();
        playerWeaponManager.DropWeapon(playerWeaponManager.GetActiveWeaponList());
        playerHealthUi.gameObject.SetActive(false);

    }

}


