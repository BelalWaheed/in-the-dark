using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient gradient;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f; // Higher = Faster slide

    private float targetHealth; // Where we want the bar to go

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        targetHealth = health; // Start full

        if (fillImage != null)
            fillImage.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        // Don't snap instantly. Just set the "Target" destination.
        targetHealth = health;
    }

    private void Update()
    {
        // Smoothly move the slider value towards the target health
        // Mathf.Lerp(current, target, speed) creates a smooth slide
        if (slider.value != targetHealth)
        {
            slider.value = Mathf.Lerp(slider.value, targetHealth, smoothSpeed * Time.deltaTime);

            // Update color based on the CURRENT slider position (changes as it slides)
            if (fillImage != null)
                fillImage.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}