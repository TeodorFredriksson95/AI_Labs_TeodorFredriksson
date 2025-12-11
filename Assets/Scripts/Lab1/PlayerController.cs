using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    public Vector2 moveVal;
    private Rigidbody rb;
    Vector3 input;
    private bool isGrounded = false;
    [SerializeField] float jumpForce = 5f;
    private Vector2 lookVal;
    Quaternion rotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void Move(InputAction.CallbackContext value)
    {
        moveVal = value.ReadValue<Vector2>();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("player jump pressed");

        if (context.performed && isGrounded)
        {
            Debug.Log("entered jump");

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    
    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        Quaternion playerRotation = Quaternion.Euler(Vector3.up * (x * 150 * Time.fixedDeltaTime));
        transform.Rotate(0, x * 150 * Time.deltaTime, 0);

    }

    private void FixedUpdate()
    {
        Vector3 moveDir = transform.forward * moveVal.y;
        Vector3 nextPos = rb.position + moveDir * moveSpeed * Time.deltaTime;
        rb.MovePosition(nextPos);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }
}
