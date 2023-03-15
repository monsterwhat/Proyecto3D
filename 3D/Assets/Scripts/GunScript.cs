using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    private FirstPersonController _input;

    [Header("Camera")]
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
    [SerializeField] private float aimOffset = 0.5f;
    [SerializeField] private float zoomOffset = 0.5f;

    [Header("Gun Settings")]
    public ParticleSystem mussleFlash;
    public GameObject impactEffect;
    public bool isAuto = false;
    public int maxAmmo = 10;
    public float reloadTime = 1f;
    public float damage = 10f;
    public float fireRate = 15f;

    [Header("Raycast Settings")]
    public float range = 100f;
    public float impactForce = 30f;

    //Ammo Variables
    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    //Camera Adjustment Variables
    private Vector3 defaultPosition;
    private bool isAiming;
    private bool isZooming;

    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<FirstPersonController>();
        defaultPosition = transform.localPosition;
        currentAmmo = maxAmmo;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (_input.shoot)
        {
            if (!isAuto)
            {
                if (Time.time >= nextTimeToFire)
                    nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                Shoot();
            }
            _input.shoot = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }

        if (Input.GetMouseButtonDown(2))
        {
            isZooming = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isZooming = false;
        }
    }

    void LateUpdate()
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

        transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward * 100f, Camera.main.transform.up);
    }

    void Shoot()
    {
        mussleFlash.Play();
        currentAmmo--;

        switch (isBallistic)
        {
            //If the bullet is ballistic, spawn a bullet and a shell
            case true:
                GameObject bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.parent.rotation);
                GameObject shell = Instantiate(bulletPrefab, bulletExit.transform.position, transform.parent.rotation);

                Vector3 shootDirection = Camera.main.transform.forward;

                shell.GetComponent<Rigidbody>().AddForce(transform.right * shellSpeed);
                bullet.GetComponent<Rigidbody>().AddForce(shootDirection * bulletSpeed);
                Destroy(bullet, bulletLifeTime);
                Destroy(shell, shellLifeTime);
                break;

            //If the bullet is raycast, shoot a raycast
            case false:
                RaycastHit hit;
                if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
                {
                    Debug.Log(hit.transform.name);

                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * impactForce);
                    }

                    GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 2f);
                }
                break;
        }   
    }

}
