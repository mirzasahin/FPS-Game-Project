using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int HP;
    public GameObject bloodyScreen;

    // Start is called before the first frame update

    private void Start()
    {
        HP = 100;
    }

    public void TakeDamage(int damageAmout)
    {
        HP -= damageAmout;

        if(HP <= 0)
        {
            Debug.Log("Player Dead..");
            // Game over
            // Re Spawn Player
            // Dying Animation
        }
        else
        {
            Debug.Log("Player Hit");
            StartCoroutine(BloodyScreenEffect());

        }
    }

    private IEnumerator BloodyScreenEffect()
    {
        if(bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;   

        float duration = 3f;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // if t = 0 result will be equals a(1), if t = 1 result will be equals b(0)

            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand"))
        {
            TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
        }
    }
}
