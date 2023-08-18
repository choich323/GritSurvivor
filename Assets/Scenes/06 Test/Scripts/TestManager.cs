using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class TestManager : MonoBehaviour
{
    public RectTransform[] uiTestData;
    public GameObject uiSubmitCheck;
    public GameObject uiScoreResult;
    public GameObject uiTestDataBackground;
    ExcelReader excelReader;

    // test score
    float score;

    void Awake()
    {
        excelReader = GetComponent<ExcelReader>();
        if (!PlayerPrefs.HasKey("lastTest"))
        {
            Init();
        }
    }

    // Initialize locally stored data
    public void Init()
    {
        PlayerPrefs.SetInt("lastTest", 0);
        for(int index = 0; index < 10; index++)
        {
            PlayerPrefs.SetString("testSlot" + index.ToString(), "");
        }
    }

    public void Submit()
    {
        uiSubmitCheck.SetActive(true);
    }

    public void Cancel()
    {
        uiSubmitCheck.SetActive(false);
    }

    public void Scoring()
    {
        int textFrontNum = int.Parse(MemorizationManager.instance.ifFront.text);
        int textBackNum = int.Parse(MemorizationManager.instance.ifBack.text);
        int keyIndex = 0;
        // The total number of questions
        float max = (textBackNum - textFrontNum + 1) * 30;
        // The number of correct answers
        float correct = 0;

        // Save the current page data
        int pageIndex = (int.Parse(MemorizationManager.instance.uiPageNumber.text) - 1) * 30;
        foreach(RectTransform line in excelReader.uiLines)
        {
            InputField inputData = line.GetChild(2).GetComponent<InputField>();
            excelReader.answer[pageIndex] = inputData.text;
            pageIndex++;
        }
        // Scoring
        for(int index = (textFrontNum - 1) * 30; index < textBackNum * 30; index++)
        {
            // Meaning data
            string means = excelReader.randWords[excelReader.keys[keyIndex]][1];
            bool right = false;
            // When multiple meanings
            foreach (string mean in means.Split('.'))
            {
                // When user inputs multiple answers
                foreach (string ans in excelReader.answer[index].Split(','))
                {
                    // Ignore spaces
                    string tmp = "";
                    foreach(char spell in ans)
                    {
                        if(spell != ' ')
                            tmp += spell.ToString();
                    }
                    if (mean == tmp)
                    {
                        right = true;
                        break;
                    }
                }
                if (right) break;
            }
            if (right) correct++;
            else
            {
                // save incorrect answers
                BackendGameData.Instance.BookmarkDataAdd(excelReader.randWords[excelReader.keys[keyIndex]][0], excelReader.randWords[excelReader.keys[keyIndex]][2]);
            }
            keyIndex++;
        }

        // Scoring: up to the second decimal place
        score = Mathf.Round(((float)correct / max) * 10000f) / 100f;

        // Save at local
        // current time
        string testLog = DateTime.Now.ToString() + "\n" + MemorizationManager.instance.ifFront.text + "~" + MemorizationManager.instance.ifBack.text + "_" + correct.ToString() + "/" + max.ToString() + "_" + score.ToString() + "점";
        // save slot number
        int slotNum = (PlayerPrefs.GetInt("lastTest") + 1) % 10;       
        // save test history at slot
        PlayerPrefs.SetString("testSlot" + slotNum.ToString(), testLog);

        PlayerPrefs.SetInt("lastTest", slotNum);

        if(score >= 60f)
        {
            // Save incorrect answers to DB
            BackendGameData.Instance.BookmarkDataUpload("Last Test");
            // Save log to server based on test result
            if (score >= 90f)
            {
                InsertLog(testLog);
            }
        }
        BackendGameData.userData.words.Clear();

        // Turn off the submission UI and turn on the score verification UI
        uiSubmitCheck.SetActive(false);
        uiScoreResult.SetActive(true);
        Text scoreResult = uiScoreResult.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>();
        // Test result
        scoreResult.text = "시험 결과: " + correct + " / " + max + string.Format("\n백분율: {0:F2}", score);
    }

    // After checking the test results, finish the test and return to the range selection UI
    public void ExitTest()
    {
        uiScoreResult.SetActive(false);
        MemorizationManager.instance.ReturnSelect();
    }

    // Save test data log
    public void InsertLog(string testLog)
    {
        Debug.Log("Insert Log");

        Param param = new Param();
        param.Add("시험 날짜/시험 범위_시험 결과_시험 점수", testLog);

        // user nickname, test history, history expiration date
        var bro = Backend.GameLog.InsertLog("TestPass", param, 10);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("Insert Error : " + bro);
            return;
        }

        Debug.Log("Insert Success : " + bro);
    }

    // Check test data from local storage
    public void TestDataToggle(Toggle testData)
    {
        uiTestDataBackground.SetActive(testData.isOn);
        if(testData.isOn == true)
        {
            TestDataLoad();
        }
    }

    public void TestDataLoad()
    {
        int slotNum = PlayerPrefs.GetInt("lastTest");
        foreach(RectTransform testData in uiTestData)
        {
            Text data = testData.GetChild(0).GetComponent<Text>();
            data.text = PlayerPrefs.GetString("testSlot" + slotNum.ToString());
            slotNum--;
            if(slotNum < 0)
            {
                slotNum = 9;
            }
        }
    }
}
