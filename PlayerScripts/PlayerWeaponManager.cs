using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;




public class PlayerWeaponManager : MonoBehaviour
{
    public enum WeaponState
    {
        Active,
        Holstered,
        Reloading
    }
    private Player player;
    private PlayerController controller;
    [SerializeField] private Transform crossHairTarget;
    [SerializeField] private Transform[] weaponSlots;
    [SerializeField] private Transform leftGrip;
    [SerializeField] private Transform rightGrip;
    [SerializeField] public WeaponUiManager weaponUiManager;
    [SerializeField] private Rig aimLayer;
    [SerializeField] private Animator rigAnimator;

    public int assaultWeaponAmmo = 300;
    public int pistolWeaponAmmo = 200;
    public int smgWeaponAmmo = 400;
    public int storedAssaultAmmo;
    public int storedPistolAmmo;
    public int storedSmgAmmo;

    int selectedWeaponIndex = 0;
    int maxNoOfWeapons = 5;
    Weapon[] equippedWeapons = new Weapon[2];
    List<Weapon> equippedWeaponsList = new List<Weapon>();
    int activeWeaponIndex; 
    public WeaponState weaponState = WeaponState.Holstered;
    public bool switched;
    float hideWeaponTimer = 5f;
    //bool test = true;
    
    public bool isActive()
    {
        return weaponState == WeaponState.Active;
    }

    public bool isHolstered()
    {
        return weaponState == WeaponState.Holstered;
    }

    public bool isReloading()
    {
        return weaponState == WeaponState.Reloading;
    }
   

    /* this is passed to the weapon script and assigned in the weapon prefab 
    useful when we are trying to get weapon as we have already assigned each weapon an int based on
    if they are primary or secondary so all primary weapon are in slot 0 and secondary in slot 1*/
    public enum WeaponSlots
    {
        Assault = 0,
        Pistol = 1,
        SMG = 2

    }

    public enum WeaponType
    {
        Assault,
        Pistol,
        SMG
    }

    /*this is used to get the weapons equipped at the different index of the array of equipped
    wapons which has been made 2 and in the equip function we store the equip weapon in either 0 or 1 based on their assigned values in the weaponslot enum
    so this returns if there is a weapon stored at the provided index*/
    public Weapon GetWeapon(int index)
    {
        if(index < 0 || index >= equippedWeapons.Length) return null;
        return equippedWeapons[index];
    }
    public Weapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }


    //The below are variations of above code since i switched weapon stroing from array to list
    public Weapon GetWeaponFromList(int index)
    {
        if (index < 0 || index >= equippedWeaponsList.Count) return null;
        return equippedWeaponsList[index];
    }
    //activeWeaponIndex is the index of the present weapon in use 
   
    public Weapon GetActiveWeaponList()
    {
        return GetWeaponFromList(activeWeaponIndex);
    }
    void addWeapon(Weapon weapon)
    {
        equippedWeaponsList.Add(weapon);
    }

    public bool isWeaponMax()
    {
        return equippedWeaponsList.Count > maxNoOfWeapons;
    }
    public int CheckWeaponPos(Weapon weapon)
    {
        for (int i = 0; i < equippedWeaponsList.Count; i++)
        {
            if (weapon == equippedWeaponsList[i])
                return i;
        }
        return -1;
    }

    public bool DoesWeaponTypeExist(Weapon weapon)
    {
        for (int i = 0; i < equippedWeaponsList.Count; i++)
        {
            if (weapon.weaponType == equippedWeaponsList[i].weaponType)
                return true;
        }
        return false;
    }
 
    public Weapon CheckWeapon(Weapon _weapon)
    {
        for (int i = 0; i < equippedWeaponsList.Count; i++)
        {
            if (_weapon.weaponType == equippedWeaponsList[i].weaponType)
                return _weapon;
        }
        return null;
    }

    void RemoveWeaponFromList(Weapon _weapon)
    {
        equippedWeaponsList.Remove(_weapon);
    }

    int WeaponSwitchingInt()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeaponIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeaponIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeaponIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeaponIndex = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedWeaponIndex = 4;
        }

        return selectedWeaponIndex;
    }

    void Start()
    {
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
        //primaryWeaponAmmo = 300;
        Weapon existingWeapon = GetComponentInChildren<Weapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }
    // Update is called once per frame
    void Update()
    {  
        if (!player.isDead)
        {
            SwitchWeapon();
            FireWeapon();
            WeaponAnimations();
            SetWeaponActive();
            //Debug.Log(isWeaponMax());
        }       
    }
    //this control the aiming and hiding animations based on the right mouse button click which serves as the aiming 
    void WeaponAnimations()
    {
        rigAnimator.SetBool("isAiming", (controller.isAiming() && !controller.isCrouching()));

        rigAnimator.SetBool("crouchAim", (controller.isCrouching() && controller.isAiming()));
        

        if (!controller.isAiming())
        {
           StartCoroutine(HolsterWeaponAnim(0.7f));
        }
        else
        {
          
            StartCoroutine(EquipWeaponAnim(0.3f));
        }
    }
    //this sets the invisible weapon to visible when the aim action "right mouse button" is performed
    void SetWeaponActive()
    {
        if (isActive())
        {
            Weapon weapon = GetActiveWeaponList();
            if (weapon != null)
            {
                weapon.gameObject.SetActive(true);
            }

        }
    }
    IEnumerator HolsterWeaponAnim(float time)
    {
        var weapon = GetActiveWeaponList();
        if (weapon != null)
            hideWeapon(weapon);
        rigAnimator.SetBool("hideWeapon", true);
        yield return new WaitForSeconds(time);
        while(rigAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }
        weaponState = WeaponState.Holstered;
   
            
        
    }

    IEnumerator EquipWeaponAnim(float time)
    {
        //Weapon currentWeapon = GetActiveWeapon();
        Weapon currentWeapon = GetActiveWeaponList();
        rigAnimator.runtimeAnimatorController = currentWeapon.weaponAnim;
        rigAnimator.SetBool("hideWeapon", false);
        if (!currentWeapon.gameObject.activeSelf)
            currentWeapon.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        while (rigAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }
        weaponState = WeaponState.Active;
    }
    public IEnumerator ReloadWeaponAnim()
    {
        weaponState = WeaponState.Reloading;
        rigAnimator.SetBool("reload", true);
        yield return new WaitForSeconds(0.5f);
        while(rigAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }
        weaponState = WeaponState.Active;
        rigAnimator.SetBool("reload", false);

    }

    //this is called when the shoot action is performed
    private void FireWeapon()
    {
        //var weapon = GetWeapon(activeWeaponIndex);
        var weapon = GetActiveWeaponList();
        if ((weapon != null) && isActive() && Shooting())
        {
            weapon.UpdateWeapon(Time.deltaTime, crossHairTarget.position);
            //DeductAmmo(weapon);
            weapon.StartFiring();        
        }
        else if (weapon && (!Shooting() || isHolstered()))
        {
            weapon.StopFiring();
        }
       
    }
   
    private void SwitchWeapon()
    {
        if (controller.Pressed())
        {
            SelectWeapon();

        }
        
    }
     
    public bool Shooting()
    {
        return controller.isShooting;
    }

    // this function is used for equipping wepaon  it attaches the weapon based on the slot index this is already assigned on the weapon prefab and also sets the weapon parent to the player
    public void Equip(Weapon newWeapon)
    {
        StopCoroutine(DestroyDropWeapon(newWeapon, 0.0f));
        int weaponSlotIndex = (int)newWeapon.weaponSlots;
        Weapon weapon =  CheckWeapon(newWeapon);
        weapon = newWeapon;
        weapon.isDropped = false;
        weapon.transform.parent = weaponSlots[weaponSlotIndex];
        weapon.GetComponent<WeaponRecoil>().enabled = true;
        weapon.recoil.playerAimCamera = player;
        weapon.recoil.rigController = rigAnimator;
        weapon.gameObject.transform.localPosition = Vector3.zero;
        if(weapon.isSMG)
            weapon.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        else
            weapon.gameObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        
        CheckAmmo(weapon);
        addWeapon(weapon);
        // equippedWeapons[weaponSlotIndex] = weapon;

        //activeWeaponIndex = (int)weapon.weaponSlots;

        activeWeaponIndex = CheckWeaponPos(weapon);
        weapon.gameObject.SetActive(true);
        StartCoroutine(EquipWeaponAnim(0.3f));
        weaponUiManager.AddingWeapon(weapon);

        
    }


    // this checks the weapon type and adds available ammo from the weapon to the players own personal ammo
    public void CheckAmmo(Weapon weapon)
    {
        switch (weapon.weaponType)
        {
            case WeaponType.Assault:
                if(weapon.availableAmmo >= 0)
                {
                    storedAssaultAmmo = weapon.availableAmmo;
                    assaultWeaponAmmo += storedAssaultAmmo;
                    weapon.availableAmmo = 0;
                    storedAssaultAmmo = 0;
                }           
                break;

            case WeaponType.Pistol:
                if(weapon.availableAmmo > 0)
                {
                    storedPistolAmmo = weapon.availableAmmo;
                    pistolWeaponAmmo += storedPistolAmmo;
                    weapon.availableAmmo = 0;
                    storedPistolAmmo = 0;
                }
                break;

            case WeaponType.SMG:
                if(weapon.availableAmmo > 0)
                {
                    storedSmgAmmo = weapon.availableAmmo;
                    smgWeaponAmmo += storedSmgAmmo;
                    weapon.availableAmmo = 0;
                    storedSmgAmmo = 0;
                }
                break;
        }

    }

    public int WhichAmmoToUse(Weapon weapon)
    {
        if(weapon.weaponType == WeaponType.Assault)
              return assaultWeaponAmmo;
                 
        if(weapon.weaponType == WeaponType.Pistol)
                 return pistolWeaponAmmo;
        if (weapon.weaponType == WeaponType.SMG)
            return smgWeaponAmmo;
        return 0;

    }
    // this deals in switching weapon based on pressing either the 1 or 2 key and also deals with the animation for both weapons
    public void SelectWeapon()
    {

        //var weapon = GetWeapon(WeaponSwitchingInt());
        var weapon = GetWeaponFromList(WeaponSwitchingInt());
        if (weapon != null)
        {
          
            
            if(activeWeaponIndex != WeaponSwitchingInt())
            {
                StartCoroutine(SwitchWeaponAnim(WeaponSwitchingInt()));
                //Debug.Log("it was switched");
            }

            activeWeaponIndex = WeaponSwitchingInt();
            if (!weapon.gameObject.activeSelf)
                weapon.gameObject.SetActive(true);
           
        }

        if (weapon == null)
        {
            return;
        }

      

    }

    IEnumerator SwitchWeaponAnim(int index)
    {
        yield return StartCoroutine(HolsterWeaponAnim(0.1f));
        activeWeaponIndex = index;
        yield return StartCoroutine(EquipWeaponAnim(0.1f));
    }
    // to automatically hide the players weapon after seconds of it not being in use
    void hideWeapon(Weapon _weapon)
    {       
        hideWeaponTimer -= Time.deltaTime;
        if(_weapon != null && hideWeaponTimer < 0)
        {
            _weapon.gameObject.SetActive(false);
            hideWeaponTimer = 5;
        }
    }

    //a function that makes player drop the current active weapon
    public void DropWeapon(Weapon currentWeapon)
    {
       
       // var currentWeapon = GetActiveWeapon();
        if (currentWeapon)
        {
            weaponUiManager.RemoveWeapon(currentWeapon);
            currentWeapon.availableAmmo = 0;
            currentWeapon.isDropped = true;
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            if(currentWeapon.gameObject.GetComponent<Rigidbody>() == false)
            {
                currentWeapon.gameObject.AddComponent<Rigidbody>();
            }
            currentWeapon.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            //equippedWeapons[activeWeaponIndex] = null;
            RemoveWeaponFromList(currentWeapon);
            StartCoroutine(DestroyDropWeapon(currentWeapon, 6.0f));
        }
    }

    IEnumerator DestroyDropWeapon(Weapon weapon, float timer)
    {
        //Debug.Log("called on destroy");
        yield return new WaitForSeconds(timer);
        Destroy(weapon.gameObject);
      
        
    }
}
