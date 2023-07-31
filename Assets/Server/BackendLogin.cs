using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 뒤끝 SDK
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

    public void CustomLogin(string id, string pw)
    {
        // 로그인 구현하기 로직
        Debug.Log("로그인을 요청합니다.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인에 성공했습니다. : " + bro);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogError("로그인에 실패했습니다. : " + bro);
            Debug.LogError("잘못된 사용자 아이디 혹은 비밀번호 입니다.");
        }
    }

    public void CustomSignUp(string id, string pw)
    {
        // 회원가입 구현하기 로직
        // 실제 앱에서는 쓰지 않고 관리자가 계정을 직접 생성할 것으로 예상되지만, 일단 작성
        Debug.Log("회원가입을 요청합니다.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("회원가입에 실패했습니다. : " + bro);
        }
    }

    public void UpdateNickname(string nickname)
    {
        // 닉네임 변경 구현하기 로직
        // 마찬가지로 실제 앱에서는 쓰지 않고 관리자가 닉네임을 직접 생성할 것으로 예상되지만, 일단 작성
        Debug.Log("닉네임 변경을 요청합니다.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경에 성공했습니다 : " + bro);
        }
        else
        {
            Debug.LogError("닉네임 변경에 실패했습니다 : " + bro);
        }
    }
}
