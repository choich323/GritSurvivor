using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    public GameObject uiSubmitCheck;
    ExcelReader excelReader;

    // 시험 점수
    float score;

    void Awake()
    {
        excelReader = GetComponent<ExcelReader>();
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
        int max = (textBackNum - textFrontNum + 1) * 30;
        // 정답수
        int correct = 0;

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
            string means = excelReader.randWords[excelReader.keys[keyIndex++]][1];
            bool right = false;
            foreach(string mean in means.Split('.'))
            {
                foreach (string ans in excelReader.answer[index].Split(','))
                {
                    if (mean == ans)
                    {
                        right = true;
                        break;
                    }
                }
                if (right) break;
            }
            if (right) correct++;
        }

        Debug.Log(correct);
        Debug.Log(max);

        // 점수에 따른 합/불
        if(score >= 90.0f)
        {
            SaveTest();
        }
        else
        {

        }
    }

    public void SaveTest()
    {

    }
}
