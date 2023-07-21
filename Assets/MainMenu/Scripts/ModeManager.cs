using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeManager : MonoBehaviour
{
    public void EnterGameMode()
    {
        SceneManager.LoadScene("UndeadSurvivor");
    }

    public void EnterMomorizationMode()
    {
        SceneManager.LoadScene("Memorization");
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
