using UnityEngine;
using UnityEngine.SceneManagement; // Needed to reload the scene

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // Drag your UI Panel here

    private void Awake()
    {
        // Ensure the panel is hidden when the game starts
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // Call this when the player dies
    public void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // Optional: Pause the game
            // Time.timeScale = 0f; 
        }
    }

    // Link this function to your "Retry" Button
    public void RestartGame()
    {
        // Unpause if you paused it
        // Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}