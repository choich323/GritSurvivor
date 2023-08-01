using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BackEnd;
using System.Text;

public class UserData
{
    public Dictionary<string, string> words = new Dictionary<string, string>();

    // Get�Լ����� �����͸� ������ϱ� ���� �Լ�.
    public override string ToString()
    {
        StringBuilder result = new StringBuilder();

        foreach (var itemKey in words.Keys)
        {
            result.AppendLine($"{itemKey} : {words[itemKey]}");
        }

        return result.ToString();
    }
}

public class BackendGameData {

    private static BackendGameData _instance = null;

    public static BackendGameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendGameData();
            }

            return _instance;
        }
    }

    public static UserData userData;

    private string gameDataRowInDate = string.Empty;

    // Get �Լ� ����. ������ ���α׷��� �°� ���� �ʿ�.
    public void BookmarkDataGet(string wordNumber)
    {
        Debug.Log("���� ���� ��ȸ �Լ��� ȣ���մϴ�.");
        var bro = Backend.GameData.GetMyData("BOOKMARK", new Where());
        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ��ȸ�� �����߽��ϴ�. : " + bro);


            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json���� ���ϵ� �����͸� �޾ƿɴϴ�.

            // �޾ƿ� �������� ������ 0�̶�� �����Ͱ� �������� �ʴ� ���Դϴ�.
            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�.");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //�ҷ��� ���������� �������Դϴ�.

                userData = new UserData();

                foreach (string itemKey in gameDataJson[0][wordNumber].Keys)
                {
                    userData.words.Add(itemKey, gameDataJson[0][wordNumber][itemKey].ToString());
                }

                Debug.Log(userData.ToString());
            }
        }
        else
        {
            Debug.LogError("���� ���� ��ȸ�� �����߽��ϴ�. : " + bro);
        }
    }

    // ���� �����Ϳ� �ܾ� �߰�
    public void BookmarkDataAdd(string word, string mean)
    {
        if (userData == null)
        {
            userData = new UserData();
        }

        userData.words.Add(word, mean);
    }

    // DB�� ����ȭ
    public void BookmarkDataUpload(string day)
    {
        if (userData == null)
        {
            userData = new UserData();
        }

        Param param = new Param();
        param.Add(day, userData.words);

        BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("�� ���� �ֽ� �������� ������ ������ ��û�մϴ�.");

            bro = Backend.GameData.Update("BOOKMARK", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}�� �������� ������ ������ ��û�մϴ�.");

            bro = Backend.GameData.UpdateV2("BOOKMARK", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("�������� ������ ������ �����߽��ϴ�. : " + bro);
        }
        else
        {
            Debug.LogError("�������� ������ ������ �����߽��ϴ�. : " + bro);
        }

        // �Ķ���Ϳ� �߰��� �� ���� �����ʹ� �ʱ�ȭ
        userData.words.Clear();
    }
}
