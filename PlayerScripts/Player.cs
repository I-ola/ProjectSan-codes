using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using static PlayerWeaponManager;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private PlayerController control;
    private ReloadWeapon reload;
    private SoundManager.SoundsCategory soundCategory;
    

    [SerializeField] private GameObject cameraTarget;
    private PlayerWeaponManager weaponManager;
   
    public Image pickupWeaponImage;
 
    private float horizontal;
    private float vertical;
    Vector3 movDir;

    Weapon collidedWeapon;

    private Key key;
    private Door door;
    public Transform doorChecker;
    
    
    private float startYScale;
    private float startCenter;

    [HideInInspector] public float cameraTargetYaw;
    [HideInInspector] public float cameraTargetPitch;
    private float sensitivity;
    [HideInInspector] public bool isDead = false;
   
    private float speed;
    private float walkSpeed = 5f;
    private float runSpeed = 7f;
    private float crouchSpeed = 2f;
    float intensity = 0;
    bool canReset = true;
    float timeBeforeReset = 5f;
    float reductionAmount;

    [HideInInspector] public bool hitPortal = false;
    CharacterController characterController;
    [HideInInspector] public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching
    }
   
    void Start()
    {
        //rb = this.GetComponent<Rigidbody>();
        control = this.GetComponent<PlayerController>();
        weaponManager = GetComponent<PlayerWeaponManager>();
        reload = GetComponent<ReloadWeapon>();
        characterController = GetComponent<CharacterController>();
  
        cameraTargetYaw = cameraTarget.transform.rotation.eulerAngles.y;

        startYScale = characterController.height;
        startCenter = characterController.center.y;
        
    }

    private void Update()
    {
       if(!isDead)
       {
            MovePlayer();
            CrouchManager();
            StateHandler();
            PickAndDropWeapon();
            OpenDoor();
            SoundBasedOnMovement();
           
       }

    }

    private void LateUpdate()
    {
        if(!isDead)
            CameraMovement();
    }

    public float Horizontal()
    {
        return horizontal;
    }
    public float Vertical()
    {
        return vertical;
    }

    bool isRunning()
    {
        return control.isSprinting() && !control.isAiming() && vertical == 1 && !control.isCrouching();
    }
    public bool canMoveHorizontal()
    {
        return horizontal == 0;
    }

    public bool canMoveVertical()
    {
        return vertical == 0;
    }
    bool isWalking()
    {
        return (!canMoveHorizontal() || !canMoveVertical()) && !isRunning();
    }

    bool isCrouchWalking()
    {
        return isWalking() && control.isCrouching();
    }
    private void CrouchManager()
    {
        float newHeight = startYScale / 2f;
        float anotherHeight = startYScale / 1.25f;
        //start crouch
        if (control.isCrouching() && !isWalking())
        {

            characterController.height = newHeight;
            characterController.center = new Vector3(characterController.center.x, startCenter / 2f, characterController.center.z);
        }
        else if (isCrouchWalking())
        {

            characterController.height = anotherHeight;
            characterController.center = new Vector3(characterController.center.x, startCenter / 1.25f, characterController.center.z);

        }
        else
        {

            characterController.height = startYScale;
            characterController.center = new Vector3(characterController.center.x, startCenter, characterController.center.z);
        }
    }
    
    //this handles the speed of player movement based on the key pressed and player state
    private void StateHandler()
    {
        if(isRunning())
        {
            state = MovementState.sprinting;
            speed = runSpeed;
            vertical += 1f;
        }
        else
        {
            state = MovementState.walking;
            speed = walkSpeed;
           
        }

        if (control.isCrouching())
        {
            state = MovementState.crouching;
            speed = crouchSpeed;
        }
    }

    //this handles player movement and rotates and move the player based on the camera position
    private void MovePlayer()
    {
        Vector2 inputVector = control.MovementInput();

        movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, cameraForward);

        Vector3 move = cameraForward * movDir.z + right * movDir.x;

        //transform.position += move * speed * Time.deltaTime;
        characterController.SimpleMove(move * speed);

        Quaternion targetRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f);

        

        horizontal = movDir.x;
        vertical = movDir.z;
     

    }
   
    //This controls the movement of the cinemachine camera based on the mouse action 
    private void CameraMovement()
    {
        Vector2 look = control.LookInput();
        cameraTargetYaw += look.x * 1.0f * sensitivity;
        cameraTargetPitch += look.y * -1.0f *sensitivity;

        cameraTargetYaw = Clamp(cameraTargetYaw, float.MinValue, float.MaxValue);
        cameraTargetPitch = Clamp(cameraTargetPitch, -30.0f, 70.0f);

        cameraTarget.transform.rotation = Quaternion.Euler(cameraTargetPitch, cameraTargetYaw, 0.0f);

    }

    // this is used to clamp the camera movement between 360 degrees so the camera won't move out of range
    private float Clamp(float lf, float min, float max)
    {
        if (lf > 360)
            lf -= 360;
        if (lf < -360)
            lf += 360;
        
        return Mathf.Clamp(lf, min, max);
    }
    public void SetSensitivity(float newValue)
    {
        sensitivity = newValue;
    }
     
    void CollidedWeaponIconEnabled()
    {
        Weapon weapon = collidedWeapon;
        if (weapon)
        {
            pickupWeaponImage.sprite = weapon.weaponSprite;
            pickupWeaponImage.gameObject.SetActive(true);

        }
    }

    void CollidedWeaponDisabled()
    {
        collidedWeapon = null;
        pickupWeaponImage.sprite = null;
        pickupWeaponImage.gameObject.SetActive(false);
    }

    IEnumerator DisableWeapon(float time)
    {
        yield return new WaitForSeconds(time);
        CollidedWeaponDisabled();
         
    }
    //This handles Weapon pickup and dropping 
    private void PickAndDropWeapon()
    {
        Vector3 dist;          
        if (collidedWeapon != null && control.isInteracting())
        {
            dist = this.transform.position - collidedWeapon.transform.position;
           
            if (dist.magnitude <= 5f)
            {
                
                PickWeapon(collidedWeapon);
                // Debug.Log("picked");
                collidedWeapon = null;
            }
            
        }
        
        else if(control.isInteracting() && collidedWeapon == null && weaponManager.isActive())
        {
            DropCurrentWeapon();
            control.aiming = false;
            control.aimCamera.Priority = 9;
            //Debug.Log("droppedWeapon");
        }
    }


    /*this helps to assign and store used ammo the weapon available ammo is assigned to the value of the ammo on player which can either be secondary or primary
    the 2 different ammo for the different weapons primary and secondary and are assigne to the weapons based on the type assigned in the weapon prefab
    when the weapon is fired we then assign the new amount of the weapon available ammo in an empty var
    and then equate the players own ammo to those values so when player picks or passes over a weapono of the same type it will just add its own personal values to the current player ammo*/
    private void CalculateAmmo(Weapon weapon)
    {
        if (weaponManager.DoesWeaponTypeExist(weapon) && weapon.availableAmmo > 0)
        {
            Weapon oldWeapon = weaponManager.CheckWeapon(weapon);
            switch (weapon.weaponType)
            {
                case WeaponType.Assault:
                    if (oldWeapon != null)
                    {
                        weaponManager.storedAssaultAmmo = weapon.availableAmmo;
                        weaponManager.assaultWeaponAmmo += weaponManager.storedAssaultAmmo;
                        weapon.availableAmmo = 0;
                        weaponManager.storedAssaultAmmo = 0;
                     
                    }
                    break;

                case WeaponType.Pistol:
                    if (oldWeapon != null)
                    {
                        weaponManager.storedPistolAmmo = weapon.availableAmmo;
                        weaponManager.pistolWeaponAmmo += weaponManager.storedPistolAmmo;
                        weapon.availableAmmo = 0;
                        weaponManager.storedPistolAmmo = 0;
                       
                    }
                    break;

                case WeaponType.SMG:
                    if(oldWeapon != null)
                    {
                        weaponManager.storedSmgAmmo = weapon.availableAmmo;
                        weaponManager.pistolWeaponAmmo += weaponManager.storedSmgAmmo;
                        weapon.availableAmmo = 0;
                        weaponManager.storedSmgAmmo = 0;
                    }
                    break;
            }
        }
   
    }
    //this is the main function for picking up weapon using the Equip and Drop function in the AimingAndEquiping Script 
    private void PickWeapon(Weapon newWeapon)
    {
       
        if (newWeapon != null && !weaponManager.isWeaponMax())
        {
            newWeapon.GetComponent<Rigidbody>().isKinematic = true;
            newWeapon.GetComponent<BoxCollider>().enabled = false;
            weaponManager.Equip(newWeapon);
            CollidedWeaponDisabled();
            return;
      
        }
        else if(newWeapon!= null && weaponManager.isWeaponMax())
        {
            DropCurrentWeapon();
            newWeapon.GetComponent<Rigidbody>().isKinematic = true;
            newWeapon.GetComponent<BoxCollider>().enabled = false;
            weaponManager.Equip(newWeapon);
            CollidedWeaponDisabled();
            return;
        }


        
    }


    //the main function for dropping weapon this uses the DropWeapon function already defined in the AimingAndEquiping script
    private void DropCurrentWeapon()
    {
        //Weapon weapon = weaponManager.GetActiveWeapon();
        Weapon weapon = weaponManager.GetActiveWeaponList();
        if(weapon != null)
        {
            weaponManager.DropWeapon(weapon);
        //Debug.Log("dropped a" + weapon.weaponName);
        }
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Key collidedKey = hit.gameObject.GetComponent<Key>();
        if(collidedKey != null)
        {
            collidedKey.gameObject.SetActive(false);
            collidedKey.transform.SetParent(this.transform);
            key = collidedKey;
        }

         Weapon theCollidedWeapon = hit.gameObject.GetComponent<Weapon>();
        if (theCollidedWeapon!= null)
        {
            collidedWeapon = theCollidedWeapon.GetComponent<Weapon>();
            CalculateAmmo(collidedWeapon);
            CollidedWeaponIconEnabled();
            StartCoroutine(DisableWeapon(6));
            //Debug.Log("collided with weapon");
        }

        if (hit.gameObject.CompareTag("Portal"))
        {
            hitPortal = true;
        }
    }

    void GetDoor()
    {
        RaycastHit hit;
        Physics.Raycast(doorChecker.transform.position, doorChecker.transform.forward, out hit, 10.0f);
        if(hit.collider != null && hit.collider.GetComponent<Door>() != null)
        {
            door = hit.collider.GetComponent<Door>();
            //Debug.Log(Vector3.Dot(door.transform.forward, transform.forward));
        }
    }
   
    void OpenDoor()
    {
        GetDoor();
           
        if (control.isInteracting() && door != null && key != null)
        {
           
           bool pos = Vector3.Dot(door.transform.forward , transform.forward) <= 0;
           //Debug.Log(pos);
           door.CheckKey(key, door, pos);
        
        }
    }

    void SoundBasedOnMovement()
    {

        //Weapon weapon = weaponManager.GetActiveWeapon();
        Weapon weapon = weaponManager.GetActiveWeaponList();
        if (isWalking() && !control.isCrouching() && !isRunning())
        {
            soundCategory = SoundManager.SoundsCategory.Walking;
            intensity += 0.05f;
            SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
            canReset = false;
            reductionAmount = 0.5f;
        }
        if (control.isSprinting())
        {
            soundCategory = SoundManager.SoundsCategory.Running;
            intensity += 0.5f;
            SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
            canReset = false;
            reductionAmount = 0.2f;
        }
        if (isCrouchWalking())
        {
            soundCategory = SoundManager.SoundsCategory.CrouchWalking;
            intensity += 0.02f;
            SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
            canReset = false;
            reductionAmount = 1f;
        }
        if (control.isCrouching() && !isWalking()) 
        {
            canReset = true;
            reductionAmount = 3f;
        }
        if(weapon != null)
        {
            if (control.isShooting && weapon.weaponType == PlayerWeaponManager.WeaponType.Assault)
            {
                soundCategory = SoundManager.SoundsCategory.Shooting;
                intensity += 0.5f;
                SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
                canReset = false;
                reductionAmount = 1f;
            }

            if (control.isShooting && weapon.weaponType == PlayerWeaponManager.WeaponType.Pistol)
            {
                soundCategory = SoundManager.SoundsCategory.Shooting;
                intensity += 0.1f;
                SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
                canReset = false;
                reductionAmount = 3f;
            }
        }
        if(timeBeforeReset > 0 && !canReset)
        {
            timeBeforeReset-=Time.deltaTime;
        }
        if (timeBeforeReset <= 0)
        {
            canReset = true;
            timeBeforeReset = 5f;
        }
        if (intensity > 0 && canReset)
        {
            intensity -= reductionAmount;
            Mathf.Clamp(intensity, 0f, float.MaxValue);
            //Debug.Log(intensity);
        }
       
    }

    
}
 