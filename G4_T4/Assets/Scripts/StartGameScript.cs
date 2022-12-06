using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour
{
    public void PlayGame()
    {
        // Change Scene
        SceneManager.LoadScene("GameScene");
    }
    public void QuitGame()
    {
        // Change Scene
        Debug.Log("Quit");
        Application.Quit();
    }
}
