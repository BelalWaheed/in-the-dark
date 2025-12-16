using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider; // Reference to the UI Slider
    [SerializeField] private Gradient gradient; // To change color (Green -> Red)
    [SerializeField] private Image fillImage;   // The colored part of the slider

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        // Set color to the top of the gradient (Green)
        if (fillImage != null)
            fillImage.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        // Change color based on percentage left (Green -> Yellow -> Red)
        if (fillImage != null)
            fillImage.color = gradient.Evaluate(slider.normalizedValue);
    }
}