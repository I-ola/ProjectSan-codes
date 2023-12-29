using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
    public float pursueSpeed = 5.0f;
    public float attackSpeed = 6f;
    public float attackStoppingDistance = 4f;
    public float normalStoppingDistance = 2f;
    public float patrolSpeed = 2.0f;
    public float crouchIdleSpeed = 0.0f;
    public float goToCoverSpeed = 5.0f;
    public float idleSpeed = 1.0f;
    public float suspectWalkingSpeed = 2.0f;
    public float soundIntensityMax = 0.4f;
    public float scanLocationIntensity = 0.4f;
    public float checkLocationIntensity = 1f;
    public float stopSpeed = 0f;
}
