using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <-- ADDED: For the Text Display

public class LevelManager : MonoBehaviour
{
    // The Singleton Instance
    public static LevelManager instance;

    // UI Reference
    public TextMeshProUGUI killCounterDisplay; // <-- ADDED: To hold the Text component

    [Header("Level Settings")]
    [SerializeField] private int enemiesToKill = 3; // Goal
    [SerializeField] private string nextSceneName = "World2";

    // Tracking
    private int currentKills = 0;
    private bool levelUnlocked = false; // <-- Kept this, just in case

    private void Awake()
    {
        // Singleton initialization
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // Ensures only one manager exists
    }

    private void Start()
    {
        // Initial setup for the kill display
        UpdateKillDisplay();
    }

    private void Update()
    {
        // Logic for pressing 'E' (from your previous script)
        if (currentKills >= enemiesToKill && !levelUnlocked)
        {
            // First time reaching the goal
            levelUnlocked = true;
            // Optional: Visually confirm unlock to the player (e.g., text color change)
        }

        if (levelUnlocked && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextLevel();
        }
    }

    // Called by Enemy when it dies
    public void AddKill()
    {
        currentKills++;

        // --- CRUCIAL: Call the update function immediately after the kill count changes!
        UpdateKillDisplay();

        Debug.Log("Enemies Killed: " + currentKills + " / " + enemiesToKill);

        if (currentKills >= enemiesToKill)
        {
            // Now handled in Update(), but we leave the logic here too for confirmation
        }
    }

    // FUNCTION TO UPDATE THE TEXT
    private void UpdateKillDisplay()
    {
        if (killCounterDisplay != null)
        {
            // The logic is: "Kills: [current]/[required]"
            killCounterDisplay.text = "Kills: " + currentKills + " / " + enemiesToKill;

            // Optional: Highlight the text when the goal is met
            if (currentKills >= enemiesToKill)
            {
                // 
                killCounterDisplay.color = Color.green; // Set text color to green
            }
        }
    }


    private void LoadNextLevel()
    {
        Debug.Log("Level Complete! Loading World 2...");
        SceneManager.LoadScene(nextSceneName);
    }
}