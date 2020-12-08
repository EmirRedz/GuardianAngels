using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] allguns;
    Gun equippedGun;
    public int weaponIndex;
    public bool isSwapping;
    void Start()
    {
        isSwapping = false;
        EquipGun(0);
    }

    private void Update()
    {
        StartCoroutine(ChooseGun());
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int gunIndex)
    {
        EquipGun(allguns[gunIndex]);
    }

    public void OnTriggerHold()
    {
        if(equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if(equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public float GetGunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        if(equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.StartReload();
        }
    }

    public IEnumerator ChooseGun()
    {
        if (Input.GetMouseButtonDown(1) && !Gun.isReloading)
        {
            isSwapping = true;
            AudioManager.Instance.PlaySound("SwapModesSFX", transform.position);
            weaponIndex++;
            if (weaponIndex >= allguns.Length)
            {
                weaponIndex = 0;
            }
            EquipGun(weaponIndex);
            yield return new WaitForSeconds(0.6f);
            isSwapping = false; 
        }
    }
}
