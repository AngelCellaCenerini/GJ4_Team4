using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour
{
    public void changeTitleScene()
    {
        // Change Scene
        SceneManager.LoadScene("SampleScene");
    }
}
