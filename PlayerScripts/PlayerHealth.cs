using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerHealth : Health
{
  
    Player player;
    PlayerWeaponManager activeWeapon;
    protected override void OnStart()
    {
        activeWeapon = GetComponent<PlayerWeaponManager>();
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
        // var currentWeapon = activeWeapon.GetActiveWeapon();
        Weapon currentWeapon = activeWeapon.GetActiveWeaponList();
        activeWeapon.DropWeapon(currentWeapon);
        playerHealthUi.gameObject.SetActive(false);

    }

}


