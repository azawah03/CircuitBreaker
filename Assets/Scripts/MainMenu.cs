using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetPlayerData();
            GameManager.Instance.gameTimer = 0f;
            GameManager.Instance.LoadScene("NeonArena");
        }
        else
        {
            SceneManager.LoadScene("NeonArena");
        }
    }

    public void QuitGame()
    {
        // Use GameManager if available for proper cleanup
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
            Application.Quit();
        }
    }
}
