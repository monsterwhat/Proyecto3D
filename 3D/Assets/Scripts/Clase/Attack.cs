using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject[] weapons;
    public bool weaponActive = false;
    public Animator animator;
    public int weaponLevel = 0;
    public GameObject bullet;
    public GameObject weapon;
   

    public void SetWeapon(int level)
    {
        weaponLevel = level;
        weaponActive = true;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        weapons[level].SetActive(true);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (weaponActive)
        {
            if (Input.GetButton("Fire1"))
            {
                if(weaponLevel == 0)
                {
                    //Here you should switch the whole animator 
                    animator.Play("Sword");
                }
                else if (weaponLevel==1)
                {
                    //Shoot gun (Pistol)
                    StartCoroutine(Shoot());
                }
                else if (weaponLevel==2)
                {
                    //Shoot gun (MachineGun)
                    StartCoroutine(Shoot());
                }
            }
        }
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(1);
        GameObject BulletTemp = Instantiate(bullet, weapon.transform.position, Quaternion.identity);


        Destroy(BulletTemp, 3);
    }

}
