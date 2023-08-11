using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class TestManager : MonoBehaviour
{
    public GameObject uiSubmitCheck;
    public GameObject uiScoreResult;
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
        // ���� ��Ͽ� ����Ʈ
        List<string[]> wrong = new List<string[]>();

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
                // ������ ���� ����
                wrong.Add(excelReader.randWords[excelReader.keys[keyIndex]]);
            }
            keyIndex++;
        }

        // ���� ���: �Ҽ��� 2��° �ڸ�����
        score = Mathf.Round(((float)correct / max) * 10000f) / 100f;

        // ���ÿ� ����
        // ���� �ð�
        string testLog = DateTime.Now.ToString() + "\n" + MemorizationManager.instance.ifFront.text + "~" + MemorizationManager.instance.ifBack.text + "_" + correct.ToString() + "/" + max.ToString() + "_" + score.ToString() + "��";
        // ���̺� ���� �ѹ�
        int slotNum = PlayerPrefs.GetInt("lastTest");
        // ���� ���Կ� �׽�Ʈ ��� ����
        PlayerPrefs.SetString("testSlot" + slotNum.ToString(), testLog);
        slotNum++;
        if (slotNum > 9)
            slotNum = 0;
        // ���� �ѹ� �������Ѽ� ����
        PlayerPrefs.SetInt("lastTest", slotNum);

        // ���� �ϸ�ũ�� ���


        // �հ� ���ο� ���� ������ �α� ���
        if (score >= 90f)
        {
            InsertLog(testLog);
        }

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
}
