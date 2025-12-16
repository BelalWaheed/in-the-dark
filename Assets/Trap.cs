using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Trap Settings")]
    public int damageAmount = 10;
    public float knockbackForce = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        

        if (player != null)
        {
            // Deal damage
            player.TakeDamage(damageAmount);

            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            Vector2 force=new Vector2(knockbackDirection.x * 6f, knockbackForce);
            player.ApplyKnockback(force);



        }
    }
}
