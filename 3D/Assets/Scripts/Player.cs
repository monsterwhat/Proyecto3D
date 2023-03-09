using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float horizontalMovement;
    public float verticalMovement;
    public CharacterController player;
    private Vector3 playerInput;

    public float playerSpeed;
    private Vector3 movePlayer;

    public Camera mainCamera;
    private Vector3 camForward;
    private Vector3 camRight;



    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");
        playerInput = new Vector3 (horizontalMovement,0, verticalMovement);
        playerInput = Vector3.ClampMagnitude(playerInput, 1);

        camDirection();

        movePlayer = playerInput.x * camRight + playerInput.z * camForward;

        player.transform.LookAt(player.transform.position + movePlayer);

        player.Move(movePlayer * playerSpeed * Time.deltaTime);
    }

    private void camDirection()
    {
        camForward = mainCamera.transform.forward;
        camRight = mainCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }
}
