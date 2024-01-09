using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public Player playerAimCamera;
    [HideInInspector] public Cinemachine.CinemachineImpulseSource cameraShake;
    [HideInInspector] public Animator rigController;
   
    float verticalRecoil;
    float horizontalRecoil;
    public float duration;
    float minVertical;
    float maxVertical;
    float minHorizontal;
    public float maxHorizontal;
    float time;

    private void Awake()
    {
        cameraShake = GetComponent<CinemachineImpulseSource>();
    }
    public void GenerateRecoil(string weaponName)
    {
        time = duration;
        cameraShake.GenerateImpulse(Camera.main.transform.forward);

        rigController.Play("Recoil", 1, 0.0f);

        if(weaponName == "Pistol")
        {
            maxHorizontal = 0.0f;
            minHorizontal = 0.0f;
            minVertical = 0.0f;
            maxVertical = 1.0f;
        }

        if(weaponName == "Rifle") 
        {
            minVertical = 1.0f;
            maxVertical = 2.0f;
            minHorizontal = -2.0f;
            maxHorizontal = 2.0f;
        }

        if(weaponName == "SMG")
        {
            minVertical = 1.0f;
            maxVertical = 2.0f;
            minHorizontal = -2.0f;
            maxHorizontal = 2.0f;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if(time > 0)
        {
            verticalRecoil = Random.Range(minVertical, maxVertical);
            horizontalRecoil = Random.Range(minHorizontal, maxHorizontal);
            playerAimCamera.cameraTargetPitch -= (verticalRecoil* Time.deltaTime) / duration;
            playerAimCamera.cameraTargetYaw -= (horizontalRecoil * Time.deltaTime) / duration;

            time -= Time.deltaTime;
           

        }
        
    }
}
