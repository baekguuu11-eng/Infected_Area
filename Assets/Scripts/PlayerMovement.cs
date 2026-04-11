using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Visual Settings")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Animator visualAnimator;

    [Header("Animation Settings")]
    [SerializeField] private float speedLerp = 12f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float originalVisualScaleX;
    private float currentAnimSpeed = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (visualTransform == null)
        {
            Transform foundVisual = transform.Find("Visual");
            if (foundVisual != null)
                visualTransform = foundVisual;
        }

        if (visualAnimator == null && visualTransform != null)
            visualAnimator = visualTransform.GetComponent<Animator>();

        if (visualTransform != null)
            originalVisualScaleX = Mathf.Abs(visualTransform.localScale.x);
    }

    private void Update()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.W)) y += 1f;
        if (Input.GetKey(KeyCode.S)) y -= 1f;

        movement = new Vector2(x, y).normalized;

        UpdateVisualDirection();
        UpdateAnimation(x, y);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    private void UpdateAnimation(float x, float y)
    {
        if (visualAnimator == null)
            return;

        float targetSpeed = (x != 0f || y != 0f) ? 1f : 0f;
        currentAnimSpeed = Mathf.Lerp(currentAnimSpeed, targetSpeed, Time.deltaTime * speedLerp);

        if (currentAnimSpeed < 0.01f)
            currentAnimSpeed = 0f;
        if (currentAnimSpeed > 0.99f)
            currentAnimSpeed = 1f;

        visualAnimator.SetFloat("Speed", currentAnimSpeed);
    }

    private void UpdateVisualDirection()
    {
        if (visualTransform == null)
            return;

        Vector3 currentScale = visualTransform.localScale;

        // A 누르면 원본 방향
        if (movement.x < 0f)
        {
            currentScale.x = originalVisualScaleX;
        }
        // D 누르면 반전 방향
        else if (movement.x > 0f)
        {
            currentScale.x = -originalVisualScaleX;
        }

        visualTransform.localScale = currentScale;
    }
}