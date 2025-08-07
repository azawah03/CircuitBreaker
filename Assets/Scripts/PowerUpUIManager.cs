using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PowerUpUIManager : MonoBehaviour
{
    public TextMeshProUGUI notificationText;
    public float displayDuration = 2f;

    // Singleton for easy access
    public static PowerUpUIManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    public void ShowPowerUpNotification(string powerUpName)
    {
        if (notificationText == null) return;

        // Set message based on power-up name
        string message = "";
        switch (powerUpName)
        {
            case "SpeedBoost":
                message = "SPEED BOOST ACTIVATED!";
                notificationText.color = Color.yellow;
                break;
            case "RapidFire":
                message = "RAPID FIRE ACTIVATED!";
                notificationText.color = Color.red;
                break;
            case "ShieldPowerup":
                message = "SHIELD ACTIVATED!";
                notificationText.color = Color.cyan;
                break;
            default:
                message = "POWER-UP ACTIVATED!";
                notificationText.color = Color.white;
                break;
        }

        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        notificationText.gameObject.SetActive(false);
    }
}