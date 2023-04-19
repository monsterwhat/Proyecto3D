using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public bool isPickable = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "playerInteractionZone")
        {
            other.GetComponent<PickUpObject>().ObjectToPickUp = this.gameObject;
        }
    }
}
