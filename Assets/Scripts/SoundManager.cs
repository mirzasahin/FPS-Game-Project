using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource ShootingChannel;

    public AudioClip M1911Shot;
    public AudioClip AK74Shot;

    public AudioSource reloadingSoundAK74;
    public AudioSource reloadingSoundM1911;

    public AudioSource emptyMagazineSoundM1911;

    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;

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

    public void PlayShootingSound(Weapon.WeaponModel weapon)
    {
        switch (weapon)
        {
            case Weapon.WeaponModel.M1911:
                ShootingChannel.PlayOneShot(M1911Shot);
                break;
            case Weapon.WeaponModel.AK74:
                ShootingChannel.PlayOneShot(AK74Shot);
                break;
        }
    }

    public void PlayReloadSound(Weapon.WeaponModel weapon)
    {
        switch (weapon)
        {
            case Weapon.WeaponModel.M1911:
                reloadingSoundM1911.Play();
                break;
            case Weapon.WeaponModel.AK74:
                reloadingSoundAK74.Play();
                break;
        }
    }
    
}
