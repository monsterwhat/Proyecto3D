using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
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


    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<FirstPersonController>();

    }

    // Update is called once per frame
    void Update()
    {
        if(_input.shoot) {
            Shoot();
            _input.shoot = false;
        }
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
