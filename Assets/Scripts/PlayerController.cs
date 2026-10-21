using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool faceMoveDirection;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runSpeedMultiplier = 1.5f;

    private Animator animator;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isRunning;
    private GameUI gameUI;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        gameUI = FindAnyObjectByType<GameUI>();


    }

    void Update()
    {
        UpdateAnimator();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed) isRunning = true;
        else if (context.canceled) isRunning = false;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = forward * moveInput.y + right * moveInput.x;

        Vector3 velocity = rb.linearVelocity;

        velocity.x = movement.x * moveSpeed;
        velocity.z = movement.z * moveSpeed;

        rb.linearVelocity = velocity;

        if (faceMoveDirection && movement.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime));
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            Coin coin = collision.gameObject.GetComponent<Coin>();

            if (coin != null)
            {
                gameUI.AddScore(coin.GetScoreValue());
                Destroy(coin.gameObject);
            }
        }
    }



    void UpdateAnimator()
    {
        if (animator == null) return;

        float currentSpeed = new Vector3(moveInput.x, 0f, moveInput.y).magnitude;

        // Check directly if Left Shift is held
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift);

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("isRunning", shiftHeld);
    }
}
