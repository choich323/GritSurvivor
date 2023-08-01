using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

// 뒤끝 SDK
using BackEnd;

public class ServerManager : MonoBehaviour
{
    [Header ("# Login")]
    public InputField userID;
    public InputField userPW;

    public void Login()
    {
        var test = Backend.Initialize(true); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (test.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + test); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("초기화 실패 : " + test); // 실패일 경우 statusCode 400대 에러 발생 
        }

        BackendLogin.Instance.CustomLogin(userID.text, userPW.text); 
    }
}
