using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

    public float weaponScale = 1.0f; // scale of weapons

    public GameObject[] weapons;

    public Transform LeftHand;
    public Transform RightHand;
    public Transform RightHandPosition; // position where weapon should spawn

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

        // Instantiate all the weapons as child objects of the RightHand transform
        for (int i = 0; i < weapons.Length; i++)
        {
            GameObject weaponPrefab = weapons[i];
            GameObject weapon = Instantiate(weaponPrefab, RightHand);
            weapon.SetActive(false);
            var gunscript = weapon.GetComponent<GunScript>();
            weaponPositions.Add(new Vector3(gunscript.xOffset, gunscript.yOffset, gunscript.zOffset));
            weaponRotations.Add(Quaternion.Euler(0, 0, 0));
            weapon.transform.localPosition = weaponPositions[i]; // set weapon position based on offsets
            weapon.transform.localScale = Vector3.one * weaponScale; // set weapon scale
            weapon.transform.localRotation = weaponRotations[i]; // set weapon rotation
        }

        // Select the first weapon by default
        selectedWeapon = 0;
        selectWeapon(selectedWeapon);
    }


    // Start is called before the first frame update
    void Start()
    {

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

        GunScript currentGun = RightHand.GetChild(0).GetComponentInChildren<GunScript>();
        if (currentGun != null && currentGun.isReloading)
        {
            // Don't switch weapon if the current weapon is reloading
            selectedWeapon = previousSelectedWeapon;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            selectWeapon(selectedWeapon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSwitcher();
    }

    void selectWeapon(int weaponIndex)
    {
        int i = 0;
        foreach (Transform child in RightHand)
        {
            GunScript gun = child.GetComponentInChildren<GunScript>();
            if (gun != null)
            {
                // Reset the weapon's position and rotation to their original values
                child.localPosition = new Vector3(gun.xOffset, gun.yOffset, gun.zOffset);
                child.localRotation = Quaternion.Euler(gun.gunRotations);
                child.localScale = Vector3.one * gun.gunSize;

                if (i == weaponIndex)
                {
                    child.gameObject.SetActive(true);
                    gun.currentAmmo = gun.maxAmmo;

                    //Set hand positions
                    //LeftHand.position = gun.leftHandPosition.position;
                    RightHand.position = RightHandPosition.position;

                    child.SetParent(RightHand);
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
