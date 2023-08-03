using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BackEnd;
using System.Text;

public class UserData
{
    public Dictionary<string, string> words = new Dictionary<string, string>();

    // Get함수에서 데이터를 디버깅하기 위한 함수.
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

    // 최근 테스트에서 오답이었던 단어 모음을 로드
    public void BookmarkDataGet()
    {
        Debug.Log("게임 정보 조회 함수를 호출합니다.");
        var bro = Backend.GameData.GetMyData("BOOKMARK", new Where());
        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 조회에 성공했습니다. : " + bro);

            MemorizationManager.instance.isLoad = true;
            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json으로 리턴된 데이터를 받아옵니다.

            // 받아온 데이터의 갯수가 0이라면 데이터가 존재하지 않는 것입니다.
            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //불러온 게임정보의 고유값입니다.

                if (userData == null)
                {
                    userData = new UserData();
                }
                else
                {
                    // 이미 데이터가 있는 경우 컨테이너 비우기
                    userData.words.Clear();
                }

                if (!gameDataJson[0].ContainsKey("Last Test"))
                {
                    Debug.Log("Column 데이터가 존재하지 않습니다.");
                    return;
                }

                // 단어 데이터 담기
                foreach (string itemKey in gameDataJson[0]["Last Test"].Keys)
                {
                    userData.words.Add(itemKey, gameDataJson[0]["Last Test"][itemKey].ToString());
                }
            }
        }
        else
        {
            Debug.LogError("게임 정보 조회에 실패했습니다. : " + bro);
        }
    }

    public void BookmarkDataGet(string day)
    {
        Debug.Log("게임 정보 조회 함수를 호출합니다.");
        var bro = Backend.GameData.GetMyData("BOOKMARK", new Where());
        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 조회에 성공했습니다. : " + bro);

            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json으로 리턴된 데이터를 받아옵니다.

            // 받아온 데이터의 갯수가 0이라면 데이터가 존재하지 않는 것입니다.
            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //불러온 게임정보의 고유값입니다.

                if (userData == null)
                {
                    userData = new UserData();
                }
                else
                {
                    // 이미 데이터가 있는 경우 컨테이너 비우기
                    userData.words.Clear();
                }

                if (!gameDataJson[0].ContainsKey(day))
                {
                    Debug.Log("Column 데이터가 존재하지 않습니다.");
                    return;
                }

                // 단어 데이터 담기
                foreach (string itemKey in gameDataJson[0][day].Keys)
                {
                    userData.words.Add(itemKey, gameDataJson[0][day][itemKey].ToString());
                }
            }
        }
        else
        {
            Debug.LogError("게임 정보 조회에 실패했습니다. : " + bro);
        }
    }

    // 유저 데이터에 단어 추가
    public void BookmarkDataAdd(string word, string mean)
    {
        if (userData == null)
        {
            userData = new UserData();
        }

        userData.words.Add(word, mean);
    }

    // DB에 동기화
    public void BookmarkDataUpload(string column)
    {
        if (userData == null)
        {
            userData = new UserData();
        }

        Param param = new Param();
        param.Add(column, userData.words);

        BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.Update("BOOKMARK", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.UpdateV2("BOOKMARK", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("게임정보 데이터 수정에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("게임정보 데이터 수정에 실패했습니다. : " + bro);
        }

        // 파라미터에 추가한 뒤 유저 데이터는 초기화
        userData.words.Clear();
    }
}
