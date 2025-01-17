using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class PlayerController : MonoBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
 
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    Animator myAnimator;

    [HideInInspector]
    public bool canMove = true;
 
    [SerializeField]
    private Camera playerCamera;
 
    private Alteruna.Avatar _avatar;
 
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        _avatar = GetComponent<Alteruna.Avatar>();
 
        if (!_avatar.IsMe)
            return;
        
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        playerCamera.transform.SetParent(transform);
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
 
    void Update()
    {
        if (!_avatar.IsMe)
            return;
        // Yes
        bool isRunning = false;
 
        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);
 
        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKeyDown(KeyCode.W))
        {
            myAnimator.SetBool("RunForward", true);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            myAnimator.SetBool("strafeRight", true);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            myAnimator.SetBool("strafeLeft", true);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            myAnimator.SetBool("Backwards", true);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            myAnimator.SetBool("RunForward", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            myAnimator.SetBool("strafeRight", false);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            myAnimator.SetBool("strafeLeft", false);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            myAnimator.SetBool("Backwards", false);
        }
        if (characterController.isGrounded)
        {
            myAnimator.SetBool("Jump", false);
        }

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            myAnimator.SetBool("Jump", true);
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
 
        // Player and Camera rotation
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}