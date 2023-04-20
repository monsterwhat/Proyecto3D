using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 6;
    //public Transform playerTransform;
    public Vector3 forward;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        forward = player.gameObject.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += forward * moveSpeed * Time.deltaTime;
    }
}
