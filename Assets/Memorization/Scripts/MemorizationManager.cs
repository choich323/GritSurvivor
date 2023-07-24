using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemorizationManager : MonoBehaviour
{
    public static MemorizationManager instance;

    // btn~ : 버튼, if: 인풋 필드

    [Header ("# Select UI")]
    public GameObject scrollViewWordGroup;
    public GameObject uiSelect;
    public GameObject uiWarningText;

    [Header ("# Memorization UI")]
    public GameObject btnReturnSelect;
    public InputField ifFront;
    public InputField ifBack;

    void Awake()
    {
        instance = this;
    }

    // 범위 선택 UI -> 암기 UI
    public void EnterMemorization()
    {
        // 선택창/경고 끄기, 선택창 돌아가기 버튼/스크롤뷰 켜기
        uiSelect.SetActive(false);
        uiWarningText.SetActive(false);
        btnReturnSelect.SetActive(true);
        scrollViewWordGroup.SetActive(true);
    }

    // 암기 UI -> 범위 선택 UI
    public void ReturnSelect()
    {
        // 입력란 초기화
        ifFront.text = "";
        ifBack.text = "";
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
}
