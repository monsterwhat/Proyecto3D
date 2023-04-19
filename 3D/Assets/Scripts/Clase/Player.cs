using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float horizontalMove;
    public float verticalMove;
    public CharacterController player;
    private Vector3 playerInput;
    public KeyCode sprintJoystick = KeyCode.JoystickButton2;
    public KeyCode sprintKeyboard = KeyCode.Space;

    public float playerSpeed;
    private Vector3 movePlayer;
    public float gravity = 9.8f;
    public float fallVelocity;
    public float jumpVelocity;

    public Camera mainCamera;
    private Vector3 camForward;
    private Vector3 camRight;

    public Animator playerAnimatorController;

    public bool isOnSlope = false;
    private bool isSprinting = false;
    private Vector3 hitNormal;
    public float slideVelocity;
    public float slopeForceDown;

    void Start()
    {
        player = GetComponent<CharacterController>();
        playerAnimatorController = GetComponent<Animator>();
    }

    void Update()
    {
        if (GetComponent<Health>().lives > 0)
        {
            horizontalMove = Input.GetAxis("Horizontal");
            verticalMove = Input.GetAxis("Vertical");
            playerInput = new Vector3(horizontalMove, 0, verticalMove);
            playerInput = Vector3.ClampMagnitude(playerInput, 1);

            playerAnimatorController.SetFloat("WalkVelocity", playerInput.magnitude * playerSpeed);

            camDirection();

            movePlayer = playerInput.x * camRight + playerInput.z * camForward;

            movePlayer = movePlayer * playerSpeed;

            player.transform.LookAt(player.transform.position + movePlayer);

            SetGravity();

            PlayerSkills();

            player.Move(movePlayer * playerSpeed * Time.deltaTime);
            playerAnimatorController.SetFloat("Speed", playerSpeed);
            playerAnimatorController.SetFloat("Direction", verticalMove);
            //playerAnimatorController.SetBool("isSprinting", isSprinting);

        }
    }

    void camDirection()
    {
        camForward = mainCamera.transform.forward;
        camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }

    void SetGravity()
    {
        if (player.isGrounded)
        {
            fallVelocity = -gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;
        }
        else
        {
            fallVelocity -= gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;
            playerAnimatorController.SetFloat("VerticalVelocity", player.velocity.y);
        }
        playerAnimatorController.SetBool("IsGrounded", player.isGrounded);
        SlideDown();
    }

    public void SlideDown()
    {
        isOnSlope = Vector3.Angle(Vector3.up, hitNormal) >= player.slopeLimit;
        if (isOnSlope)
        {
            movePlayer.x += ((1f - hitNormal.x) * hitNormal.x) * slideVelocity;
            movePlayer.z += ((1f - hitNormal.z) * hitNormal.z) * slideVelocity;
            movePlayer.y = slopeForceDown;
        }
    }

    void PlayerSkills()
    {
        if (player.isGrounded && Input.GetButton("Jump"))
        {
            fallVelocity = jumpVelocity;
            movePlayer.y = fallVelocity;
            playerAnimatorController.SetTrigger("Jump");
        }
    }

    private void OnAnimatorMove()
    {

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

}
