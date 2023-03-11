using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Objects")]
    public CharacterController playerController;
    public Animator playerAnimator;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpHeight = 1.5f;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    [SerializeField] Vector3 moveDirection;

    [Header("External forces")]
    public float gravity = -18f;
    private Vector3 velocity;

    [Header("Ground Check")]
    public float groundDistance = 0.4f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("Animator Bools")]
    public bool isWalking;
    public bool isJumping;
    public bool isAttacking;
    public bool isRunning;

    // Internal variables
    float horizontalInput;
    float verticalInput;

    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        PlayerMove();
        GroundControl();
        SpeedControl();
    }

    private void PlayerMove()
    {
        isWalking = playerAnimator.GetBool("Walk");
        isJumping = playerAnimator.GetBool("Jump");
        isAttacking = playerAnimator.GetBool("Attack");

        // Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        moveDirection = transform.right * horizontalInput
                                + transform.forward * verticalInput;

        playerController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Walking
        if (isGrounded && horizontalInput != 0)
        {
            isWalking = true;
            playerAnimator.SetFloat("XSpeed", moveDirection.x);
        }
        else if (isGrounded && verticalInput != 0)
        {
            isWalking = true;
            playerAnimator.SetFloat("YSpeed", moveDirection.z);
        }

        // Running
        if (Input.GetKeyDown(runKey) && isGrounded && horizontalInput != 0)
        {
            isRunning = true;
            playerAnimator.SetTrigger("Run");
            playerAnimator.SetFloat("XSpeed", moveDirection.x);
        }
        else if (Input.GetKeyDown(runKey) && isGrounded && verticalInput != 0)
        {
            isRunning = true;
            playerAnimator.SetTrigger("Run");
            playerAnimator.SetFloat("YSpeed", moveDirection.z);
        }

        // Jumping
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            isJumping = true;
            playerAnimator.SetBool("Jump", true);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Attacking
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            isAttacking = true;
            playerAnimator.SetBool("Attack", true);
        }
    }

    private void SpeedControl()
    {
        // For gravity and after jump
        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
    }

    private void GroundControl()
    {
        // Creates a shpere around groundCheck to check if isGrounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Just to force player to stay on the ground
        }

    }
}
