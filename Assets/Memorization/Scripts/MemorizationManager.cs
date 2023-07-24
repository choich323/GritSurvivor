using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemorizationManager : MonoBehaviour
{
    public static MemorizationManager instance;

    // btn~ : ��ư, if: ��ǲ �ʵ�

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

    // ���� ���� UI -> �ϱ� UI
    public void EnterMemorization()
    {
        // ����â/��� ����, ����â ���ư��� ��ư/��ũ�Ѻ� �ѱ�
        uiSelect.SetActive(false);
        uiWarningText.SetActive(false);
        btnReturnSelect.SetActive(true);
        scrollViewWordGroup.SetActive(true);
    }

    // �ϱ� UI -> ���� ���� UI
    public void ReturnSelect()
    {
        // �Է¶� �ʱ�ȭ
        ifFront.text = "";
        ifBack.text = "";
        // ����â �ѱ�, ����â ���ư��� ��ư/��ũ�� �� ����
        uiSelect.SetActive(true);
        btnReturnSelect.SetActive(false);
        scrollViewWordGroup.SetActive(false);
    }

    // ���� ���� ���ư���
    public void ReturnMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
