using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.06f;

    [Header("Death Effects")]
    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private int shardCount = 6;
    [SerializeField] private float shardSpawnRadius = 0.15f;
    [SerializeField] private float shardBiasStrength = 0.9f;
    [SerializeField] private GameObject deathStainPrefab;

    [Header("Game Feel")]
    [SerializeField] private float deathHitStopDuration = 0.035f;
    [SerializeField] private float deathShakeDuration = 0.12f;
    [SerializeField] private float deathShakeMagnitude = 0.12f;

    private int currentHealth;
    private bool isDead = false;
    private Color originalColor;
    private Vector2 lastHitDirection = Vector2.zero;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, Vector2.zero);
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        if (hitDirection != Vector2.zero)
            lastHitDirection = hitDirection.normalized;

        currentHealth -= damage;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = flashColor;
        yield return new WaitForSecondsRealtime(flashDuration);

        if (!isDead)
            spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;

        if (GameFeelManager.Instance != null)
        {
            GameFeelManager.Instance.DoHitStop(deathHitStopDuration);
            GameFeelManager.Instance.Shake(deathShakeDuration, deathShakeMagnitude);
        }

        SpawnDeathStain();
        SpawnShards();

        Destroy(gameObject);
    }

    private void SpawnDeathStain()
    {
        if (deathStainPrefab == null) return;

        Vector3 stainPosition = transform.position + new Vector3(0f, -0.1f, 0f);
        Instantiate(deathStainPrefab, stainPosition, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
    }

    private void SpawnShards()
    {
        if (shardPrefab == null) return;

        for (int i = 0; i < shardCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shardSpawnRadius;

            GameObject shard = Instantiate(
                shardPrefab,
                (Vector2)transform.position + randomOffset,
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))
            );

            EnemyDeathShard shardScript = shard.GetComponent<EnemyDeathShard>();
            if (shardScript != null)
                shardScript.Launch(lastHitDirection, shardBiasStrength);
        }
    }
}