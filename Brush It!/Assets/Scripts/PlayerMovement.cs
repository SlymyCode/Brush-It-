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
    [SerializeField] private float toIdleTransition = 0.2f;
    [SerializeField] private float walkTransition = 0.2f;
    [SerializeField] private float runTransition = 0.2f;
    [SerializeField] private float runMult = 1f;
    
    private CharacterController controller;
    public Vector3 moveInput;
    private Vector3 velocity;
    private PlayerInput playerInput;
    private float speedMultiplier = 1f;
    
    public StaminaBar stamina;
    private bool wantsToRun = false;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
    
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
        wantsToRun = context.ReadValueAsButton();
    }
    
    void Update()
    {
        if (wantsToRun && stamina.CanRun && animator.GetLayerWeight(1) == 0 && animator.GetLayerWeight(2) == 0)
        {
            speedMultiplier = runMult;
        }
        else
        {
            speedMultiplier = 1f;
        }
        
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
            /*
            else if (dir.sqrMagnitude < 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(camForward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            */
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if ((animator.GetLayerWeight(1) > 0 || animator.GetLayerWeight(2) > 0) && moveInput.sqrMagnitude > 0.01f)
        {
            float strength = 0.5f;
            speedMultiplier = 1f;
            animator.SetFloat("MovementStrength", strength, walkTransition, Time.deltaTime);
        }
        else if ((animator.GetLayerWeight(1) > 0 || animator.GetLayerWeight(2) > 0) && moveInput.sqrMagnitude < 0.01f)
        {
            float strength = 0;
            speedMultiplier = 1f;
            animator.SetFloat("MovementStrength", strength, toIdleTransition, Time.deltaTime);
        }
        else if (moveInput.sqrMagnitude > 0.01f)
        {
            float strength = (speedMultiplier > 1f) ? 1f : 0.5f;
            animator.SetFloat("MovementStrength", strength, walkTransition, Time.deltaTime);
        }
        else if (moveInput.sqrMagnitude > 0.5f)
        {
            float strength = (speedMultiplier > 1f) ? 1f : 0.5f;
            animator.SetFloat("MovementStrength", strength, runTransition, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("MovementStrength", 0f, toIdleTransition, Time.deltaTime);
        }
    }
}