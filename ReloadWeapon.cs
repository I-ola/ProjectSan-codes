using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static PlayerWeaponManager;

public class ReloadWeapon : MonoBehaviour
{
    [SerializeField] private WeaponAnimationEvents AnimationEvents;
    private PlayerWeaponManager playerWeapon;
    private PlayerController playerController;
    [HideInInspector] private AiAgent ai;
    [SerializeField] private Transform leftHand;

    int amountBeforeReload;
    bool alreadyran = false;
    GameObject magazineOnHand;
    GameObject droppedMagazine;
    private Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        AnimationEvents.weaponAnimationEvent.AddListener(OnAnimationEvent);
        playerWeapon = GetComponent<PlayerWeaponManager>();
        playerController = GetComponent<PlayerController>();
        ai = GetComponent<AiAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerWeapon != null)
        {
            // weapon = playerWeapon.GetActiveWeapon();
            weapon = playerWeapon.GetActiveWeaponList();
            if (weapon != null)
            {
                bool ammoLow = weapon.ammoCount < weapon.magazineSize;
                bool ammoFinished = weapon.ammoCount == 0;
                if (((playerController.Reload() && ammoLow) || (ammoFinished)) && playerWeapon.isActive() && !playerWeapon.isReloading())
                {
                    if (!alreadyran)
                    {
                        amountBeforeReload = weapon.ammoCount;
                        Reload(weapon);
                    }
                
                }
                if (weapon.ammoCount > amountBeforeReload)
                {
                    alreadyran = false;
                    amountBeforeReload = 0;
                 
                }

            }
        }
        else if (ai != null)
        {
            //Debug.Log("in AI");
            weapon = ai.weapon;
            if (weapon)
            {
                if (weapon.ammoCount <= 0 && !ai.isReloading())
                {
                    Reload(weapon);
                }
            }
        }     

    }

    void OnAnimationEvent(string eventName)
    {
       // Debug.Log(eventName);
       switch(eventName)
        {
          
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }
    }
    
    void DetachMagazine()
    {
        if (playerWeapon != null)
        {
            //weapon = playerWeapon.GetActiveWeapon();
            weapon = playerWeapon.GetActiveWeaponList();
        } else if (ai != null)
        {
            weapon = ai.weapon;
           
        }
        
        magazineOnHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }
    
    void DropMagazine()
    {
        droppedMagazine = Instantiate(magazineOnHand, magazineOnHand.transform.position, magazineOnHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        StartCoroutine(DestroyDroppedMagazine(droppedMagazine));
        magazineOnHand.SetActive(false);
        
    }

    void RefillMagazine()
    {
        magazineOnHand.SetActive(true);
    }

    void AttachMagazine()
    {
        if(playerWeapon != null)
        {
            //weapon = playerWeapon.GetActiveWeapon();
            weapon = playerWeapon.GetActiveWeaponList();
            weapon.magazine.SetActive(true);
            Destroy(magazineOnHand);
            weapon.ammoCount += weapon.reloadAmount;
            weapon.reloadAmount = 0;
            playerWeapon.switched = false;
        }
        else if(ai != null)
        {
            weapon = ai.weapon;
            weapon.magazine.SetActive(true);
            Destroy(magazineOnHand);
            weapon.ammoCount += weapon.reloadAmount;
            weapon.reloadAmount = 0;
        }
    }

    // when called checks for the different parameters  to know the reload amount such as if ammoAvailable is less than the required ammo needed for reload or if there is no longer any availableAmmo to use in reloading the weapon
    //the isReloading bool helps in different ways such as if the player is in the middle of reloading and stops aiming or switches the weapon on aiming or switching back to the weapon the reload starts
    public void Reload(Weapon weapon)
    {
        alreadyran = true;
        
        if (weapon && ai != null)
        {
            if(weapon.availableAmmo > 0 )
            {        
                 if (weapon.bulletUsed <= weapon.availableAmmo)
                 {
                     weapon.reloadAmount = weapon.bulletUsed;
                 }
                 else if(weapon.bulletUsed >= weapon.availableAmmo)
                 {
                     weapon.reloadAmount = weapon.availableAmmo;
                 }

                weapon.availableAmmo -= weapon.reloadAmount;
             
             StartCoroutine(ai.ReloadWeaponAnim());         
            }
            
        }

        if(weapon && playerWeapon != null && playerWeapon.WhichAmmoToUse(weapon) > 0)
        {  
            if (weapon.bulletUsed <= playerWeapon.WhichAmmoToUse(weapon))
            {
                weapon.reloadAmount = weapon.bulletUsed;
            }
            else if (weapon.bulletUsed >= playerWeapon.WhichAmmoToUse(weapon))
            {
                weapon.reloadAmount = playerWeapon.WhichAmmoToUse(weapon);
            }
            StartCoroutine(playerWeapon.ReloadWeaponAnim());
            DeductAmmo(weapon, weapon.reloadAmount);
        }
    }

    public void DeductAmmo(Weapon weapon, int amount)
    {
        int amountLeft;
        switch (weapon.weaponType)
        {
            case WeaponType.Assault:
                amountLeft = playerWeapon.assaultWeaponAmmo - amount;
                playerWeapon.assaultWeaponAmmo = amountLeft;

                break;

            case WeaponType.Pistol:
                amountLeft = playerWeapon.pistolWeaponAmmo - amount;
                playerWeapon.pistolWeaponAmmo = amountLeft;

                break;

            case WeaponType.SMG:
                amountLeft = playerWeapon.smgWeaponAmmo - amount;
                playerWeapon.smgWeaponAmmo = amountLeft;

                break;
        }
    }
    IEnumerator DestroyDroppedMagazine(GameObject droppedMagazine)
    {

        yield return new WaitForSeconds(5.0f);
       
        Destroy(droppedMagazine);
    }
}
