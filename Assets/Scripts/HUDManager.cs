using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;

    public GameObject middleDot;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;

            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }
        
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        GameObject weaponPrefab = null;

        switch (model)
        {
            case Weapon.WeaponModel.M1911:
                weaponPrefab = Resources.Load<GameObject>("M1911_Weapon");
                break;

            case Weapon.WeaponModel.AK74:
                weaponPrefab = Resources.Load<GameObject>("AK74_Weapon");
                break;

            default:
                return null;
        }

        if (weaponPrefab != null)
        {
            SpriteRenderer spriteRenderer = weaponPrefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                return spriteRenderer.sprite;
            }
            else
            {
                Debug.LogError("SpriteRenderer not found on the weapon prefab.");
            }
        }
        else
        {
            Debug.LogError($"Prefab not found for weapon model: {model}");
        }

        return null; // Hata durumunda null döndürülebilir veya baþka bir varsayýlan sprite atanabilir.
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        GameObject ammoPrefab = null;

        switch (model)
        {
            case Weapon.WeaponModel.M1911:
                ammoPrefab = Resources.Load<GameObject>("M1911_Ammo");
                break;

            case Weapon.WeaponModel.AK74:
                ammoPrefab = Resources.Load<GameObject>("AK74_Ammo");
                break;

            default:
                return null;
        }

        if (ammoPrefab != null)
        {
            SpriteRenderer spriteRenderer = ammoPrefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                return spriteRenderer.sprite;
            }
            else
            {
                Debug.LogError("SpriteRenderer not found on the ammo prefab.");
            }
        }
        else
        {
            Debug.LogError($"Prefab not found for ammo model: {model}");
        }

        return null; // Hata durumunda null döndürülebilir veya baþka bir varsayýlan sprite atanabilir.
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if(weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        // this will never happen, but we need to return something
        return null;
    }
}
