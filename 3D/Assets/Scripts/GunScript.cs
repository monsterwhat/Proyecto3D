using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GunScript : MonoBehaviour
{
    private AdvancedCharacterController _input;

    [Header("Camera (Auto)")]
    public Camera fpsCam;

    [Header("Prefabs")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Bullet Points")]
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private Transform bulletExit;

    [Header("Hand Placement")]
    public Transform leftHandPosition;
    [Header("Hand Rotations")]
    public float xOffset;
    public float yOffset;
    public float zOffset;
    [Header("Gun offset")]
    public float gunOffsetX;
    public float gunOffsetY;
    public float gunOffsetZ;

    [Range(-1f, 1f)]
    public float weaponRotation;

    [Header("Type of Bullets")]
    [SerializeField] private bool isBallistic = false;

    [Header("Ballistic Settings")]
    [SerializeField] private float bulletSpeed = 2000;
    [SerializeField] private float shellSpeed = 100;
    [SerializeField] private float bulletLifeTime = 2;
    [SerializeField] private float shellLifeTime = 2f;

    [Header("Camera Settings")]
    public float aimOffset = 0.5f;
    public float zoomOffset = 0.5f;
    public float gunSize = 1;
    public float gunForwardOffset = 100f;

    [Header("Gun Settings")]
    public ParticleSystem mussleFlash;
    public GameObject impactEffect;
    public int maxAmmo = 10;
    public float reloadTime = 1f;
    public float damage = 10f;
    public float fireRate = 15f;
    public float gunRecoilMultiplier = 1f;
    public float recoilAmount = 0.02f;


    [Header("Raycast Settings")]
    public float range = 100f;
    public float impactForce = 30f;
    public float impactDuration = 0.2f;

    //Ammo Variables
    public int currentAmmo;
    public bool isReloading = false;
    private float nextTimeToFire = 0f;

    private void Awake()
    {
        // Get camera component from parent object
        fpsCam = transform.root.GetComponent<Camera>();
        _input = transform.root.GetComponent<AdvancedCharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");

        // Show reloading message
        GameObject canvas = GameObject.Find("AmmoHUD");
        Text reloadingText = canvas.transform.Find("ReloadText").GetComponent<Text>();
        reloadingText.text = "Reloading...";
        reloadingText.enabled = true;

        yield return new WaitForSeconds(reloadTime);

        // Hide reloading message
        reloadingText.enabled = false;

        currentAmmo = maxAmmo;
        isReloading = false;
        nextTimeToFire = 0f;
    }

    void UpdateAmmoCounter()
    {
        GameObject canvas = GameObject.Find("AmmoHUD");
        Text ammoText = canvas.transform.Find("CurrentAmmo").GetComponent<Text>();

        string bulletSymbols = "";
        for (int i = 0; i < currentAmmo; i++)
        {
            bulletSymbols += "I";
        }

        ammoText.text = bulletSymbols;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAmmoCounter();

        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentAmmo < maxAmmo && !isReloading)
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                return;
            }
        }
    }

    void GunRecoil()
    {
        Vector3 recoilRotation = new Vector3(Random.Range(-recoilAmount, recoilAmount), Random.Range(-recoilAmount, recoilAmount), 0f) * gunRecoilMultiplier;
        transform.localRotation *= Quaternion.Euler(recoilRotation);

        Vector3 recoilPosition = new Vector3(Random.Range(-recoilAmount, recoilAmount), Random.Range(-recoilAmount, recoilAmount), 0f) * gunRecoilMultiplier;
        transform.localPosition += recoilPosition;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
    }

    void ShootBallistic()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.parent.rotation);
        GameObject shell = Instantiate(bulletPrefab, bulletExit.transform.position, transform.parent.rotation);

        Vector3 shootDirection = Camera.main.transform.forward;

        shell.GetComponent<Rigidbody>().AddForce(transform.right * shellSpeed);
        bullet.GetComponent<Rigidbody>().AddForce(shootDirection * bulletSpeed);
        Destroy(bullet, bulletLifeTime);
        Destroy(shell, shellLifeTime);
    }

    void ShootRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, impactDuration);
        }
        else return;
    }

    void Shoot()
    {
        mussleFlash.Play();
        currentAmmo--;

        if (!isReloading)
        {
            switch (isBallistic)
            {
                //If the bullet is ballistic, spawn a bullet and a shell
                case true:
                    ShootBallistic();
                    break;

                //If the bullet is raycast, shoot a raycast
                case false:
                    ShootRayCast();
                    GunRecoil();
                    break;
            }
        }
        _input.shoot = false;
    }
}
