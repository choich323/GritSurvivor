using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 뒤끝 SDK
using BackEnd;

public class ServerManager : MonoBehaviour
{
    [Header ("# Login")]
    public InputField userID;
    public InputField userPW;

    // 로그인 실패 문자
    public GameObject loginFail;

    WaitForSeconds wait = new WaitForSeconds(2f);

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
        // 로그인 에러 끄는 코루틴 중이었으면 종료
        StopCoroutine("OffError");
        BackendLogin.Instance.CustomLogin(userID.text, userPW.text, loginFail);
        // 로그인 오류 메세지 끄기
        StartCoroutine("OffError");
    }

    IEnumerator OffError()
    {
        yield return wait;

        loginFail.SetActive(false);
    }
}
