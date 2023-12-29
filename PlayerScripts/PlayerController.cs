using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookValue;
    private InputAction aimAction;
    private InputAction shootAction;
    private InputAction numAction;
    private InputAction reloadAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction interactAction;

    private PlayerWeaponManager aimingAndEquiping;
    private Player player;

    private bool interacting;
    public bool aiming;
    private bool numPress;
    private bool reload;
    private bool sprinting;
    private bool crouch;
    private int boostCameraAmount = 12;
    public CinemachineVirtualCamera aimCamera;

    public bool isShooting;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        aimingAndEquiping = GetComponent<PlayerWeaponManager>();

        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
        lookValue = playerInput.actions["Look"];
        numAction = playerInput.actions["Numbers"];
        reloadAction = playerInput.actions["Reload"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];
        interactAction = playerInput.actions["Interactive"];
    }
    private void Start()
    {
       
        player = GetComponent<Player>();
        

    }

    private void OnEnable()
    {
        Aiming();
        ShootAction();
        NumAction();
        ReloadAction();
        Sprint();
        Crouch();
        InteractWith();
    }
    public Vector2 MovementInput()
    {
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
      
        return inputVector;
    }

    public Vector2 LookInput() 
    {
        Vector2 look = lookValue.ReadValue<Vector2>();
        look = look.normalized;

        return look;
    }

    public bool Pressed()
    {
        return numPress;
    }

    private void NumAction()
    {
        numAction.performed += NumAction_performed;
        numAction.canceled += NumAction_canceled;
    }

    private void NumAction_canceled(InputAction.CallbackContext obj)
    {
        numPress = false;
    }

    private void NumAction_performed(InputAction.CallbackContext obj)
    {
        numPress = true;
    }


    private void Aiming()
    {
        aimAction.performed += AimAction_performed;
        aimAction.canceled += AimAction_canceled;

    }

    private void AimAction_performed(InputAction.CallbackContext obj)
    {
       
        //Weapon weapon = aimingAndEquiping.GetActiveWeapon();
        Weapon weapon = aimingAndEquiping.GetActiveWeaponList();
        
        if (weapon != null && !player.isDead)
        {
            aiming = true;
            AImIn();
        }
    }

    public void AimAction_canceled(InputAction.CallbackContext obj)
    {
        if (aiming)
        {
            aiming = false;

            AimOut();
        }
         

    }

    void AImIn()
    { 
            aimCamera.Priority += boostCameraAmount;
    }

    void AimOut()
    {
        if (!player.isDead)
        {
           
            aimCamera.Priority -= boostCameraAmount;
        }

    }
    public bool isAiming()
    {
        return aiming;
    }

    private void ReloadAction()
    {
        reloadAction.performed += ReloadAction_performed;
        reloadAction.canceled += ReloadAction_canceled;
    }

    private void ReloadAction_canceled(InputAction.CallbackContext obj)
    {
        reload = false;
    }

    private void ReloadAction_performed(InputAction.CallbackContext obj)
    {
        reload = true;
    }

    public bool Reload()
    {
        return reload;
    }

    private void ShootAction()
    {
        shootAction.performed += ShootAction_performed;
        shootAction.canceled += ShootAction_canceled;
    }

    private void ShootAction_canceled(InputAction.CallbackContext obj)
    {
        isShooting = false;
    }

    private void ShootAction_performed(InputAction.CallbackContext obj)
    {
        isShooting = true;

    }

    private void Sprint()
    {
        sprintAction.performed += SprintAction_performed;
        sprintAction.canceled += SprintAction_canceled;
    }

    private void SprintAction_canceled(InputAction.CallbackContext obj)
    {
        sprinting = false;
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        sprinting = true;
    }

    public bool isSprinting()
    {
        return sprinting;
    }

    private void Crouch()
    {
        crouchAction.performed += CrouchAction_performed;
        crouchAction.canceled += CrouchAction_canceled;
    }

    private void CrouchAction_canceled(InputAction.CallbackContext obj)
    {
        crouch = false;
    }

    private void CrouchAction_performed(InputAction.CallbackContext obj)
    {
        crouch = true;
    }

    public bool isCrouching()
    {
        return crouch;
    }

    private void InteractWith()
    {
        interactAction.performed += InteractAction_performed;
        interactAction.canceled += InteractAction_canceled;
    }

    private void InteractAction_canceled(InputAction.CallbackContext obj)
    {
        interacting = false;
    }

    private void InteractAction_performed(InputAction.CallbackContext obj)
    {
        interacting = true;
    }

    public bool isInteracting()
    {
        return interacting;
    }
    /* public void LegacyInput()
     {
       Vector2 inputVector = new Vector2(0, 0);
         if (Input.GetKey(KeyCode.W))
         {
             inputVector.y = +1;

         }


         if (Input.GetKey(KeyCode.A))
         {
             inputVector.x = -1;

         }


         if (Input.GetKey(KeyCode.D))
         {
             inputVector.x = +1;


         }

         if (Input.GetKey(KeyCode.S))
         {
             inputVector.y = -1;

         }

     }*/

}
