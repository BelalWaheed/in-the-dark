using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

        if (player == null)
        {
            Debug.LogError("Player component not found in parent!");
        }
    }

    // Called from Animation Event
    public void DisableMovementAndJump()
    {
        if (player == null) return;
        player.EnableMovementAndJump(false);
    }

    // Called from Animation Event
    public void EnableMovementAndJump()
    {
        if (player == null) return;
        player.EnableMovementAndJump(true);
    }
    public void DamegeEnemies() => player.DamegeEnemies();
}
