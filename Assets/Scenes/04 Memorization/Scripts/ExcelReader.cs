using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcelReader : MonoBehaviour
{
    // �׽�Ʈ ��� ���п�
    public bool isTestMode;

    // �� ������ ������ ������ ���
    public RectTransform[] uiLines;

    // csv �����͸� ������ �����̳�
    List<Dictionary<string, object>> data;
    // �������� ������ ���� ������ ����
    public Dictionary<float, string[]> randWords = new Dictionary<float, string[]>();
    public List<float> keys = new List<float>();

    // ������ �Է��� ������ �迭
    public string[] answer = new string[6001];
    int pageIndex = 0;

    WaitForSeconds wait;

    void Awake()
    {
        // �ʱ�ȭ
        if (isTestMode)
            data = CSVReader.Read("VOCA(test)");
        else
            data = CSVReader.Read("VOCA(memory)");
        wait = new WaitForSeconds(2f);
    }

    public void Init()
    {
        // ���� ���� �ڷ�ƾ�� ���� ���̾��ٸ� �켱 ����
        StopCoroutine("OffError");

        // �ܾ� �ϱ� é�� ����(�ؽ�Ʈ)
        string textFront = MemorizationManager.instance.ifFront.text;
        string textBack = MemorizationManager.instance.ifBack.text;

        // ���� é�Ͱ� ��ĭ�� ��� ����ó��
        if (textFront == "")
        {
            MemorizationManager.instance.uiRangeError.SetActive(true);
            // 2�� �� �˸� ��
            StartCoroutine("OffError");
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
            MemorizationManager.instance.uiRangeError.SetActive(true);
            // 2�� �� �˸� ��
            StartCoroutine("OffError");
            return;
        }

        if (isTestMode)
        {
            // �Է¹޾Ҵ� ������ ���� �ʱ�ȭ
            for (int i = 0; i < 6001; i++)
                answer[i] = "";

            // ������ �ʱ�ȭ
            randWords.Clear();
            keys.Clear();

            for (int index = (textFrontNum - 1) * 30; index < textBackNum * 30; index++) {
                // �ܾ� pair
                string[] word = new string[2];
                word[0] = data[index]["word"].ToString();
                word[1] = data[index]["mean"].ToString();
                // �ܾ��� Ű�� �� random ������ ����� dictionary�� ����
                float key = Random.value;
                randWords[key] = word;
                keys.Add(key);
            }
            // Ű�� �����Ͽ� �������� ��ġ
            keys.Sort();
        }
        // �ϱ�/���� ���� �ؽ�Ʈ
        MemorizationManager.instance.uiTestRange.text = "(" + textFrontNum.ToString() + " ~ " + textBackNum.ToString() + ")";
        PageLoad(textFrontNum, textBackNum, 0, true);

        // UI ��ȯ
        MemorizationManager.instance.EnterMemorization();
    }

    // ���� ������ �ؽ�Ʈ ���� �ڷ�ƾ
    IEnumerator OffError()
    {
        yield return wait;

        MemorizationManager.instance.uiRangeError.SetActive(false);
    }

    // ������ ������ ����
    public void PageLoad(int textFrontNum, int textBackNum, int add, bool isInit)
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
            int keyIndex = pageIndex - (textFrontNum - 1) * 30;
            foreach (RectTransform line in uiLines)
            {
                Text word = line.GetChild(1).GetComponent<Text>();
                word.text = randWords[keys[keyIndex]][0];

                InputField inputData = line.GetChild(2).GetComponent<InputField>();

                if (!isInit)
                {
                    // ���� �����͸� ����
                    if (pageIndex - 30 < 0)
                        answer[(textBackNum - 1) * 30 + pageIndex] = inputData.text;
                    else
                        answer[pageIndex - 30] = inputData.text;
                }
                // ���ο� �����͸� �ε�
                inputData.text = answer[pageIndex];
                
                // ���� �ܾ��
                pageIndex++; keyIndex++;
            }
        }
        // ������ �ѹ� ����
        MemorizationManager.instance.uiPageNumber.text = (textFrontNum + MemorizationManager.instance.add).ToString();
    }
}