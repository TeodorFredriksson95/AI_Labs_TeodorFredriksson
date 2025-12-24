using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    private bool isGrounded = false;

    public Vector3 playerVelocity;
    public Vector2 moveVal;
    private Rigidbody rb;

    private bool applyForce;
    private Vector3 knockBackForce;
    private bool canMove = true;

    [SerializeField] RollingBallController rbc;

    private float catchBallCooldownTimer = 10f;
    private float catchBallCooldownCounter = 0f;
    private bool canCatchBall => catchBallCooldownCounter <= 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

    }

    public void Move(InputAction.CallbackContext value)
    {
        moveVal = value.ReadValue<Vector2>();
    }


    public void OnJump(InputAction.CallbackContext context)
    {

        if (context.performed && isGrounded)
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        Quaternion playerRotation = Quaternion.Euler(Vector3.up * (x * 150 * Time.fixedDeltaTime));
        transform.Rotate(0, x * 150 * Time.deltaTime, 0);

        DidPlayerCatchBall();
        
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = transform.forward * moveVal.y;
        Vector3 nextPos = rb.position + moveDir * moveSpeed * Time.deltaTime;
        playerVelocity = rb.linearVelocity;

        if (canMove)
            rb.MovePosition(nextPos);


        if (applyForce)
        {
            ApplyKnockback();
        }
    }

    public void ApplyKnockback()
    {
        rb.AddForce(knockBackForce, ForceMode.Impulse);
        applyForce = false;
        canMove = false;
    }

    public void SetKnockback(Vector3 force)
    {
        applyForce = true;
        knockBackForce = force;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canMove = true;
            isGrounded = true;
        }

    }

    private bool DidPlayerCatchBall()
    {
        if (canCatchBall)
        {
            if (IsBallCaught())
            {
                RuntimeUI.Instance.UpdateScoreLabel();
                catchBallCooldownCounter = catchBallCooldownTimer;
                return true;
            }
        }

        catchBallCooldownCounter -= Time.deltaTime;
        return false;
    }

    private bool IsBallCaught()
    {

        float distance = Vector3.Distance(transform.position, rbc.transform.position);
        if (distance < 2f)
            return true;

        return false;
    }
}
