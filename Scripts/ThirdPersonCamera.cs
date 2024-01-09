using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class ThirdPersonCamera : MonoBehaviour
{
   // [SerializeField] private Transform player;
    private CinemachineVirtualCamera aimCamera;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject crossHair;
    private Player player;
    private PlayerController playerController;


    

    private void Awake()
    {
        
        
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        aimCamera = GetComponent<CinemachineVirtualCamera>();
        playerController = player1.GetComponent<PlayerController>();
        player = player1.GetComponent<Player>();    
    }

    private void Update()
    {
      
        if (aimCamera.Priority >= 10)
        {
            crossHair.SetActive(true);
        }

        if(aimCamera.Priority <= 10)
        {
            crossHair.SetActive(false);
        }

        if (playerController.aiming)
        {
            player.SetSensitivity(UserSettings.instance.aimSensitivity);
          

        }
        else if (!playerController.aiming)
        {
            player.SetSensitivity(UserSettings.instance.normalSensitivity);
           
        }


        if (player.isDead)
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
        }
    }
}
      