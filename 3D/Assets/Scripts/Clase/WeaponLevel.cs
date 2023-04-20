using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLevel : MonoBehaviour
{
    public int weaponLevel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Attack>().SetWeapon(weaponLevel);
            Destroy(gameObject);
        }
    }
}
