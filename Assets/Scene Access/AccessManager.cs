using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccessManager : MonoBehaviour
{
    public GameObject wrongCode;

    public InputField code;

    private string accessCode = "@grit0101";

    public void Access()
    {
        if (code.text == accessCode)
        {
            SceneManager.LoadScene("Login");
        }
        else
        {
            wrongCode.SetActive(true);
        }
    }
}
