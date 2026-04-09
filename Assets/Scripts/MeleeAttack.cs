using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private GameObject weaponVisual;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackAngle = 120f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private float swingDuration = 0.12f;
    [SerializeField] private float swingArc = 100f;
    [SerializeField] private float attackOriginDistance = 0.6f;

    private bool isAttacking = false;
    private Vector2 lastAttackDirection = Vector2.down;

    private void Start()
    {
        if (weaponVisual != null)
            weaponVisual.SetActive(false);

        UpdateAttackOriginPosition();
        UpdateWeaponBaseRotation();
    }

    private void Update()
    {
        UpdateAttackDirection();

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            StartCoroutine(DoAttack());
        }
    }

    private void UpdateAttackDirection()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            lastAttackDirection = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow))
            lastAttackDirection = Vector2.down;
        else if (Input.GetKey(KeyCode.LeftArrow))
            lastAttackDirection = Vector2.left;
        else if (Input.GetKey(KeyCode.RightArrow))
            lastAttackDirection = Vector2.right;

        UpdateAttackOriginPosition();
        UpdateWeaponBaseRotation();
    }

    private void UpdateAttackOriginPosition()
    {
        if (attackOrigin != null)
            attackOrigin.localPosition = (Vector3)(lastAttackDirection * attackOriginDistance);
    }

    private void UpdateWeaponBaseRotation()
    {
        if (weaponPivot == null) return;

        float baseAngle = Mathf.Atan2(lastAttackDirection.y, lastAttackDirection.x) * Mathf.Rad2Deg;
        weaponPivot.localRotation = Quaternion.Euler(0f, 0f, baseAngle);
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;

        float baseAngle = Mathf.Atan2(lastAttackDirection.y, lastAttackDirection.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - swingArc * 0.5f;
        float endAngle = baseAngle + swingArc * 0.5f;

        if (weaponVisual != null)
            weaponVisual.SetActive(true);

        DealDamage();

        float elapsed = 0f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            if (weaponPivot != null)
                weaponPivot.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        UpdateWeaponBaseRotation();

        if (weaponVisual != null)
            weaponVisual.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void DealDamage()
    {
        if (attackOrigin == null)
        {
            Debug.Log("attackOrigin is null");
            return;
        }

        Collider2D[] targets = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange);
        Debug.Log("Found colliders: " + targets.Length);

        foreach (Collider2D target in targets)
        {
            Vector2 toTarget = ((Vector2)target.bounds.center - (Vector2)attackOrigin.position).normalized;
            float angle = Vector2.Angle(lastAttackDirection, toTarget);

            if (angle > attackAngle * 0.5f)
                continue;

            EnemyHealth enemy = target.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                Debug.Log("Damage applied to: " + enemy.name);
                enemy.TakeDamage(attackDamage, lastAttackDirection);
            }
        }
    }
}