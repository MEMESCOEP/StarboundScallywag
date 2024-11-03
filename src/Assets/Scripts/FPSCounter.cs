using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Assign a UI Text element in the inspector
    private float deltaTime = 0.0f;

    void Update()
    {
        // Calculate the time that has passed since the last frame
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // Calculate FPS
        float fps = 1.0f / deltaTime;

        // Update the UI text with the FPS value
        fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
    }
}
