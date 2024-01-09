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
        Reloading,
        Activating,
        Holstering
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

    public int assaultWeaponAmmo;
    public int pistolWeaponAmmo;
    public int smgWeaponAmmo;
    public int storedAssaultAmmo;
    public int storedPistolAmmo;
    public int storedSmgAmmo;

    int selectedWeaponIndex = 0;
    int maxNoOfWeapons = 5;
    Weapon[] equippedWeapons = new Weapon[2];
    List<Weapon> equippedWeaponsList = new List<Weapon>();
    int activeWeaponIndex;
    public WeaponState weaponState; //= WeaponState.Holstered;
    public bool switched;
    float hideWeaponTimer = 5f;
    //bool test = true;
    
    public bool isActive
    {
        get{ return weaponState == WeaponState.Active; }
       
    }

    public bool isHolstered
    {
        get
        {
            return weaponState == WeaponState.Holstered;
        }
       
    }

    public bool isReloading
    {
        get
        {
            return weaponState == WeaponState.Reloading;
        }
        
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
    public GameObject GetWeaponFromList(int index)
    {
        if (index < 0 || index >= equippedWeaponsList.Count) return null;
        return equippedWeaponsList[index].gameObject;
    }
    //activeWeaponIndex is the index of the present weapon in use 
   
    public GameObject GetActiveWeaponList()
    {
        return GetWeaponFromList(activeWeaponIndex);
    }
    void addWeapon(GameObject weaponObj)
    {
        equippedWeaponsList.Add(weaponObj.GetComponent<Weapon>());
    }

    public bool isWeaponMax()
    {
        return equippedWeaponsList.Count > maxNoOfWeapons;
    }

    public bool IsThereWeapon
    {
        get { return equippedWeaponsList.Count > 0; }
    }

    public int CheckWeaponPos(GameObject weapon)
    {
        for (int i = 0; i < equippedWeaponsList.Count; i++)
        {
            if (weapon == equippedWeaponsList[i].gameObject)
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
                return equippedWeaponsList[i];
        }
        return null;
    }

    void RemoveWeaponFromList(GameObject _weapon)
    {
        equippedWeaponsList.Remove(_weapon.GetComponent<Weapon>());
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
        assaultWeaponAmmo = 300;
        pistolWeaponAmmo = 200;
        smgWeaponAmmo = 500;
        StartCoroutine(HolsterWeaponAnim(0.3f));
        //primaryWeaponAmmo = 300;
        Weapon existingWeapon = GetComponentInChildren<Weapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!player.isDead)
            WeaponAnimations();

        if (!player.isDead && IsThereWeapon)
        {
            SwitchWeapon();
            FireWeapon();
           
            SetWeaponActive();
            HideWeapon();
            //Debug.Log(isWeaponMax());
        }
        //Debug.Log("WEAPON STATE FROM WEAPONMANAGER    " + weaponState);
           
           
    }
    //this control the aiming and hiding animations based on the right mouse button click which serves as the aiming 
    void WeaponAnimations()
    {
        rigAnimator.SetBool("isAiming", (controller.isAiming() && !controller.isCrouching()));

        rigAnimator.SetBool("crouchAim", (controller.isCrouching() && controller.isAiming()));

        if (!controller.isAiming() && !isHolstered)
        {
           StartCoroutine(HolsterWeaponAnim(0.3f));
        }
        else if (controller.isAiming() && (!isActive)) 
        {
            StartCoroutine(EquipWeaponAnim(0.3f));
        }

        if (isReloading)
        {
            StopCoroutine(EquipWeaponAnim(0f));
        }
    }
    //this sets the invisible weapon to visible when the aim action "right mouse button" is performed
    void SetWeaponActive()
    {
        if (isActive)
        {
            GameObject weapon = GetActiveWeaponList();
            if (weapon != null)
            {
                weapon.SetActive(true);
            }

        }
    }
    IEnumerator HolsterWeaponAnim(float time)
    {
        weaponState = WeaponState.Holstering;
        rigAnimator.SetBool("hideWeapon", true);
        yield return new WaitForSeconds(time);
        while(rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
      
        weaponState = WeaponState.Holstered;


    }

    IEnumerator EquipWeaponAnim(float time)
    {
        weaponState = WeaponState.Activating;
        var currentWeapon = GetActiveWeaponList().GetComponent<Weapon>();
        rigAnimator.runtimeAnimatorController = currentWeapon.weaponAnim;
        rigAnimator.SetBool("hideWeapon", false);
        if (!currentWeapon.gameObject.activeSelf)
            currentWeapon.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
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
        while(rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        rigAnimator.SetBool("reload", false);
        weaponState = WeaponState.Active;


    }

    //this is called when the shoot action is performed
    private void FireWeapon()
    {
        
        var weapon = GetActiveWeaponList().GetComponent<Weapon>();
        if(weapon != null)
        {
            if (isActive && Shooting())
            {
                weapon.UpdateWeapon(Time.deltaTime, crossHairTarget.position);
                //DeductAmmo(weapon);

                weapon.StartFiring();
                //Debug.Log("I am running");
            }

            if ((!controller.isAiming() || isReloading) && Shooting())
            {
                weapon.StopFiring();
            }
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
    public void Equip(GameObject newWeapon)
    {
        Weapon weapon = newWeapon.GetComponent<Weapon>();
        StopCoroutine(DestroyDropWeapon(newWeapon, 0.0f));
        int weaponSlotIndex = (int)weapon.weaponSlots;
       // Weapon weapon =  CheckWeapon(newWeapon);
       
        weapon.isDropped = false;
        newWeapon.gameObject.transform.parent = weaponSlots[weaponSlotIndex];
        newWeapon.GetComponent<WeaponRecoil>().enabled = true;
        weapon.recoil.playerAimCamera = player;
        weapon.recoil.rigController = rigAnimator;
        weapon.gameObject.transform.localPosition = Vector3.zero;
        if(weapon.isSMG)
            weapon.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        else
            weapon.gameObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        
        CheckAmmo(newWeapon);
        addWeapon(newWeapon);

        activeWeaponIndex = CheckWeaponPos(newWeapon);
        newWeapon.SetActive(true);
        StartCoroutine(EquipWeaponAnim(0.3f));
        weaponUiManager.AddingWeapon(newWeapon);

        
    }


    // this checks the weapon type and adds available ammo from the weapon to the players own personal ammo
    public void CheckAmmo(GameObject weaponObj)
    {
        Weapon weapon = weaponObj.GetComponent<Weapon>();

        if (weapon == null)
            return;

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

    public int WhichAmmoToUse(GameObject weaponObj)
    {
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        if (weapon == null)
            return 0;

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
        if (GetWeaponFromList(WeaponSwitchingInt()) == null)
            return;

        if(activeWeaponIndex != WeaponSwitchingInt())
        {
            StartCoroutine(SwitchWeaponAnim(WeaponSwitchingInt()));
            //Debug.Log("it was switched");
        }

        if(!GetWeaponFromList(WeaponSwitchingInt()).activeSelf)
            GetWeaponFromList(WeaponSwitchingInt()).SetActive(true);
    }

    IEnumerator SwitchWeaponAnim(int index)
    {
        yield return StartCoroutine(HolsterWeaponAnim(0.3f));
        activeWeaponIndex = index;
        yield return StartCoroutine(EquipWeaponAnim(0.3f));
    }
    // to automatically hide the players weapon after seconds of it not being in use
    void HideWeapon()
    {
        for (int i = 0; i < equippedWeaponsList.Count ; i++)
        {
           
            if (equippedWeaponsList[i] != null && isHolstered && equippedWeaponsList[i].gameObject.activeSelf)
            {
                hideWeaponTimer -= Time.deltaTime;
                if (hideWeaponTimer < 0)
                {
                    equippedWeaponsList[i].gameObject.SetActive(false);
                    hideWeaponTimer = 5;
                }
            } 
        }
     ;
        
    }

    //a function that makes player drop the current active weapon
    public void DropWeapon(GameObject currentWeaponObj)
    {

        // var currentWeapon = GetActiveWeapon();
        if (!IsThereWeapon)
            return;

        Weapon currentWeapon = currentWeaponObj.GetComponent<Weapon>();
        if (currentWeapon == null)
            return;
      
        weaponUiManager.RemoveWeapon(currentWeapon.gameObject);
        currentWeapon.availableAmmo = 0;
        currentWeapon.isDropped = true;
        currentWeapon.transform.SetParent(null);
        currentWeaponObj.SetActive(true);
        currentWeaponObj.GetComponent<BoxCollider>().enabled = true;
        if(currentWeaponObj.GetComponent<Rigidbody>() == false)
        {
            currentWeaponObj.AddComponent<Rigidbody>();
        }
        currentWeaponObj.GetComponent<Rigidbody>().isKinematic = false;
        //equippedWeapons[activeWeaponIndex] = null;
        RemoveWeaponFromList(currentWeaponObj);
        StartCoroutine(DestroyDropWeapon(currentWeaponObj, 6.0f));
       
    }

    IEnumerator DestroyDropWeapon(GameObject weaponObj, float timer)
    {
        //Debug.Log("called on destroy");
        yield return new WaitForSeconds(timer);
        //Destroy(weapon.gameObject);
        weaponObj.SetActive(false);
      
        
    }
}
