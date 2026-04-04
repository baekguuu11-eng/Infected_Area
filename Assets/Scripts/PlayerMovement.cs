using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A))
            x -= 1f;
        if (Input.GetKey(KeyCode.D))
            x += 1f;
        if (Input.GetKey(KeyCode.S))
            y -= 1f;
        if (Input.GetKey(KeyCode.W))
            y += 1f;

        movement = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}