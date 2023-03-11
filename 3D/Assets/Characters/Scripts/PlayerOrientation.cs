using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrientation : MonoBehaviour
{
    public Transform orientation;

    void Update()
    {
        transform.position = orientation.position;
    }
}
