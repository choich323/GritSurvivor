using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeManager : MonoBehaviour
{
    public void EnterGameMode()
    {
        SceneManager.LoadScene("05 UndeadSurvivor");
    }

    public void EnterMemorizationMode()
    {
        SceneManager.LoadScene("04 Memorization");
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
