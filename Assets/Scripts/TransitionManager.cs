using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static void ToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public static void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public static void ToCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    
    public static void ToQuit()
    {
        Application.Quit();
    }
}
