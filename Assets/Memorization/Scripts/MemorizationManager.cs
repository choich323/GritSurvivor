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
    public Text uiPageNumber;
    public Toggle toggleAnswerCheckAll;

    [Header("# Warning UI")]
    public GameObject uiWarning;
    public GameObject uiPageMoveWarning;
    public GameObject uiReturnSelectWarning;

    // 페이지 변동값 기록
    int add = 0;
    int lineIndex = 0;
    bool isRight;

    void Awake()
    {
        instance = this;
        excelReader = GetComponent<ExcelReader>();
        // 유저의 아이디를 제대로 식별하는지 확인(임시 코드)
        Debug.Log(ServerManager.instance.userID.text);
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
        // 선택창 켜기, 선택창 돌아가기 버튼/스크롤 뷰, 경고창 끄기
        uiSelect.SetActive(true);
        btnReturnSelect.SetActive(false);
        scrollViewWordGroup.SetActive(false);
        uiWarning.SetActive(false);
        uiReturnSelectWarning.SetActive(false);
    }

    // 메인 모드로 돌아가기
    public void ReturnMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PageChange()
    {
        int textFrontNum = int.Parse(ifFront.text);

        // 경고창 끄기
        uiWarning.SetActive(false);
        uiPageMoveWarning.SetActive(false);

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

    public void OpenAllAnswer(Toggle toggle)
    {
        foreach (RectTransform line in excelReader.uiLines)
        {
            line.GetChild(3).GetChild(1).gameObject.SetActive(!toggle.isOn); // toggle의 터치 안내 텍스트
            line.GetChild(3).GetChild(2).gameObject.SetActive(toggle.isOn);  // toggle의 정답 텍스트
        }
    }

    // N 번째 라인에 대하여 N의 값을 구하는 함수
    public void AnswerIndex(int index)
    {
        lineIndex = index;   
    }

    public void OpenAnswer(Toggle toggle)
    {
        // 인덱스를 통해 몇번째 라인인지 체크하고 해당 라인에 대해서만 변화를 적용
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(1).gameObject.SetActive(!toggle.isOn);
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(2).gameObject.SetActive(toggle.isOn);
    }

    public void ShowWarning(bool isReturn)
    {
        uiWarning.SetActive(true);
        if(isReturn)
        {
            uiReturnSelectWarning.SetActive(true);
        }
        else
        {
            uiPageMoveWarning.SetActive(true);
        }
    }

    // 취소 버튼
    public void Cancel(bool isReturn)
    {
        uiWarning.SetActive(false);
        if (isReturn)
        {
            uiReturnSelectWarning.SetActive(false);
        }
        else
        {
            uiPageMoveWarning.SetActive(false);
        }
    }

    // 페이지 이동시 다음 페이지인지 이전 페이지인지 구분하기 위한 인덱스를 로드
     public void DirCheck(bool isRight)
    {
        this.isRight = isRight;
    }
}
