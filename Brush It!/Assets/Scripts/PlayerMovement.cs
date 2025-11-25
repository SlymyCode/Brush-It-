using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private bool ShouldFaceMoveDirection = false;
    [SerializeField] private float animTransition = 0.2f;
    [SerializeField] private float runMult = 1f;
    
    private CharacterController controller;
    public Vector3 moveInput;
    private Vector3 velocity;
    private PlayerInput playerInput;
    private float speedMultiplier = 1f;
    
    private Vector2 lookInput;
    private float turnValue = 0f;
    [SerializeField] private float turningSpeed = 10f;
    
    [SerializeField] private float turnLayerSmoothSpeed = 5f;
    private float turnLayerWeight = 0f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            speedMultiplier = runMult;
        }
        else
        {
            speedMultiplier = 1f;
        }
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

//    public void OnJump(InputAction.CallbackContext context)
//   {
//        if (context.performed && controller.isGrounded)
//        {
//            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
//        }
//    }
    
    void Update()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();
        
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        float currentSpeed = Mathf.Lerp(speed, speed * speedMultiplier, animator.GetFloat("MovementStrength"));
        controller.Move(moveDirection * (currentSpeed * Time.deltaTime));
        
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        if (ShouldFaceMoveDirection)
        {
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            forward.Normalize();
            right.Normalize();

            Vector3 dir = forward * moveInput.y + right * moveInput.x;
            
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            else if (dir.sqrMagnitude < 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(camForward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        if (moveInput.sqrMagnitude > 0.01f)
        {
            float strength = (speedMultiplier > 1f) ? 1f : 0.5f;
            animator.SetFloat("MovementStrength", strength, animTransition, Time.deltaTime);
            turnLayerWeight = Mathf.Lerp(turnLayerWeight, 0f, turnLayerSmoothSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetFloat("MovementStrength", 0f, animTransition, Time.deltaTime);
            turnLayerWeight = Mathf.Lerp(turnLayerWeight, 0.8f, turnLayerSmoothSpeed * Time.deltaTime);
        }
        
        float rawTurn = Mathf.Clamp(lookInput.x, -1f, 1f);
        turnValue = Mathf.Lerp(turnValue, rawTurn, turningSpeed * Time.deltaTime);

        animator.SetFloat("Turn", turnValue);
        
        animator.SetLayerWeight(1, turnLayerWeight);
    }
}