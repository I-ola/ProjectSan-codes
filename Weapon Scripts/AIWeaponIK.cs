using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    public float weight;
}
public class AIWeaponIK : MonoBehaviour
{
     Vector3 targetTransform;
     Transform aimTransform;



    public int iterations = 10;
    [Range(0, 1)]
    public float weight = 1.0f;

    public HumanBone[] humanBones;
    Transform[] boneTransforms;

     float angleLimit = 90f;
     float distanceLimit = 0.9f;
    public Vector3 aimOffset;
    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for(int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
    }

   
    // Update is called once per frame
    void LateUpdate()
    {
        
        if(aimTransform == null)
        {
            return;
        }

        if(targetTransform == null)
        {
            return;
        }
   
        Vector3 targetPosition = GetTargetPosition();
        for (int i = 0; i < iterations; i++)
        {
            for (int b = 0; b < boneTransforms.Length; b++)
            {
                Transform bone = boneTransforms[b];
                float boneWeight = humanBones[b].weight * weight;
                AimAtTarget(bone, targetPosition, boneWeight);
               
            }
           
        }
        
    }

    Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = (targetTransform)- aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendout = 0.0f;

        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > angleLimit)
        {
            blendout += (targetAngle - angleLimit) / 50.0f;
        }

        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit)
        {
            blendout += distanceLimit - targetDistance;
        }
        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendout);
        return aimTransform.position + direction;
    }

    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion rotationWeight = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);    
        bone.rotation = rotationWeight * bone.rotation;
    }

    public void SetTargetTransform(Transform target, AiAgent agent)
    {
        if(target != null)
        {
            Collider targetCollider = target.gameObject.GetComponent<Collider>();
            Vector3 targetBounds = targetCollider.bounds.center;
            targetTransform = targetBounds;
            targetTransform.y = agent.sensor.GetColliderHeight(targetCollider);
        }
      
    }

    public void SetAimTransform(Transform aim)
    {
        aimTransform = aim;
    }
}
    