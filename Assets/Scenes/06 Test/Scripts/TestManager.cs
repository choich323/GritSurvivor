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

    // 시험 점수
    float score;

    void Awake()
    {
        excelReader = GetComponent<ExcelReader>();
        if (!PlayerPrefs.HasKey("lastTest"))
        {
            Init();
        }
    }

    // 로컬 저장 데이터 초기화
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
        // 문제수
        float max = (textBackNum - textFrontNum + 1) * 30;
        // 정답수
        float correct = 0;

        // 현재 페이지 데이터 저장
        int pageIndex = (int.Parse(MemorizationManager.instance.uiPageNumber.text) - 1) * 30;
        foreach(RectTransform line in excelReader.uiLines)
        {
            InputField inputData = line.GetChild(2).GetComponent<InputField>();
            excelReader.answer[pageIndex] = inputData.text;
            pageIndex++;
        }
        // 채점
        for(int index = (textFrontNum - 1) * 30; index < textBackNum * 30; index++)
        {
            // 뜻
            string means = excelReader.randWords[excelReader.keys[keyIndex]][1];
            bool right = false;
            // 뜻이 여러 개인 경우
            foreach(string mean in means.Split('.'))
            {
                // 유저가 작성한 뜻이 여러 개인 경우
                foreach (string ans in excelReader.answer[index].Split(','))
                {
                    // 띄어쓰기 있는 경우 무시
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
            // 정답이면
            if (right) correct++;
            else
            {
                BackendGameData.Instance.BookmarkDataAdd(excelReader.randWords[excelReader.keys[keyIndex]][0], excelReader.randWords[excelReader.keys[keyIndex]][2]);
            }
            keyIndex++;
        }

        // 점수 계산: 소수점 2번째 자리까지
        score = Mathf.Round(((float)correct / max) * 10000f) / 100f;

        // 로컬에 저장
        // 현재 시간
        string testLog = DateTime.Now.ToString() + "\n" + MemorizationManager.instance.ifFront.text + "~" + MemorizationManager.instance.ifBack.text + "_" + correct.ToString() + "/" + max.ToString() + "_" + score.ToString() + "점";
        // 세이브 슬롯 넘버
        int slotNum = (PlayerPrefs.GetInt("lastTest") + 1) % 10;       
        // 지정 슬롯에 테스트 기록 저장
        PlayerPrefs.SetString("testSlot" + slotNum.ToString(), testLog);

        PlayerPrefs.SetInt("lastTest", slotNum);

        if(score >= 60f)
        {
            // 오답 북마크에 기록
            BackendGameData.Instance.BookmarkDataUpload("Last Test");
            // 합격 여부에 따라 서버에 로그 기록
            if (score >= 90f)
            {
                InsertLog(testLog);
            }
        }
        BackendGameData.userData.words.Clear();

        // 제출확인 창을 끄고 점수 확인란을 켜기
        uiSubmitCheck.SetActive(false);
        uiScoreResult.SetActive(true);
        Text scoreResult = uiScoreResult.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>();
        // 소수점 둘째자리까지 확인
        scoreResult.text = "시험 결과: " + correct + " / " + max + string.Format("\n백분율: {0:F2}", score);
    }

    // 시험 결과 확인후 시험 종료하고 범위 선택화면으로 돌아가는 함수
    public void ExitTest()
    {
        uiScoreResult.SetActive(false);
        MemorizationManager.instance.ReturnSelect();
    }

    // 시험 데이터를 로그로 저장하는 함수
    public void InsertLog(string testLog)
    {
        Debug.Log("로그 삽입을 시도합니다.");

        Param param = new Param();
        param.Add("시험 날짜/시험 범위_시험 결과_시험 점수", testLog);

        // 닉네임과 테스트 기록, 기록 유효기간
        var bro = Backend.GameLog.InsertLog("TestPass", param, 10);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("로그 삽입 중 에러가 발생했습니다. : " + bro);
            return;
        }

        Debug.Log("로그 삽입에 성공했습니다. : " + bro);
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
