using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Health health;
   
  
    public void OnRaycastHit(Weapon weapon, GameObject bodyPart, Vector3 direction)
    {         
       health.TakeDamage(weapon, bodyPart,direction);

    }

    

}
