using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

// �ڳ� SDK
using BackEnd;

public class ServerManager : MonoBehaviour
{
    [Header ("# Login")]
    public InputField userID;
    public InputField userPW;

    public void Login()
    {
        var test = Backend.Initialize(true); // �ڳ� �ʱ�ȭ

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (test.IsSuccess())
        {
            Debug.Log("�ʱ�ȭ ���� : " + test); // ������ ��� statusCode 204 Success
        }
        else
        {
            Debug.LogError("�ʱ�ȭ ���� : " + test); // ������ ��� statusCode 400�� ���� �߻� 
        }

        BackendLogin.Instance.CustomLogin(userID.text, userPW.text); 
    }
}
