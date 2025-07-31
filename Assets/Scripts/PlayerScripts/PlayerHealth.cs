using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    private bool isGameOver = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentHealth = 0f; // later on we can make it gradually decrease depending on the enemy
            isGameOver = true;
            Time.timeScale = 0f;
        }
    }

    void OnGUI()
    {
        if (isGameOver)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 60;
            style.normal.textColor = Color.red;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 30, 400, 100);
            GUI.Label(rect, "YOU LOST", style);
        }
    }
}
