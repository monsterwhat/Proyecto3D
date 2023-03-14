using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    private FirstPersonController _input;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletPoint;
    [SerializeField]
    private Transform bulletExit;
    [SerializeField]
    private float bulletSpeed = 2000;
    [SerializeField]
    private float shellSpeed = 100;
    [SerializeField]
    private float bulletLifeTime = 2;
    [SerializeField]
    private float shellLifeTime = 2f;
    [SerializeField]
    private float fireRate = 0.5f;
    [SerializeField]
    private float aimOffset = 0.5f;
    [SerializeField]
    private float zoomOffset = 0.5f;

    private Vector3 defaultPosition;
    private bool isAiming;
    private bool isZooming;

    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<FirstPersonController>();
        defaultPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.shoot)
        {
            Shoot();
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
        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.parent.rotation);
        GameObject shell = Instantiate(bulletPrefab, bulletExit.transform.position, transform.parent.rotation);

        Vector3 shootDirection = Camera.main.transform.forward;

        shell.GetComponent<Rigidbody>().AddForce(transform.right * shellSpeed);
        bullet.GetComponent<Rigidbody>().AddForce(shootDirection * bulletSpeed);
        Destroy(bullet, bulletLifeTime);
        Destroy(shell, bulletLifeTime);
    }

}
