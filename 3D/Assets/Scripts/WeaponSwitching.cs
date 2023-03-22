using System;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

    public float weaponDistance = 1.0f; // distance between weapons
    public float weaponScale = 1.0f; // scale of weapons
    public float weaponRotation = 0.0f; // rotation of weapons around Y axis

    public GameObject[] weapons;

    public Transform LeftHand;
    public Transform RightHand;

    private void Awake()
    {
        LoadWeapons();
    }

    void LoadWeapons()
    {
        // Instantiate all the weapons as child objects of the RightHand transform
        foreach (GameObject weaponPrefab in weapons)
        {
            GameObject weapon = Instantiate(weaponPrefab, RightHand);
            weapon.SetActive(false);
            weapon.transform.localPosition = Vector3.zero; // set weapon position to (0, 0, 0) relative to the RightHand
            weapon.transform.localScale = Vector3.one * weaponScale; // set weapon scale
            weapon.transform.localRotation = Quaternion.Euler(0, weaponRotation, 0); // set weapon rotation
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
                child.localPosition = Vector3.zero;
                // Set the scale and rotation of the weapon
                child.localScale = Vector3.one * gun.gunSize;
                child.localRotation = Quaternion.Euler(0, weaponRotation, 0);

                if (i == weaponIndex)
                {
                    child.gameObject.SetActive(true);
                    gun.currentAmmo = gun.maxAmmo;

                    //Set hand positions
                    LeftHand.position = gun.leftHandPosition.position;
                    //RightHand.position = gun.rightHandPosition.position;

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
