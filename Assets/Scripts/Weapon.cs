using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;

    public PlayerMovement playerMovement;

    [Header("Shooting")]
    // Shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    // Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    // Spread
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    [Header("Bullet")]
    // Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; // seconds

    public GameObject muzzleEffect;
    internal Animator animator; // Can access it from other scripts, but can't set from inspector

    [Header("Loading")]
    // Loading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public Vector3 spawnScale;

    bool isADS;

    public enum WeaponModel
    {
        M1911,
        AK74
    }

    public WeaponModel thisWeaponModel;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    private void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (isActiveWeapon)
        {
            foreach (Transform child in transform) // We access the gameobject itself with transform, and the parts inside it with child.
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender"); // To prevent weapons from being visible behind walls
            }
            gameObject.layer = LayerMask.NameToLayer("WeaponRender");

            if (Input.GetMouseButtonDown(1) && isActiveWeapon)
            {
                EnterADS();
            }

            if (Input.GetMouseButtonUp(1) && isActiveWeapon)
            {
                ExitADS();
            }

            GetComponent<Outline>().enabled = false;

            isPlayerRun();

            // Empty Magazine Sound
            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSoundM1911.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                // Holding Down Left Mouse Button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                // Clicking Left Mouse Button Once
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            // If you want to automatically reload when magazine is empty

            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
            {
                //Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void isPlayerRun()
    {
        if (playerMovement.isMoving)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }

    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.middleDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.middleDot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if(isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            animator.SetTrigger("RECOIL");
        }


        //SoundManager.Instance.shootingSoundM1911.Play();

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        // Poiting the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;
        
        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Destroy the bullet after some time
        DestroyBulletAfterTime(bullet);

        // Checking if the we are done shooting
        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) // we already shoot once before this check
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        if(WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }

        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true; 
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // Shooting from the middle of the screen to check where are we pointing at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            // Hitting Something
            targetPoint = hit.point;
        }
        else
        {
            // Shooting at the air
            targetPoint = ray.GetPoint(100);
            Debug.Log(ray);
            Debug.Log(targetPoint);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Returning the shooting direction and spread
        return direction + new Vector3(0, y, z);
    }

    private void DestroyBulletAfterTime(GameObject bullet)
    {
        Destroy(bullet, bulletPrefabLifeTime);
    }
}
