using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static PlayerWeaponManager;

public class ReloadWeapon : MonoBehaviour
{
    [SerializeField] private WeaponAnimationEvents AnimationEvents;
    private PlayerWeaponManager playerWeaponManager;
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

       DoOnStart();
    }

    void DoOnStart()
    {
        AnimationEvents.weaponAnimationEvent.AddListener(OnAnimationEvent);
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
        playerController = GetComponent<PlayerController>();
        ai = GetComponent<AiAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerWeaponManager != null && playerWeaponManager.IsThereWeapon)
        {
            // weapon = playerWeapon.GetActiveWeapon();
            weapon = playerWeaponManager.GetActiveWeaponList().GetComponent<Weapon>();
            if (weapon == null)
                return;
            
            if (((playerController.Reload() && weapon.ammoCount < weapon.magazineSize) || (weapon.ammoCount == 0)) && playerWeaponManager.isActive && !playerWeaponManager.isReloading)
            {
              
                if (!alreadyran)
                {
               
                    amountBeforeReload = weapon.ammoCount;
                    Reload(weapon.gameObject);
                }
            
            }
            if (weapon.ammoCount > amountBeforeReload)
            {
                alreadyran = false;
                amountBeforeReload = 0;
             
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
                    Reload(weapon.gameObject);
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
        if (playerWeaponManager != null)
        {
            //weapon = playerWeapon.GetActiveWeapon();
            weapon = playerWeaponManager.GetActiveWeaponList().GetComponent<Weapon>();
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
        if(playerWeaponManager != null)
        {
            //weapon = playerWeapon.GetActiveWeapon();
            weapon = playerWeaponManager.GetActiveWeaponList().GetComponent<Weapon>();
            weapon.magazine.SetActive(true);
            Destroy(magazineOnHand);
            weapon.ammoCount += weapon.reloadAmount;
            weapon.reloadAmount = 0;
            playerWeaponManager.switched = false;
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
    public void Reload(GameObject weaponObj)
    {
        alreadyran = true;
        Weapon weapon = weaponObj.GetComponent<Weapon>();
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

        if(weapon && playerWeaponManager != null && playerWeaponManager.WhichAmmoToUse(weaponObj) > 0)
        {
            StartCoroutine(playerWeaponManager.ReloadWeaponAnim());
            if (weapon.bulletUsed <= playerWeaponManager.WhichAmmoToUse(weaponObj))
            {
                weapon.reloadAmount = weapon.bulletUsed;
            }
            else if (weapon.bulletUsed >= playerWeaponManager.WhichAmmoToUse(weaponObj))
            {
                weapon.reloadAmount = playerWeaponManager.WhichAmmoToUse(weaponObj);
            }
            DeductAmmo(weapon, weapon.reloadAmount);
        }
    }

    public void DeductAmmo(Weapon weapon, int amount)
    {
        int amountLeft;
        switch (weapon.weaponType)
        {
            case WeaponType.Assault:
                amountLeft = playerWeaponManager.assaultWeaponAmmo - amount;
                playerWeaponManager.assaultWeaponAmmo = amountLeft;

                break;

            case WeaponType.Pistol:
                amountLeft = playerWeaponManager.pistolWeaponAmmo - amount;
                playerWeaponManager.pistolWeaponAmmo = amountLeft;

                break;

            case WeaponType.SMG:
                amountLeft = playerWeaponManager.smgWeaponAmmo - amount;
                playerWeaponManager.smgWeaponAmmo = amountLeft;

                break;
        }
    }
    IEnumerator DestroyDroppedMagazine(GameObject droppedMagazine)
    {

        yield return new WaitForSeconds(5.0f);
       
        Destroy(droppedMagazine);
    }
}
