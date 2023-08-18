using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using BackEnd;

public class BackendLogin
{
    private static BackendLogin _instance = null;

    public static BackendLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendLogin();
            }

            return _instance;
        }
    }

    public void CustomLogin(string id, string pw, GameObject fail)
    {
        Debug.Log("Login Request");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("Login Success : " + bro);
            SceneManager.LoadScene("03 MainMenu");
        }
        else
        {
            Debug.LogError("Login Failed : " + bro);
            fail.SetActive(true);
        }
    }
}
