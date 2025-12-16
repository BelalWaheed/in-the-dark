using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("UI")]
    public HealthBar healthBar;

    // ===================== Health & Death =====================
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    // Invincibility Logic
    private float nextVulnerableTime;
    private float invincibilityDuration = 1.0f;

    private GameOverUI gameManager;

    // ===================== Combat =====================
    [Header("Combat")]
    public Collider2D[] enemyColliders;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private int attackDamage = 20;

    // ===================== Components =====================
    private Rigidbody2D rb;
    private Animator anim;

    // ===================== Movement =====================
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    private bool facingRight = true;
    private bool canMove = true;

    // ===================== Jump =====================
    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance = 1.75f;

    private bool isGrounded;
    private bool canJump = true;

    private void Awake()
    {
        whatIsGround = LayerMask.GetMask("Ground");
        gameManager = Object.FindFirstObjectByType<GameOverUI>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (isDead) return;

        CheckGround();
        HandleMovement();
        HandleFlip();
        HandleAnimation();
        HandleInput();
    }

    // ===================== Health Logic (THE FIX IS HERE) =====================
    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < nextVulnerableTime) return;

        currentHealth -= damage;
        nextVulnerableTime = Time.time + invincibilityDuration;

        anim.SetTrigger("hurt");

        // *** CRITICAL FIX: UNLOCK MOVEMENT ***
        // Even if we were mid-attack, we must reset controls now.
        EnableMovementAndJump(true);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; // Disable physics so enemy stops pushing corpse

        if (gameManager != null)
            gameManager.TriggerGameOver();
    }

    // ===================== Attack Logic =====================
    public void DamegeEnemies()
    {
        enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);
        foreach (Collider2D enemy in enemyColliders)
        {
            Enemy1 enemyScript = enemy.GetComponent<Enemy1>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage, transform);
            }
        }
    }

    // ===================== Input & Movement =====================
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.J)) TryAttack();
    }

    private void HandleMovement()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        if (canMove)
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (!canJump || !isGrounded) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void TryAttack()
    {
        if (!isGrounded) return;

        // Stop moving while attacking
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetTrigger("attack");
    }

    private void HandleAnimation()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void HandleFlip()
    {
        if (rb.linearVelocity.x > 0 && !facingRight) Flip();
        else if (rb.linearVelocity.x < 0 && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void EnableMovementAndJump(bool enable)
    {
        canMove = enable;
        canJump = enable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}