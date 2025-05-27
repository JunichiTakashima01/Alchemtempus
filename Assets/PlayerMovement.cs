using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Movement")]
    public float jumpPower = 10f;
    public float horizontalMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }
    public void Move(InputAction.CallbackContext context){
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context){
        if (context.performed){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
    }
}
