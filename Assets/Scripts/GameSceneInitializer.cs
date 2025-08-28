using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    void Start()
    {
        // When the game scene loads, tell GameManager to start playing
        if (GameManager.Instance != null)
        {
            // Find and assign player references
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GameManager.Instance.playerHealth = player.GetComponent<PlayerHealth>();
                GameManager.Instance.playerMovement = player.GetComponent<PlayerMovement>();
                GameManager.Instance.playerShooting = player.GetComponent<PlayerShooting>();
            }

            GameManager.Instance.ChangeGameState(GameState.Playing);
        }
        else
        {
            Debug.LogError("GameManager not found! Make sure it exists in the MainMenu scene.");
        }
    }
}