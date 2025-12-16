using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [Header("UI")]
    public HealthBar enemyHealthBar;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Combat")]
    [SerializeField] private float damageRate = 1.0f; // Damage every 1 second
    [SerializeField] private int attackDamage = 20;   // How much damage to deal
    [SerializeField] private float knockbackForce = 10f;

    private float nextDamageTime; // Timer for continuous damage
    private bool isHurt = false;  // Stop moving if hurt

    [Header("Components")]
    private Animator anim;
    private Collider2D myCollider;
    private Rigidbody2D rb;

    [Header("AI Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float stopDistance = 1.0f;

    private Transform playerTarget;

    private void Start()
    {
        anim = GetComponent<Animator>();
        myCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTarget = playerObj.transform;

        if (enemyHealthBar != null)
            enemyHealthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (playerTarget == null || currentHealth <= 0 || isHurt) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // Chase Logic
        if (distanceToPlayer < chaseRange && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    private void ChasePlayer()
    {
        // Move towards player
        Vector2 targetPosition = new Vector2(playerTarget.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        anim.SetBool("isWalking", true);

        // Face player
        if (transform.position.x < playerTarget.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Left
        else
            transform.localScale = new Vector3(1, 1, 1); // Right
    }

    // ===================== Taking Damage =====================
    public void TakeDamage(int damageAmount, Transform attacker)
    {
        currentHealth -= damageAmount;

        if (anim != null) anim.SetTrigger("hurt");

        if (enemyHealthBar != null)
        enemyHealthBar.SetHealth(currentHealth);

        if (rb != null && attacker != null)
        {
            ApplyKnockback(attacker);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isHurt = true;
            Invoke("ResetHurtState", 0.5f);
        }

    }

    private void ResetHurtState() => isHurt = false;

    private void ApplyKnockback(Transform attacker)
    {
        Vector2 direction = (transform.position - attacker.position).normalized;
        rb.linearVelocity = Vector2.zero; // Reset old velocity
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    private void Die()
    {
        if (anim != null) anim.SetBool("isDead", true);
        if (myCollider != null) myCollider.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Destroy(gameObject, 2f);
        this.enabled = false;
    }

    // ===================== Dealing Damage (Continuous) =====================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            TryDamage(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            TryDamage(collision.gameObject);
    }

    private void TryDamage(GameObject playerObj)
    {
        // Only damage if the cooldown timer has finished
        if (Time.time >= nextDamageTime)
        {
            Player player = playerObj.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
                nextDamageTime = Time.time + damageRate; // Reset Timer
            }
        }
    }
}