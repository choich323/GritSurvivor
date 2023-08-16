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

    // ���� ����
    float score;

    void Awake()
    {
        excelReader = GetComponent<ExcelReader>();
        if (!PlayerPrefs.HasKey("lastTest"))
        {
            Init();
        }
    }

    // ���� ���� ������ �ʱ�ȭ
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
        // ������
        float max = (textBackNum - textFrontNum + 1) * 30;
        // �����
        float correct = 0;

        // ���� ������ ������ ����
        int pageIndex = (int.Parse(MemorizationManager.instance.uiPageNumber.text) - 1) * 30;
        foreach(RectTransform line in excelReader.uiLines)
        {
            InputField inputData = line.GetChild(2).GetComponent<InputField>();
            excelReader.answer[pageIndex] = inputData.text;
            pageIndex++;
        }
        // ä��
        for(int index = (textFrontNum - 1) * 30; index < textBackNum * 30; index++)
        {
            // ��
            string means = excelReader.randWords[excelReader.keys[keyIndex]][1];
            bool right = false;
            // ���� ���� ���� ���
            foreach(string mean in means.Split('.'))
            {
                // ������ �ۼ��� ���� ���� ���� ���
                foreach (string ans in excelReader.answer[index].Split(','))
                {
                    // ���� �ִ� ��� ����
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
            // �����̸�
            if (right) correct++;
            else
            {
                BackendGameData.Instance.BookmarkDataAdd(excelReader.randWords[excelReader.keys[keyIndex]][0], excelReader.randWords[excelReader.keys[keyIndex]][2]);
            }
            keyIndex++;
        }

        // ���� ���: �Ҽ��� 2��° �ڸ�����
        score = Mathf.Round(((float)correct / max) * 10000f) / 100f;

        // ���ÿ� ����
        // ���� �ð�
        string testLog = DateTime.Now.ToString() + "\n" + MemorizationManager.instance.ifFront.text + "~" + MemorizationManager.instance.ifBack.text + "_" + correct.ToString() + "/" + max.ToString() + "_" + score.ToString() + "��";
        // ���̺� ���� �ѹ�
        int slotNum = (PlayerPrefs.GetInt("lastTest") + 1) % 10;       
        // ���� ���Կ� �׽�Ʈ ��� ����
        PlayerPrefs.SetString("testSlot" + slotNum.ToString(), testLog);

        PlayerPrefs.SetInt("lastTest", slotNum);

        if(score >= 60f)
        {
            // ���� �ϸ�ũ�� ���
            BackendGameData.Instance.BookmarkDataUpload("Last Test");
            // �հ� ���ο� ���� ������ �α� ���
            if (score >= 90f)
            {
                InsertLog(testLog);
            }
        }
        BackendGameData.userData.words.Clear();

        // ����Ȯ�� â�� ���� ���� Ȯ�ζ��� �ѱ�
        uiSubmitCheck.SetActive(false);
        uiScoreResult.SetActive(true);
        Text scoreResult = uiScoreResult.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>();
        // �Ҽ��� ��°�ڸ����� Ȯ��
        scoreResult.text = "���� ���: " + correct + " / " + max + string.Format("\n�����: {0:F2}", score);
    }

    // ���� ��� Ȯ���� ���� �����ϰ� ���� ����ȭ������ ���ư��� �Լ�
    public void ExitTest()
    {
        uiScoreResult.SetActive(false);
        MemorizationManager.instance.ReturnSelect();
    }

    // ���� �����͸� �α׷� �����ϴ� �Լ�
    public void InsertLog(string testLog)
    {
        Debug.Log("�α� ������ �õ��մϴ�.");

        Param param = new Param();
        param.Add("���� ��¥/���� ����_���� ���_���� ����", testLog);

        // �г��Ӱ� �׽�Ʈ ���, ��� ��ȿ�Ⱓ
        var bro = Backend.GameLog.InsertLog("TestPass", param, 10);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("�α� ���� �� ������ �߻��߽��ϴ�. : " + bro);
            return;
        }

        Debug.Log("�α� ���Կ� �����߽��ϴ�. : " + bro);
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
