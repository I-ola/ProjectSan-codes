using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossHairTarget : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] private PlayerController controller;
    [SerializeField] private LayerMask aimAtLayer;
    
    Ray ray;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
   
       
        if (controller.isAiming())
        {
            RaycastHit hit;
            if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity, aimAtLayer))
            {
               transform.position = hit.point;
            }
         
          
        }
       
    }
}
