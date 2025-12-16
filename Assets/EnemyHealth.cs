using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public void Die()
    {
        // Using LevelManager instead of SceneController
        LevelManager manager = Object.FindFirstObjectByType<LevelManager>();

        if (manager != null)
        {
            manager.AddKill();
        }
        else
        {
            Debug.LogWarning("LevelManager not found! Cannot report kill.");
        }

        Destroy(gameObject);
    }
}