using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    [Header("Fire modes")]
    public FireMode fireMode;
    bool triggerReleasedSinceLastShot;
    public int burstCount;
    int shotsRemainingInBurst;
    private GunController controller;
    [Header("Reload")]
    public Animator anim;
    public int magazine;
    int bulletsLeftInMagazine;
    public float reloadTime = .3f;
    public float reloadDelay = .3f;
    public static bool isReloading;

    [Header("General")]
    public Transform[] barrles;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;


    public Transform shell;
    public Transform shellEjection;
    Muzzle_Flash muzzleflash;
    float nextShotTime;

    private void Awake()
    {
        controller = FindObjectOfType<GunController>();
        anim = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        muzzleflash = GetComponent<Muzzle_Flash>();
    }

    void Start()
    {
        shotsRemainingInBurst = burstCount;
        bulletsLeftInMagazine = magazine;

        if(fireMode == FireMode.Single)
        {
            triggerReleasedSinceLastShot = true;
        }
    }

    private void Update()
    {
        if(!isReloading && bulletsLeftInMagazine == 0)
        {
            StartReload();
        }
    }

    void Shoot()
    {

        if (!isReloading && Time.time > nextShotTime && bulletsLeftInMagazine > 0 && !controller.isSwapping)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < barrles.Length; i++)
            {
                if(bulletsLeftInMagazine == 0)
                {
                    break;
                }
                bulletsLeftInMagazine--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, barrles[i].position, barrles[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }


            if (fireMode == FireMode.Burst)
            {
                AudioManager.Instance.PlaySound("BurstSFX", transform.position);
            }
            else if (fireMode == FireMode.Single)
            {
                if(controller.weaponIndex == 3)
                {
                    AudioManager.Instance.PlaySound("ShotgunSFX", transform.position);
                }
                else
                {
                    AudioManager.Instance.PlaySound("SingleshotSFX", transform.position);
                }
            }
            else
            {
                AudioManager.Instance.PlaySound("FullautoSFX", transform.position);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }

    public void StartReload()
    {
        if (!isReloading && bulletsLeftInMagazine != magazine)
        {
            StartCoroutine(ReloadSoundDelay());
            StartCoroutine(AnimateReload());
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        transform.LookAt(aimPoint);
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;

        if(fireMode == FireMode.Burst)
        {
            anim.SetBool("burstfire", true);
        }
        else if(fireMode == FireMode.Single)
        {
            anim.SetBool("singleshot", true);
        }
        else
        {
            anim.SetBool("fullauto",true);
        }
    }

    public void OnTriggerRelease()
    {
        if (fireMode == FireMode.Burst)
        {
            anim.SetBool("burstfire", false);
        }
        else if (fireMode == FireMode.Single)
        {
            anim.SetBool("singleshot", false);
        }
        else
        {
            anim.SetBool("fullauto", false);
        }

        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }

    IEnumerator AnimateReload()
    {
        StartCoroutine(ReloadSoundDelay());
        isReloading = true;
        anim.SetBool("reload", true);
        yield return new WaitForSeconds(reloadTime);
        anim.SetBool("reload", false);
        bulletsLeftInMagazine = magazine;   
        isReloading = false;
    }

    IEnumerator ReloadSoundDelay()
    {
        yield return new WaitForSeconds(.1f);
        AudioManager.Instance.PlaySound("ReloadP1", transform.position);
        yield return new WaitForSeconds(reloadDelay);
        if(controller.weaponIndex == 3)
        {
            AudioManager.Instance.PlaySound("ShotgunReload", transform.position);

        }
        else
        {
            AudioManager.Instance.PlaySound("Reload", transform.position);
        }
    }
}
