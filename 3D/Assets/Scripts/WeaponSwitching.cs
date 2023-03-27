using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

    public float weaponScale = 1.0f; // scale of weapons

    public GameObject[] weapons;

    public Transform RightHandIK;
    public Transform LeftHandIK;

    private void Awake()
    {
        LoadWeapons();
    }

    private List<Vector3> weaponPositions = new List<Vector3>();
    private List<Quaternion> weaponRotations = new List<Quaternion>();

    void LoadWeapons()
    {
        // Reset weapon positions and rotations
        weaponPositions.Clear();
        weaponRotations.Clear();

        // Instantiate all the weapons as child objects of this object
        for (int i = 0; i < weapons.Length; i++)
        {
            GameObject weaponPrefab = weapons[i];
            GameObject weapon = Instantiate(weaponPrefab, transform);
            weapon.SetActive(false);
            var gunscript = weapon.GetComponent<GunScript>();
            weaponPositions.Add(new Vector3(0, 0, 0));
            weaponRotations.Add(Quaternion.Euler(0, 0, 0));
            weapon.transform.localPosition = weaponPositions[i]; // set weapon position based on offsets
            weapon.transform.localScale = Vector3.one * weaponScale; // set weapon scale
            weapon.transform.localRotation = weaponRotations[i]; // set weapon rotation
        }

        // Select the first weapon by default
        selectedWeapon = 0;
        selectWeapon(selectedWeapon);
    }

    void WeaponSwitcher()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= weapons.Length - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = weapons.Length - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        GunScript[] currentGun = transform.GetComponentsInChildren<GunScript>();
        for (int i = 0; i < currentGun.Length; i++)
        {
            if (currentGun[i].isActiveAndEnabled)
            {
                if (currentGun[i].isReloading)
                {
                    // Don't switch weapon if the current weapon is reloading
                    selectedWeapon = previousSelectedWeapon;
                }
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            selectWeapon(selectedWeapon);
        }
    }


    public string GetHierarchyPath()
    {
        string path = gameObject.name;
        GameObject current = gameObject;

        while (current.transform.parent != null)
        {
            current = current.transform.parent.gameObject;
            path = current.name + "/" + path;
        }

        return path;
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSwitcher();
    }

    void selectWeapon(int weaponIndex)
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            GunScript gun = child.GetComponentInChildren<GunScript>();
            if (gun != null)
            {
                // Reset the weapon's position and rotation to their original values
                child.localPosition = new Vector3(0, 0, 0);
                child.localRotation = Quaternion.Euler(0,0,0);
                child.localScale = Vector3.one * gun.gunSize;

                if (i == weaponIndex)
                {
                    child.gameObject.SetActive(true);
                    gun.currentAmmo = gun.maxAmmo;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
                i++;
            }
        }
        selectedWeapon = weaponIndex;
    }
}
