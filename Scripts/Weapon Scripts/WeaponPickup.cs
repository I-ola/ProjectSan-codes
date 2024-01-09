using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    /*public Weapon weaponPrefab;
    public Player player;
    

    private void Start()
    {
        //weaponPrefab = GetComponent<Weapon>();
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerWeaponManager activeWeapon = other.gameObject.GetComponent<PlayerWeaponManager>();
        if (activeWeapon)
        {
            Weapon newWeapon =  Instantiate(weaponPrefab);
            activeWeapon.Equip(newWeapon);
        }
    }

   void PickUpWeapon()
   {
        int weaponSlotIndex = (int)weaponPrefab.weaponSlots;
        PlayerWeaponManager currentWeapon = player.GetComponent<PlayerWeaponManager>();
        Vector3 dist = player.transform.position - transform.position;
        bool equipped = currentWeapon.GetWeapon(weaponSlotIndex);
        if(!equipped && dist.magnitude <= 3f)
        {
            weaponPrefab.GetComponent<Rigidbody>().isKinematic = true;
            currentWeapon.Equip(weaponPrefab);
        }

   } */
}
