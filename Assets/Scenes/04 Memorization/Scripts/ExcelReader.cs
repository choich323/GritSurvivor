using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcelReader : MonoBehaviour
{
    string[] answer;
    int pageIndex = 0;

    public bool isTestMode;

    // �� ������ ������ ������ ���
    public RectTransform[] uiLines;

    // csv �����͸� ������ �����̳�
    List<Dictionary<string, object>> data;

    void Awake()
    {
        // �ʱ�ȭ
        if (isTestMode)
            data = CSVReader.Read("VOCA(test)");
        else
            data = CSVReader.Read("VOCA(memory)");
    }

    public void Init()
    {
        // �ܾ� �ϱ� é�� ����(�ؽ�Ʈ)
        string textFront = MemorizationManager.instance.ifFront.text;
        string textBack = MemorizationManager.instance.ifBack.text;

        // ���� é�Ͱ� ��ĭ�� ��� ����ó��
        if (textFront == "")
        {
            MemorizationManager.instance.uiWarningText.SetActive(true);
            return;
        }
        // �� é�Ͱ� ��ĭ�� ��� �ڵ����� 200���� ����
        if(textBack == "")
        {
            MemorizationManager.instance.ifBack.text = "200";
            textBack = "200";
        }

        // �ϱ� ���� ������ string -> int
        int textFrontNum = int.Parse(textFront);
        int textBackNum = int.Parse(textBack);

        // �ϱ� é���� ������ �߸��� ��� ����ó��
        if (textFrontNum > 200 || textFrontNum <= 0 || textBackNum > 200 || textBackNum <= 0 || textFrontNum > textBackNum)
        {
            MemorizationManager.instance.uiWarningText.SetActive(true);
            return;
        }

        answer = new string[6001];
        for (int i = 0; i < 6001; i++)
            answer[i] = "";
        PageLoad(textFrontNum, textBackNum, 0);

        // UI ��ȯ
        MemorizationManager.instance.EnterMemorization();
    }

    // ������ ������ ����
    public void PageLoad(int textFrontNum, int textBackNum, int add)
    {
        // �ܾ� ����
        // �� ���� �ε���: ������ é�ͺ��� ����
        pageIndex = (textFrontNum + add - 1) * 30;

        // ���� ���������� �̵�
        if (pageIndex < (textFrontNum - 1) * 30)
        {
            pageIndex = (textBackNum - 1) * 30;
            MemorizationManager.instance.add = textBackNum - textFrontNum;
        }
        // ���� ���������� �̵�
        else if (pageIndex >= textBackNum * 30)
        {
            pageIndex = (textFrontNum - 1) * 30;
            MemorizationManager.instance.add = 0;
        }

        if (!isTestMode)
        {
            foreach (RectTransform line in uiLines)
            {
                // ���ܾ� ������ �ε�
                Text word = line.GetChild(1).GetComponent<Text>();
                word.text = data[pageIndex]["word"].ToString();

                // �ܾ� �� ������ �ε�
                Text textAnswer = line.GetChild(3).GetChild(2).GetComponent<Text>();
                textAnswer.text = data[pageIndex]["mean"].ToString();

                // ���� �����Ǿ� �ִ� ��� ������
                Toggle toggleAnswerCheck = line.GetChild(3).GetComponent<Toggle>();
                toggleAnswerCheck.isOn = false;

                // ��ǲ �ʵ� �ʱ�ȭ
                InputField inputData = line.GetChild(2).GetComponent<InputField>();
                inputData.text = "";

                // ���� �ܾ��
                pageIndex++;
            }
            // ��ü �� ���� ���¸� ����
            MemorizationManager.instance.toggleAnswerCheckAll.isOn = false;

        }
        else
        {
            foreach (RectTransform line in uiLines)
            {
                InputField inputData = line.GetChild(2).GetComponent<InputField>();

                // ���� �����͸� ����
                if (pageIndex - 30 < 0)
                    answer[(textBackNum - 1) * 30 + pageIndex] = inputData.text;
                else
                    answer[pageIndex - 30] = inputData.text;

                // ���ο� �����͸� �ε�
                inputData.text = answer[pageIndex];

                // ���� �ܾ��
                pageIndex++;
            }
        }
        // ������ �ѹ� ����
        MemorizationManager.instance.uiPageNumber.text = string.Format("{0}", (textFrontNum + MemorizationManager.instance.add).ToString());
    }
}