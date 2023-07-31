using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OpenAI;
using UnityEngine.InputSystem; // Import the InputSystem namespace
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private ChatGPT chatGPT;

    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [SerializeField] private TextMeshProUGUI text_speed;

    private void Start()
    {
        chatGPT = GetComponent<ChatGPT>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private bool canMove = true;

    // Method to enable/disable player movement
    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    // Update method will now check if the player can move before handling input and movement
    private void Update()
    {
        if (canMove)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
            MyInput();
            SpeedControl();
            if (grounded)
                rb.drag = groundDrag;
            else
                rb.drag = 0;
        }
        else
        {
            rb.drag = groundDrag;
        }
    }


    private void FixedUpdate()
    {
       
        MovePlayer();

    }

    private void MyInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        horizontalInput = keyboard.aKey.isPressed ? -1 : (keyboard.dKey.isPressed ? 1 : 0);
        verticalInput = keyboard.sKey.isPressed ? -1 : (keyboard.wKey.isPressed ? 1 : 0);

        // when to jump
        if (keyboard.spaceKey.isPressed && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        text_speed.SetText("Speed: " + flatVel.magnitude);
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}