using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AiStateId
{
    Idle,
    Pursue,
    Dead,
    Attack,
    Patrol,
    Suspect,
    GoToCover,
    CoverAttack
}
public interface AiStates 
{
   AiStateId GetId();
   void Enter(AiAgent agent);
   void Update(AiAgent agent);
   void Exit(AiAgent agent);

}
