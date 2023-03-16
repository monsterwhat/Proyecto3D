using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MoveWithPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController player;
    Vector3 groundPosition;
    Vector3 lastgroundPosition;
    string groundName;
    string lastGroundName;

    void Start()
    {
        player = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isGrounded)
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, player.transform.position.y / 4.2f, -transform.up, out hit))
            {
                GameObject groundIn = hit.collider.gameObject;
                groundName = groundIn.name;
                groundPosition = groundIn.transform.position;
                if(groundPosition != lastgroundPosition && groundName == lastGroundName)
                {
                    System.Diagnostics.Debug.WriteLine("Ground: " + groundName);
                    this.transform.position += groundPosition - lastgroundPosition;
                }
                lastGroundName = groundName;
                lastgroundPosition = groundPosition;
            }
        }
    }

    private void OnDrawGizmos()
    {
        player = this.GetComponent<CharacterController>();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2);
    }

}
