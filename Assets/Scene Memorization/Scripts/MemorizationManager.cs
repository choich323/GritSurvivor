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
    // 확인창 Group
    public GameObject uiWarning;
    // 페이지 이동시 확인창
    public GameObject uiPageMoveWarning;
    // 챕터 선택 메뉴로 돌아갈 때 확인창
    public GameObject uiReturnSelectWarning;

    // 페이지 변동값 기록
    int add = 0;
    int lineIndex = 0;
    bool isRight;

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

        // 한 개 챕터만 선택하는 경우에 페이지 이동 불가능하게 설정
        if (ifFront.text == ifBack.text)
        {
            btnRight.SetActive(false);
        }
        else
        {
            btnRight.SetActive(true);
        }
        // 이전 페이지 버튼은 처음에 무조건 비활성화
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
        // 북마크 동기화
        BookmarkUpdate();
    }

    // 메인 모드로 돌아가기
    public void ReturnMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PageChange()
    {
        // 북마크 동기화
        BookmarkUpdate();

        int textFrontNum = int.Parse(ifFront.text);

        // 경고창 끄기
        uiWarning.SetActive(false);
        uiPageMoveWarning.SetActive(false);

        // 오른쪽 버튼이면
        if (isRight)
        {
            excelReader.PageLoad(textFrontNum, ++add);
        }
        // 왼쪽 버튼
        else
        {
            excelReader.PageLoad(textFrontNum, --add);
        }

        // 첫 번호 or 마지막 번호인 경우 왼쪽/오른쪽 버튼 각각 on/off
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

    // 북마크 업데이트 함수
    public void BookmarkUpdate()
    {
        for (int index = 0; index < 30; index++)
        {
            bool isOn = excelReader.uiLines[index].GetChild(4).GetComponent<Toggle>().isOn;

            // 토글을 통해 즐겨찾기에 추가
            // 즐겨찾기 등록되어 있지 않았던 경우이므로 추가
            if (isOn)
            {
                string word = excelReader.uiLines[index].GetChild(1).GetComponent<Text>().text;
                string mean = excelReader.uiLines[index].GetChild(3).GetChild(2).GetComponent<Text>().text;
                BackendGameData.Instance.BookmarkDataAdd(word, mean);
            }
        }
        BackendGameData.Instance.BookmarkDataUpload("Day " + uiPageNumber.text);
    }

    // 모든 정답 공개하기 함수
    public void OpenAllAnswer(Toggle toggle)
    {
        foreach (RectTransform line in excelReader.uiLines)
        {
            line.GetChild(3).GetChild(1).gameObject.SetActive(!toggle.isOn); // toggle의 터치 안내 텍스트
            line.GetChild(3).GetChild(2).gameObject.SetActive(toggle.isOn);  // toggle의 정답 텍스트
        }
    }

    // N 번째 라인에 대하여 N의 값을 구하는 함수; 0 <= N <= 29
    public void AnswerIndex(int index)
    {
        lineIndex = index;   
    }

    public void OpenAnswer()
    {
        bool isOn = excelReader.uiLines[lineIndex].GetChild(3).GetComponent<Toggle>().isOn;

        // 인덱스를 통해 몇번째 라인인지 체크하고 해당 라인에 대해서만 변화를 적용
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(1).gameObject.SetActive(!isOn);
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(2).gameObject.SetActive(isOn);
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

    // 페이지 이동 or 챕터 선택으로 돌아가기 취소 버튼
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

    // 페이지 이동시 다음 페이지인지 이전 페이지인지, 즉 오른쪽 버튼을 눌렀는지 왼쪽 버튼을 눌렀는지 알려준다
    // 이를 이용해서 페이지를 관리하는 인덱스의 값을 더하거나 빼준다.
     public void DirCheck(bool isRight)
    {
        this.isRight = isRight;
    }
}
