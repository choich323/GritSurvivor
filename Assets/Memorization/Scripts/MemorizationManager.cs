using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemorizationManager : MonoBehaviour
{
    public static MemorizationManager instance;
    ExcelReader excelReader;

    // btn~ : 버튼, if: 인풋 필드

    [Header("# Select UI")]
    public GameObject scrollViewWordGroup;
    public GameObject uiSelect;
    public GameObject uiWarningText;
    public InputField ifFront;
    public InputField ifBack;

    [Header("# Memorization UI")]
    public GameObject btnReturnSelect;
    public GameObject btnLeft;
    public GameObject btnRight;
    public Text textPageNumber;
    public Toggle toggleAnswerCheckAll;

    // 페이지 변동값 기록
    int add = 0;
    int lineIndex = 0;

    void Awake()
    {
        instance = this;
        excelReader = GetComponent<ExcelReader>();
    }

    // 범위 선택 UI -> 암기 UI
    public void EnterMemorization()
    {
        // 선택창/경고 끄기, 선택창 돌아가기 버튼/스크롤뷰 켜기
        uiSelect.SetActive(false);
        uiWarningText.SetActive(false);
        btnReturnSelect.SetActive(true);
        scrollViewWordGroup.SetActive(true);

        // 한 개 챕터만 선택하는 경우에 페이지 이동 불가
        if (ifFront.text == ifBack.text)
        {
            btnRight.SetActive(false);
        }
        else
        {
            btnRight.SetActive(true);
        }
        btnLeft.SetActive(false);
    }

    // 암기 UI -> 범위 선택 UI
    public void ReturnSelect()
    {
        // 입력란 초기화
        ifFront.text = "";
        ifBack.text = "";
        // 누적값 초기화
        add = 0;
        // 선택창 켜기, 선택창 돌아가기 버튼/스크롤 뷰 끄기
        uiSelect.SetActive(true);
        btnReturnSelect.SetActive(false);
        scrollViewWordGroup.SetActive(false);
    }

    // 메인 모드로 돌아가기
    public void ReturnMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PageChange(bool isRight)
    {
        int textFrontNum = int.Parse(ifFront.text);

        // 오른쪽 버튼이면
        if (isRight)
        {
            excelReader.PageLoad(textFrontNum, ++add);
        }
        else
        {
            excelReader.PageLoad(textFrontNum, --add);
        }

        // 첫 번호, 마지막 번호인 경우 버튼 on/off
        if (textFrontNum + add == int.Parse(ifBack.text))
        {
            btnRight.SetActive(false);
            btnLeft.SetActive(true);
        }
        else if (add < 1)
        {
            btnLeft.SetActive(false);
            btnRight.SetActive(true);
        }
        else
        {
            btnLeft.SetActive(true);
            btnRight.SetActive(true);
        }
    }

    public void AnswerOpenAll(Toggle toggle)
    {
        foreach (RectTransform line in excelReader.uiLines)
        {
            line.GetChild(3).GetChild(1).gameObject.SetActive(!toggle.isOn); // toggle의 터치 안내 텍스트
            line.GetChild(3).GetChild(2).gameObject.SetActive(toggle.isOn);  // toggle의 정답 텍스트
        }
    }

    public void AnswerIndex(int index)
    {
        lineIndex = index;   
    }

    public void AnswerOpen(Toggle toggle)
    {
        // 인덱스를 통해 몇번째 라인인지 체크하고 해당 라인에 대해서만 변화를 적용
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(1).gameObject.SetActive(!toggle.isOn);
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(2).gameObject.SetActive(toggle.isOn);
    }
}
