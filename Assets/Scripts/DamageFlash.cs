using UnityEngine;
using UnityEngine.UI; // Needed for Image
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image flashImage; // The red panel

    [Header("Settings")]
    [Tooltip("How red the screen gets (0 to 1)")]
    [SerializeField] private float maxAlpha = 0.4f;
    [Tooltip("How fast the red fades away")]
    [SerializeField] private float fadeSpeed = 5f;

    private Color flashColor;

    private void Awake()
    {
        // Ensure the image starts invisible (alpha 0)
        if (flashImage != null)
        {
            flashColor = flashImage.color;
            flashColor.a = 0f;
            flashImage.color = flashColor;
        }
        else
        {
            Debug.LogError("DamageFlash: No Image linked!");
        }
    }

    // Call this method to start the effect
    public void TriggerFlash()
    {
        if (flashImage == null) return;

        // Stop previous flash if running to avoid glitches
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Instantly turn red
        flashColor.a = maxAlpha;
        flashImage.color = flashColor;

        // 2. Wait a tiny frame
        yield return null;

        // 3. Smoothly fade out back to invisible
        while (flashImage.color.a > 0.01f)
        {
            flashColor.a = Mathf.Lerp(flashImage.color.a, 0f, fadeSpeed * Time.deltaTime);
            flashImage.color = flashColor;
            // Wait until next frame
            yield return null;
        }

        // Ensure it's completely invisible at the end
        flashColor.a = 0f;
        flashImage.color = flashColor;
    }
}