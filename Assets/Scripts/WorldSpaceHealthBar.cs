using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(PlayerHealth))]
public class WorldSpaceHealthBar : MonoBehaviour
{
    private Canvas canvas;
    private Slider slider;
    private RectTransform sliderTransform;
    private PlayerHealth playerHealth;

    public Vector3 offset = new Vector3(0, 2.5f, 0); // height above head

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

        // Create Canvas
        GameObject canvasObj = new GameObject("HealthBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        CanvasScaler cs = canvasObj.AddComponent<CanvasScaler>();
        cs.dynamicPixelsPerUnit = 10;
        canvasObj.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2f, 0.3f);
        canvasRect.localPosition = offset;

        // Create Slider
        GameObject sliderObj = new GameObject("HealthSlider");
        sliderObj.transform.SetParent(canvas.transform);
        sliderTransform = sliderObj.AddComponent<RectTransform>();
        slider = sliderObj.AddComponent<Slider>();
        sliderTransform.sizeDelta = new Vector2(2f, 0.3f);
        sliderTransform.localPosition = Vector3.zero;

        // Slider visual
        slider.minValue = 0f;
        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.maxHealth;
        slider.interactable = false;
        slider.direction = Slider.Direction.LeftToRight;

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(slider.transform);
        UnityEngine.UI.Image bgImage = background.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = Color.black;
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill
        GameObject fillArea = new GameObject("Fill");
        fillArea.transform.SetParent(slider.transform);
        UnityEngine.UI.Image fillImage = fillArea.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = Color.red;
        slider.fillRect = fillImage.rectTransform;

        RectTransform fillRect = fillImage.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // Subscribe to health change
        playerHealth.onHealthChanged += UpdateHealthBar;
    }

    void UpdateHealthBar(float current, float max)
    {
        slider.value = current;
    }

    void LateUpdate()
    {
        // Always face camera
        if (Camera.main != null)
        {
            canvas.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
