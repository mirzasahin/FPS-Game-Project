using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int HP;

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
        }
        else
        {
            Debug.Log("Player Hit");
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
