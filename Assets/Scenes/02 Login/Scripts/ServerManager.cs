using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Backend Server SDK
using BackEnd;

public class ServerManager : MonoBehaviour
{
    [Header ("# Login")]
    public InputField userID;
    public InputField userPW;

    // Login Fail UI
    public GameObject loginFail;

    WaitForSeconds wait = new WaitForSeconds(2f);

    bool isInit = false;

    public void Login()
    {
        //  Run until initialization is successful
        if (!isInit)
        {
            var test = Backend.Initialize(true);

            // Response value for initialization
            if (test.IsSuccess())
            {
                Debug.Log("Initialization success : " + test); // statusCode 204 when Success
                isInit = true;
            }
            else
            {
                Debug.LogError("Initialization Failure : " + test); // statusCode 400 series error when Failure
            }
        }

        StopCoroutine("OffError");
        BackendLogin.Instance.CustomLogin(userID.text, userPW.text, loginFail);
        // Turn off login error message
        StartCoroutine("OffError");
    }

    IEnumerator OffError()
    {
        yield return wait;

        loginFail.SetActive(false);
    }
}
