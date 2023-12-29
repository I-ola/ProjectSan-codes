using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public Sprite weaponSprite;
    public WeaponRecoil recoil;
    public RuntimeAnimatorController weaponAnim;
    public RuntimeAnimatorController weaponAnimAI;
    [HideInInspector] public bool isFiring = false;
    [HideInInspector] public bool isDropped = false;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] public Transform bulletPathRaycast;
    [SerializeField] private TrailRenderer bulletEffect;

    //[SerializeField] public AnimationClip weaponAnimation;
    public string weaponName;

    public LayerMask hitMask;
   

    [SerializeField] private int fireRate = 25;

    [SerializeField] private float timeTillNextBullet;

    [SerializeField] float bulletSpeed = 300f;
    [SerializeField] float bulletDrop = 9.81f;

    private float bulletMaxTime = 3f;

    List<BulletScript> bullets = new List<BulletScript>();
    public PlayerWeaponManager.WeaponSlots weaponSlots;
    public PlayerWeaponManager.WeaponType weaponType;
    [HideInInspector] public bool isAssault
    {
        get
        {
            return weaponType == PlayerWeaponManager.WeaponType.Assault;
        }
    }

    [HideInInspector]
    public bool isPistol
    {
        get
        {
            return weaponType == PlayerWeaponManager.WeaponType.Pistol;
        }
    }

    public bool isSMG
    {
        get
        {
            return weaponType == PlayerWeaponManager.WeaponType.SMG;
        }
    }

    public GameObject magazine;

   
    public int ammoCount;
    public int reloadAmount;
    public int availableAmmo;
    public int magazineSize;
    public int bulletUsed;
    public float damage = 10.0f;

    [HideInInspector] public bool withPlayer;

    RaycastHit hitInfo;
    Ray ray;

    void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
        ammoCount = magazineSize;
    }

    private void Update()
    {
        withPlayer = this.gameObject.GetComponentInParent<Player>();
    }
    public void StartFiring()
    {
        isFiring = true;
        if (timeTillNextBullet > 0.0f)
        {
            timeTillNextBullet = 0.0f;
        }
        //FireBullet();
    }

    public void StopFiring()
    {
        isFiring = false;
    }

    public void UpdateWeapon(float deltaTime, Vector3 target)
    {
        if (isFiring && ammoCount > 0)
        {
            UpdateFiring(deltaTime, target);
        }


        timeTillNextBullet += deltaTime;

        UpdateBullet(deltaTime);
    }
    public void UpdateFiring(float deltaTime , Vector3 target)
    {
        float fireInterval = 1.0f / fireRate;
        while(timeTillNextBullet >= 0)
        {
             FireBullet(target);
            timeTillNextBullet -= fireInterval;
        }
    }

    public void FireBullet(Vector3 target)
    {
        if(ammoCount <= 0)
        {
            return;
        }

        ammoCount--;
        bulletUsed = magazineSize - ammoCount;
        muzzleFlash.Emit(1);

        Vector3 velocity = (target - bulletPathRaycast.position).normalized * bulletSpeed;
        var bullet = CreateBullet(bulletPathRaycast.position, velocity);
        bullets.Add(bullet);

        if (withPlayer)
        {
            recoil.GenerateRecoil(weaponName);
        }
        
     
    }
    Vector3 GetPosition(BulletScript bullet)
    {
        //Xt =  Xi + v*t + 1/2 g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    BulletScript CreateBullet(Vector3 position, Vector3 velocity)
    {
        BulletScript bullet = new BulletScript();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(bulletEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }

    public void UpdateBullet(float deltaTime)
    {
        SimulateBullet(deltaTime);
        DestroyBullet();
    }

    private void SimulateBullet(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 x0 = GetPosition(bullet);// get the currrent position of bullet 
            bullet.time += deltaTime;
            Vector3 x1 = GetPosition(bullet);
            RayCastSegment(x0, x1, bullet);
        });
    }

    private void RayCastSegment(Vector3 start, Vector3 end, BulletScript bullet)
    {
        Vector3 direction = (end - start);
        float range = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo, range, hitMask))
        {
           // Debug.DrawLine(bulletPathRaycast.position, hitInfo.point, Color.red, 2.0f);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = bulletMaxTime;
            //Debug.Log(ray.direction);
            var hitBox = hitInfo.collider.GetComponent<Hitbox>();
            if (hitBox)
            {
                GameObject bodypartHit = hitInfo.collider.gameObject;
                hitBox.OnRaycastHit(this, bodypartHit,direction);
            }
        }
        else
        {
            bullet.tracer.transform.position = end;
            bullet.time = bulletMaxTime;
        }

       
    }

    private void DestroyBullet()
    {
        bullets.RemoveAll(bullet => bullet.time >= bulletMaxTime);

    }

   
}
