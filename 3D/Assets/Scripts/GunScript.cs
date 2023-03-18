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

    //Camera Adjustment Variables
    private Vector3 defaultPosition;
    private bool isAiming;
    private bool isZooming;

    private void Awake()
    {
        // Get camera component from parent object
        fpsCam = GetComponentInParent<Camera>();
        _input = transform.root.GetComponent<AdvancedCharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = transform.localPosition;
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

    void GetAiming()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }
    }

    void GetZooming()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isZooming = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isZooming = false;
        }
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

        GetAiming();

        GetZooming();
    }

    void LateUpdate()
    {
        WeaponZoom();
    }

    void WeaponZoom()
    {
        if (isAiming)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPosition + Vector3.forward * aimOffset, Time.deltaTime * 5f);
        }
        else if (isZooming)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPosition + Vector3.forward * zoomOffset, Time.deltaTime * 5f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPosition, Time.deltaTime * 5f);
        }

        transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward * gunForwardOffset, Camera.main.transform.up);
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
