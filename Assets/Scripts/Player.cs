using UnityEngine;

public class Player : MonoBehaviour
{
    // ===================== UI & References =====================
    [Header("UI")]
    public HealthBar healthBar;
    private DamageFlash damageFlash; // NEW reference
    private GameOverUI gameManager;

    // ===================== Health =====================
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    // Invincibility (Safety Time)
    private float nextVulnerableTime;
    private float invincibilityDuration = 0.4f;

    // ===================== Combat =====================
    [Header("Combat")]
    public Collider2D[] enemyColliders;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private int attackDamage = 40;

    [Header("Combat Settings")]
    [SerializeField] private float knockbackForce = 15f; // Strong push
    [SerializeField] private float knockbackDuration = 0.2f; // How long controls are locked

    // ===================== Components =====================
    private Rigidbody2D rb;
    private Animator anim;

    // ===================== Movement & Jump =====================
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 14f;
    [SerializeField] private float jumpForce = 25f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance = 1.75f;

    // State Variables
    private bool facingRight = true;
    private bool canMove = true;
    private bool canJump = true;
    private bool isGrounded;

    // ===================== Unity Methods =====================
    private void Awake()
    {
        whatIsGround = LayerMask.GetMask("Ground");
        // Automatically find the Game Over UI manager
        gameManager = Object.FindFirstObjectByType<GameOverUI>();
        // NEW: Automatically find the flash script in the scene
        damageFlash = Object.FindFirstObjectByType<DamageFlash>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        // Initialize Health
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (isDead) return; // Stop everything if dead

        CheckGround();
        HandleMovement();
        HandleFlip();
        HandleAnimation();
        HandleInput();
    }

    // ===================== Health Logic =====================
    public void TakeDamage(int damage, Transform damageSource)
    {
        // 1. Ignore damage if dead or invincible
        if (isDead || Time.time < nextVulnerableTime) return;

        // 2. Apply Damage
        currentHealth -= damage;
        nextVulnerableTime = Time.time + invincibilityDuration;

        AudioManager.instance.PlaySFX("hurt");

        // 3. Update Visuals (UI & Animation)

        if (damageFlash != null)
            damageFlash.TriggerFlash();

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);


        anim.SetTrigger("hurt");

        // 4. CRITICAL FIX: Unlock movement immediately
        // This prevents getting stuck if you were hit mid-attack
        EnableMovementAndJump(true);

        if (damageSource != null)
        {
            StartCoroutine(KnockbackRoutine(damageSource));
        }
        else
        {
            // If no source (e.g. poison), just unlock immediately
            EnableMovementAndJump(true);
        }
        // 5. Check Death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    // CHANGE 3: The Routine that handles the push
    private System.Collections.IEnumerator KnockbackRoutine(Transform damageSource)
    {
        // 1. Disable input so physics can work
        canMove = false;

        // 2. Calculate direction: Away from enemy
        Vector2 direction = (transform.position - damageSource.position).normalized;

        // 3. Reset velocity and add force (Add a little Y lift for a hop)
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction.x * knockbackForce, knockbackForce * 0.5f), ForceMode2D.Impulse);

        // 4. Wait for the push to finish
        yield return new WaitForSeconds(knockbackDuration);

        // 5. Give control back
        canMove = true;
    }

    private void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);

        // Stop Physics
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        AudioManager.instance.PlaySFX("die");
        // Trigger Game Over Screen
        if (gameManager != null)
            gameManager.TriggerGameOver();
    }

    // ===================== Combat Logic =====================
    // Note: Kept spelling 'Damege' to match your Animation Event
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

    // ===================== Input & Control =====================
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.J)) TryAttack();
    }

    private void TryAttack()
    {
        if (!isGrounded) return;

        // Stop moving to attack
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetTrigger("attack");
        AudioManager.instance.PlaySFX("attack");
    }

    private void Jump()
    {
        if (!canJump || !isGrounded) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // 3. The Sound (MUST be after the check!)
        AudioManager.instance.PlaySFX("jump");
    }

    // ===================== Movement Implementation =====================
    private void HandleMovement()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        if (canMove)
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
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

    // Called by Animation Events
    public void EnableMovementAndJump(bool enable)
    {
        canMove = enable;
        canJump = enable;
    }

    // ===================== Debugging =====================
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}