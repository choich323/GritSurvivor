using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

// 뒤끝 SDK
using BackEnd;
using System.IO;

public class ServerManager : MonoBehaviour
{
    [Header ("# Login")]
    public InputField userID;
    public InputField userPW;

    bool isInit = false;

    public void Login()
    {
        // 초기화가 성공할 때까지만 실행
        if (!isInit)
        {
            var test = Backend.Initialize(true); // 뒤끝 초기화

            // 뒤끝 초기화에 대한 응답값
            if (test.IsSuccess())
            {
                Debug.Log("초기화 성공 : " + test); // 성공일 경우 statusCode 204 Success
                isInit = true;
            }
            else
            {
                Debug.LogError("초기화 실패 : " + test); // 실패일 경우 statusCode 400대 에러 발생
            }
        }
        BackendLogin.Instance.CustomLogin(userID.text, userPW.text); 
    }
}
