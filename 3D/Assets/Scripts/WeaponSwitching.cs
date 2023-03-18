using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

    public float weaponDistance = 1.0f; // distance between weapons
    public float weaponScale = 1.0f; // scale of weapons
    public float weaponRotation = 0.0f; // rotation of weapons around Y axis

    public GameObject[] weapons;

    private void Awake()
    {
        LoadWeapons();
    }

    void LoadWeapons()
    {
        // Instantiate all the weapons as child objects of the WeaponHolder
        foreach (GameObject weaponPrefab in weapons)
        {
            GameObject weapon = Instantiate(weaponPrefab, transform);
            weapon.SetActive(false);
            weapon.transform.SetParent(transform); // set weapon's parent to WeaponHolder
            weapon.transform.localScale = Vector3.one * weaponScale; // set weapon scale
            weapon.transform.localRotation = Quaternion.Euler(0, weaponRotation, 0); // set weapon rotation
        }

        // Select the first weapon by default
        selectedWeapon = 0;
        selectWeapon();
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
            if (selectedWeapon >= transform.childCount - 1)
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
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            selectWeapon();
        }
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSwitcher();
    }

    void selectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            GunScript gun = weapon.GetComponentInChildren<GunScript>();
            weapon.localPosition = Vector3.zero;
            // Set the scale and rotation of the weapon
            weapon.localScale = Vector3.one * gun.gunSize;
            weapon.localRotation = Quaternion.Euler(0, weaponRotation, 0);

            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                gun.currentAmmo = gun.maxAmmo;
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
        
    }
}
