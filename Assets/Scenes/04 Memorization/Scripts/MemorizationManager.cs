using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemorizationManager : MonoBehaviour
{
    public static MemorizationManager instance;
    ExcelReader excelReader;

    // ui~ : ��ȣ�ۿ��� ���� �̹����� �ؽ�Ʈ ��, btn~ : ��ư, if~ : ��ǲ �ʵ�

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

    // ������ ��ȯ�� ��� UI
    [Header("# Page Change Warning UI")]
    // Ȯ��â Group
    public GameObject uiWarning;
    // ������ �̵��� Ȯ��â
    public GameObject uiPageMoveWarning;
    // é�� ���� �޴��� ���ư� �� Ȯ��â
    public GameObject uiReturnSelectWarning;

    // �ϸ�ũ �ܾ� ������ ����(bm: bookmark)
    string[] bmWords;
    string[] bmMeans;
    int bmWordIndex = 0;

    // ��� Ȯ�ο�
    bool isBookmarkMode = false;
    public bool isLoad = false;

    // ������ ������ ���
    int add = 0;
    int lineIndex = 0;
    bool isRight;

    void Awake()
    {
        instance = this;
        excelReader = GetComponent<ExcelReader>();
    }

    // ���� ���� UI -> �ϱ� UI
    public void EnterMemorization()
    {
        // ����â/��� ����, ����â ���ư��� ��ư/��ũ�Ѻ� �ѱ�
        uiSelect.SetActive(false);
        uiWarningText.SetActive(false);
        btnReturnSelect.SetActive(true);
        scrollViewWordGroup.SetActive(true);

        // ���ã�� ������ �ε�
        //BookmarkLoad();

        if (!excelReader.isTestMode)
        {
            // �� �� é�͸� �����ϴ� ��쿡 ������ �̵� �Ұ����ϰ� ����
            if (ifFront.text == ifBack.text)
            {
                btnRight.SetActive(false);
            }
            else
            {
                btnRight.SetActive(true);
            }
            // ���� ������ ��ư�� ó���� ������ ��Ȱ��ȭ
            btnLeft.SetActive(false);
        }
    }

    // �ϸ�ũ ��ư�� ��ġ���� �� �ϸ�ũ ���� ����
    public void EnterBookmarkMode()
    {
        isBookmarkMode = true;

        // ���ã�� ������ �ε�, UI�� ����
        // ���� 1ȸ�� �����͸� �ε�
        if (!isLoad) { 
            BookmarkLoad();
        }
        BookmarkSync();

        // ����â/��� ����, ����â ���ư��� ��ư/��ũ�Ѻ� �ѱ�
        uiSelect.SetActive(false);
        uiWarningText.SetActive(false);
        btnReturnSelect.SetActive(true);
        scrollViewWordGroup.SetActive(true);

        // �ϸ�ũ�� ����� �ܾ��� ������ 30�� ������ ��� ������ �̵� ���ʿ�
        if (bmWords.Length <= 30)
        {
            btnRight.SetActive(false);
        }
        else
        {
            btnRight.SetActive(true);
        }
        // ���� ������ ��ư�� ó���� ������ ��Ȱ��ȭ
        btnLeft.SetActive(false);
    }

    // �ϱ� UI -> ���� ���� UI
    public void ReturnSelect()
    {
        // �Է¶� �ʱ�ȭ
        ifFront.text = "";
        ifBack.text = "";
        // ������ �ʱ�ȭ
        add = 0;
        // ����â �ѱ�, ����â ���ư��� ��ư/��ũ�� ��, ���â ����
        uiSelect.SetActive(true);
        btnReturnSelect.SetActive(false);
        scrollViewWordGroup.SetActive(false);
        uiWarning.SetActive(false);
        uiReturnSelectWarning.SetActive(false);
        // �ϸ�ũ ����ȭ
        //BookmarkUpdate();
        // �ϸ�ũ ��忴�� ��쿡 ��Ȱ��ȭ, �ε��� �ʱ�ȭ
        isBookmarkMode = false;
        bmWordIndex = 0;
    }

    // ���� ���� ���ư���
    public void ReturnMain()
    {
        SceneManager.LoadScene("03 MainMenu");
    }

    public void PageChange()
    {
        // ���â ����
        uiWarning.SetActive(false);
        uiPageMoveWarning.SetActive(false);

        if (isBookmarkMode)
        {
            if (!isRight)
            {
                bmWordIndex -= 31;
                bmWordIndex -= bmWordIndex % 30;
            }

            BookmarkSync();

            if (bmWordIndex <= 30)
            {
                btnLeft.SetActive(false);
                btnRight.SetActive(true);
            }
            else if(bmWordIndex >= bmWords.Length)
            {
                btnRight.SetActive(false);
                btnLeft.SetActive(true);
            }
            else
            {
                btnLeft.SetActive(true);
                btnRight.SetActive(true);
            }
        }
        else
        {
            // �ϸ�ũ ����ȭ
            //BookmarkUpdate();

            int textFrontNum = int.Parse(ifFront.text);

            // ������ ��ư�̸�
            if (isRight)
            {
                excelReader.PageLoad(textFrontNum, ++add);
            }
            // ���� ��ư
            else
            {
                excelReader.PageLoad(textFrontNum, --add);
            }

            // �ϸ�ũ ������ �ε�
            //BookmarkLoad();

            // ù ��ȣ or ������ ��ȣ�� ��� ����/������ ��ư ���� on/off
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
    }

    public void BookmarkLoad()
    {
        if (isBookmarkMode)
        {
            // DB �����Ͽ� ������ �ҷ�����
            BackendGameData.Instance.BookmarkDataGet();

            // �迭 ũ�� ����
            bmWords = new string[BackendGameData.userData.words.Count];
            bmMeans = new string[bmWords.Length];

            // �����͸� �迭�� �����ϰ� �ε���, DB���� ���� �����ʹ� �ʱ�ȭ
            foreach (string key in BackendGameData.userData.words.Keys)
            {
                bmWords[bmWordIndex] = key;
                bmMeans[bmWordIndex++] = BackendGameData.userData.words[key];
            }
            bmWordIndex = 0;
            BackendGameData.userData.words.Clear();
        }
        else
        {
            BackendGameData.Instance.BookmarkDataGet("Day " + uiPageNumber.text);

            foreach (RectTransform line in excelReader.uiLines)
            {
                // ��� �ʱ�ȭ
                line.GetChild(4).GetComponent<Toggle>().isOn = false;
                foreach (string key in BackendGameData.userData.words.Keys)
                {
                    // �� é�� ������ ������ �ܾ ������ ��� Ȱ��ȭ
                    if (line.GetChild(1).GetComponent<Text>().text == key)
                    {
                        line.GetChild(4).GetComponent<Toggle>().isOn = true;
                        break;
                    }
                }
            }
            // �ε��ߴ� ������ �ʱ�ȭ
            BackendGameData.userData.words.Clear();
        }
    }

    // �ϸ�ũ ��忡�� �ϸ�ũ �����͸� ������ ���κ� �ؽ�Ʈ�� ����
    public void BookmarkSync()
    {
        foreach (RectTransform line in excelReader.uiLines)
        {
            // �� �������� ǥ���� �ܾ 30������ ��� ǥ���� �ܾ �� ���� ��� ���� ǥ��
            if (bmWordIndex == bmWords.Length)
            {
                line.GetChild(1).GetComponent<Text>().text = "";
                line.GetChild(3).GetChild(2).GetComponent<Text>().text = "";
            }
            else
            {
                // ���ܾ�� �� �Ҵ�
                line.GetChild(1).GetComponent<Text>().text = bmWords[bmWordIndex];
                line.GetChild(3).GetChild(2).GetComponent<Text>().text = bmMeans[bmWordIndex++];
            }

            // ���� �����Ǿ� �ִ� ��� ������
            Toggle toggleAnswerCheck = line.GetChild(3).GetComponent<Toggle>();
            toggleAnswerCheck.isOn = false;
            
            // ��ǲ �ʵ� �ʱ�ȭ
            InputField inputData = line.GetChild(2).GetComponent<InputField>();
            inputData.text = "";
        }
        // ��ü �� ���� ���� ����
        MemorizationManager.instance.toggleAnswerCheckAll.isOn = false;
    }

    // �ϸ�ũ ������Ʈ �Լ�
    public void BookmarkUpdate()
    {
        for (int index = 0; index < 30; index++)
        {
            bool isOn = excelReader.uiLines[index].GetChild(4).GetComponent<Toggle>().isOn;

            // ����� ���� ���ã�⿡ �߰�
            // ���ã�� ��ϵǾ� ���� �ʾҴ� ����̹Ƿ� �߰�
            if (isOn)
            {
                string word = excelReader.uiLines[index].GetChild(1).GetComponent<Text>().text;
                string mean = excelReader.uiLines[index].GetChild(3).GetChild(2).GetComponent<Text>().text;
                BackendGameData.Instance.BookmarkDataAdd(word, mean);
            }
        }
        BackendGameData.Instance.BookmarkDataUpload("Day " + uiPageNumber.text);
    }

    // ��� ���� �����ϱ� �Լ�
    public void OpenAllAnswer(Toggle toggle)
    {
        foreach (RectTransform line in excelReader.uiLines)
        {
            line.GetChild(3).GetChild(1).gameObject.SetActive(!toggle.isOn); // toggle�� ��ġ �ȳ� �ؽ�Ʈ
            line.GetChild(3).GetChild(2).gameObject.SetActive(toggle.isOn);  // toggle�� ���� �ؽ�Ʈ
        }
    }

    // N ��° ���ο� ���Ͽ� N�� ���� ���ϴ� �Լ�; 0 <= N <= 29
    public void AnswerIndex(int index)
    {
        lineIndex = index;   
    }

    // ���� ���� ����
    public void OpenAnswer()
    {
        bool isOn = excelReader.uiLines[lineIndex].GetChild(3).GetComponent<Toggle>().isOn;

        // �ε����� ���� ���° �������� üũ�ϰ� �ش� ���ο� ���ؼ��� ��ȭ�� ����
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(1).gameObject.SetActive(!isOn);
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(2).gameObject.SetActive(isOn);
    }

    // ������ ��ȯ�� �˸�
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

    // ������ �̵� or é�� �������� ���ư��� ��� ��ư
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

    // ������ �̵��� ���� ���������� ���� ����������, �� ������ ��ư�� �������� ���� ��ư�� �������� �˷��ش�
    // �̸� �̿��ؼ� �������� �����ϴ� �ε����� ���� ���ϰų� ���ش�.
     public void DirCheck(bool isRight)
    {
        this.isRight = isRight;
    }
}
