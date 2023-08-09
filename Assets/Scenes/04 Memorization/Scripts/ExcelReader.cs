using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcelReader : MonoBehaviour
{
    string[] answer;
    int pageIndex = 0;

    public bool isTestMode;

    // 행 데이터 저장할 프리펩 담기
    public RectTransform[] uiLines;

    // csv 데이터를 저장할 컨테이너
    List<Dictionary<string, object>> data;

    void Awake()
    {
        // 초기화
        if (isTestMode)
            data = CSVReader.Read("VOCA(test)");
        else
            data = CSVReader.Read("VOCA(memory)");
    }

    public void Init()
    {
        // 단어 암기 챕터 범위(텍스트)
        string textFront = MemorizationManager.instance.ifFront.text;
        string textBack = MemorizationManager.instance.ifBack.text;

        // 시작 챕터가 빈칸인 경우 예외처리
        if (textFront == "")
        {
            MemorizationManager.instance.uiWarningText.SetActive(true);
            return;
        }
        // 끝 챕터가 빈칸인 경우 자동으로 200으로 설정
        if(textBack == "")
        {
            MemorizationManager.instance.ifBack.text = "200";
            textBack = "200";
        }

        // 암기 범위 데이터 string -> int
        int textFrontNum = int.Parse(textFront);
        int textBackNum = int.Parse(textBack);

        // 암기 챕터의 범위가 잘못된 경우 예외처리
        if (textFrontNum > 200 || textFrontNum <= 0 || textBackNum > 200 || textBackNum <= 0 || textFrontNum > textBackNum)
        {
            MemorizationManager.instance.uiWarningText.SetActive(true);
            return;
        }

        answer = new string[6001];
        for (int i = 0; i < 6001; i++)
            answer[i] = "";
        PageLoad(textFrontNum, textBackNum, 0);

        // UI 전환
        MemorizationManager.instance.EnterMemorization();
    }

    // 페이지 데이터 갱신
    public void PageLoad(int textFrontNum, int textBackNum, int add)
    {
        // 단어 갱신
        // 행 구분 인덱스: 선택한 챕터부터 시작
        pageIndex = (textFrontNum + add - 1) * 30;

        // 가장 뒷페이지로 이동
        if (pageIndex < (textFrontNum - 1) * 30)
        {
            pageIndex = (textBackNum - 1) * 30;
            MemorizationManager.instance.add = textBackNum - textFrontNum;
        }
        // 가장 앞페이지로 이동
        else if (pageIndex >= textBackNum * 30)
        {
            pageIndex = (textFrontNum - 1) * 30;
            MemorizationManager.instance.add = 0;
        }

        if (!isTestMode)
        {
            foreach (RectTransform line in uiLines)
            {
                // 영단어 데이터 로드
                Text word = line.GetChild(1).GetComponent<Text>();
                word.text = data[pageIndex]["word"].ToString();

                // 단어 뜻 데이터 로드
                Text textAnswer = line.GetChild(3).GetChild(2).GetComponent<Text>();
                textAnswer.text = data[pageIndex]["mean"].ToString();

                // 답이 공개되어 있는 경우 가리기
                Toggle toggleAnswerCheck = line.GetChild(3).GetComponent<Toggle>();
                toggleAnswerCheck.isOn = false;

                // 인풋 필드 초기화
                InputField inputData = line.GetChild(2).GetComponent<InputField>();
                inputData.text = "";

                // 다음 단어로
                pageIndex++;
            }
            // 전체 답 공개 상태면 끄기
            MemorizationManager.instance.toggleAnswerCheckAll.isOn = false;

        }
        else
        {
            foreach (RectTransform line in uiLines)
            {
                InputField inputData = line.GetChild(2).GetComponent<InputField>();

                // 현재 데이터를 저장
                if (pageIndex - 30 < 0)
                    answer[(textBackNum - 1) * 30 + pageIndex] = inputData.text;
                else
                    answer[pageIndex - 30] = inputData.text;

                // 새로운 데이터를 로드
                inputData.text = answer[pageIndex];

                // 다음 단어로
                pageIndex++;
            }
        }
        // 페이지 넘버 갱신
        MemorizationManager.instance.uiPageNumber.text = string.Format("{0}", (textFrontNum + MemorizationManager.instance.add).ToString());
    }
}