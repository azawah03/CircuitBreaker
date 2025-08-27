using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    void Start()
    {
        // When the game scene loads, tell GameManager to start playing
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameState(GameState.Playing);
        }
        else
        {
            Debug.LogError("GameManager not found! Make sure it exists in the MainMenu scene.");
        }
    }
}