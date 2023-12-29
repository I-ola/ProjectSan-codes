using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;


public class AiAgent : MonoBehaviour
{
    public enum WeaponState
    {
        Active,
        Reloading
    }

    public WeaponState weaponState = WeaponState.Active;
    [HideInInspector] public AiStateMachine stateMachine;
    private AiStateId initialState = AiStateId.Idle;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    [HideInInspector] public AiSensor sensor;
    public GameObject[] pathObj;
    [HideInInspector]public Animator animator;
    [HideInInspector] public AiTargetingSystem targeting;
    public Animator rigAnimator;
    [HideInInspector] public AIWeaponIK weaponIK;
    [HideInInspector] public Weapon weapon;
    [HideInInspector] public Transform target = null;
    [HideInInspector] public Transform weaponAim = null;
    [HideInInspector] public AIHealth health;
    [HideInInspector] public AISoundSensor soundManager;
    [HideInInspector] public Key key;
    public LayerMask weaponLayer;
    public GameOver gameOver;
    public MultiParentConstraint handConstrained;
    int weaponAmmo = 10000;

    [HideInInspector] public float animSpeed;
    //[Range(0f, 4f)] public float test;
    public bool isReloading()
    {
        return weaponState == WeaponState.Reloading;
    }

    public bool IsNotInAttackStates
    {
        get
        {
            return stateMachine.currentState != AiStateId.Pursue && stateMachine.currentState != AiStateId.Attack && stateMachine.currentState != AiStateId.CoverAttack;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();      
        navMeshAgent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AiSensor>();
        targeting = GetComponent<AiTargetingSystem>();
        weapon = GetComponentInChildren<Weapon>();
        weaponIK = GetComponent<AIWeaponIK>();
        health = GetComponent<AIHealth>();   
        soundManager = GetComponent<AISoundSensor>();
        key = GetComponentInChildren<Key>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiPursueState());
        stateMachine.RegisterState(new AiDeadState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiPatrolState());
        stateMachine.RegisterState(new AiAttackState());
        stateMachine.RegisterState(new AiSuspectState());
        stateMachine.RegisterState(new AiGoToCoverState());
        stateMachine.RegisterState(new AiCoverAttackState());
        stateMachine.ChangeState(initialState);
        weapon.availableAmmo = weaponAmmo;
        rigAnimator.runtimeAnimatorController = weapon.weaponAnimAI;
    }

    // Update is called once per frame
    void Update()
    {
        
        stateMachine.Update();
        AnimationSet();
        //Debug.Log($"{this.gameObject.name} is in {stateMachine.currentState}");
        //Debug.Log($"{this.gameObject.name} is  in attackState{IsNotInAttackStates}");

        if(weapon.availableAmmo == 0)
        {
            weapon.availableAmmo += 500;
        }
        if (targeting.HasTarget)
        {
            sensor.CallOtherAi();
        }
        if(gameOver.gameOver)
        {
          stateMachine.ChangeState(AiStateId.Idle);
        }
    }

    void AnimationSet()
    {
        animator.SetFloat("speed", animSpeed);//navMeshAgent.velocity.magnitude); //setRange);//navMeshAgent.speed);//
        //Debug.Log(navMeshAgent.velocity.magnitude);
    }
    public void CallOnDestroyAi()
    {
        StartCoroutine(DestroyDeadAi());
    }
    public IEnumerator DestroyDeadAi()
    {
        yield return new WaitForSeconds(8f);

        Destroy(this.gameObject);
    }

    public IEnumerator ReloadWeaponAnim()
    {
        weaponState = WeaponState.Reloading;
        rigAnimator.runtimeAnimatorController = weapon.weaponAnimAI;
        handConstrained.weight -= 1.0f;
        rigAnimator.SetTrigger("reload");
        yield return new WaitForSeconds(0.5f);
        while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        handConstrained.weight = 1.0f;
        rigAnimator.ResetTrigger("reload");
      
        weaponState = WeaponState.Active;
        

    }
}
