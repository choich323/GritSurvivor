using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcelReader : MonoBehaviour
{
    int pageIndex = 0;

    // �� ������ ������ ������ ���
    public RectTransform[] uiLines;

    // csv �����͸� ������ �����̳�
    List<Dictionary<string, object>> data;

    void Awake()
    {
        // �ʱ�ȭ
        data = CSVReader.Read("VOCA");
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

        // ������ �ѹ� �ʱ�ȭ
        MemorizationManager.instance.uiPageNumber.text = string.Format("{0}", textFront);

        PageLoad(textFrontNum, 0);

        // UI ��ȯ
        MemorizationManager.instance.EnterMemorization();
    }

    // ������ ������ ����
    public void PageLoad(int textFrontNum, int add)
    {
        // 1. �ܾ� ����
        // �� ���� �ε���: ������ é�ͺ��� ����
        pageIndex = (textFrontNum + add - 1) * 30;
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
            
            // ���� �ܾ��
            pageIndex++;

            // 2. ��ǲ �ʵ� �ʱ�ȭ
            InputField inputData = line.GetChild(2).GetComponent<InputField>();
            inputData.text = "";
        }
        // ��ü �� ���� ���¸� ����
        MemorizationManager.instance.toggleAnswerCheckAll.isOn = false;

        // 3. ������ �ѹ� ����
        MemorizationManager.instance.uiPageNumber.text = string.Format("{0}", (textFrontNum + add).ToString());
    }
}