using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryButton : MonoBehaviour
{
    public void MainMenu()
    {
        Time.timeScale = 1f; // Reset time
        SceneManager.LoadScene("MainMenu");
    }
}