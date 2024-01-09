using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Animator movementAnimator;
    bool animatorDisabled;
   [SerializeField] private Animator weaponAnimator;
    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        movementAnimator = GetComponent<Animator>();
        DeActivateRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeActivateRagdoll()
    {
        foreach(var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        movementAnimator.enabled = true;
        weaponAnimator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        movementAnimator.enabled = false;
        weaponAnimator.enabled = false;
   
        
       

    }

    public void ApplyForce(Vector3 force)
    {
        var rigidBody = movementAnimator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        rigidBody.AddForce(force, ForceMode.VelocityChange);
    }

    

   
}
