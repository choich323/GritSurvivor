using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

// �ڳ� SDK
using BackEnd;

public class ServerManager : MonoBehaviour
{
    // ������ ������ ������ scene������ �� �� �ֵ��� �ν��Ͻ�ȭ
    public static ServerManager instance;

    [Header ("# Login")]
    public InputField userID;
    public InputField userPW;

    void Awake()
    {
        instance = this;
    }

    void Start()
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
    }

    public void Login()
    {
        BackendLogin.Instance.CustomLogin(userID.text, userPW.text); 
    }
}
