using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BackEnd;

public class UserData
{
    public Dictionary<string, string> words = new Dictionary<string, string>();
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

    // Load words that were incorrect in the last test
    public void BookmarkDataGet()
    {
        Debug.Log("Request Game Data Load");
        var bro = Backend.GameData.GetMyData("BOOKMARK", new Where());
        if (bro.IsSuccess())
        {
            Debug.Log("Game Data Load Success : " + bro);

            MemorizationManager.instance.isLoad = true;
            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Get Data by Json

            // If the number of data received is zero, the data does not exist.
            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("There's no data");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); // Unique value of game data

                if (userData == null)
                {
                    userData = new UserData();
                }
                else
                {
                    // Empty containers if data already exists
                    userData.words.Clear();
                }

                if (!gameDataJson[0].ContainsKey("Last Test"))
                {
                    Debug.Log("Column data has not exist.");
                    return;
                }

                // Put word data
                foreach (string itemKey in gameDataJson[0]["Last Test"].Keys)
                {
                    userData.words.Add(itemKey, gameDataJson[0]["Last Test"][itemKey].ToString());
                }
            }
        }
        else
        {
            Debug.LogError("Game Data Load Failed : " + bro);
        }
    }

    // Add word to UserData
    public void BookmarkDataAdd(string word, string mean)
    {
        if (userData == null)
        {
            userData = new UserData();
        }
        userData.words[word] = mean;
    }

    // Synchronization to DB
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
            Debug.Log("Request modification of the latest game data");

            bro = Backend.GameData.Update("BOOKMARK", new Where(), param);
        }
        else
        {
            Debug.Log($" Request modification of {gameDataRowInDate} data");

            bro = Backend.GameData.UpdateV2("BOOKMARK", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("modification Success : " + bro);
        }
        else
        {
            Debug.LogError("modification fail : " + bro);
        }

        // Empty user data after adding to parameters
        userData.words.Clear();
    }
}
