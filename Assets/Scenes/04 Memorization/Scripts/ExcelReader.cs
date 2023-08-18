using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcelReader : MonoBehaviour
{
    // For Test Mode Distinction
    public bool isTestMode;

    // Array containing prefab with row data
    public RectTransform[] uiLines;

    // Container for storing csv data
    List<Dictionary<string, object>> data;

    int pageIndex = 0;

    WaitForSeconds wait;

    // Save random question data
    public Dictionary<float, string[]> randWords = new Dictionary<float, string[]>();
    [HideInInspector]
    public List<float> keys = new List<float>();
    [HideInInspector]
    // Array to store user input
    public string[] answer = new string[6001];

    void Awake()
    {
        // Initialization of CSV Data
        if (isTestMode)
            data = CSVReader.Read("VOCA(test)");
        else
            data = CSVReader.Read("VOCA(memory)");
        wait = new WaitForSeconds(2f);
    }

    public void Init()
    {
        StopCoroutine("OffError");

        // Range of chapter for memorization from user input
        string textFront = MemorizationManager.instance.ifFront.text;
        string textBack = MemorizationManager.instance.ifBack.text;

        // Error Exception: when start chapter is empty
        if (textFront == "")
        {
            MemorizationManager.instance.uiRangeError.SetActive(true);
            // Off Error UI after 2 seconds
            StartCoroutine("OffError");
            return;
        }
        // Automatically set to 200 if the last chapter is blank
        if (textBack == "")
        {
            MemorizationManager.instance.ifBack.text = "200";
            textBack = "200";
        }

        // Range Type change: string -> int
        int textFrontNum = int.Parse(textFront);
        int textBackNum = int.Parse(textBack);

        // Error Exception: if the range is entered incorrectly
        if (textFrontNum > 200 || textFrontNum <= 0 || textBackNum > 200 || textBackNum <= 0 || textFrontNum > textBackNum)
        {
            MemorizationManager.instance.uiRangeError.SetActive(true);
            // Off Error UI after 2 seconds
            StartCoroutine("OffError");
            return;
        }

        // random ordering for test mode
        if (isTestMode)
        {
            // User input data clear
            for (int i = 0; i < 6001; i++)
                answer[i] = "";

            // Random word data clear
            randWords.Clear();
            keys.Clear();

            for (int index = (textFrontNum - 1) * 30; index < textBackNum * 30; index++) {
                // word data set: { word, meaning for scoring, meaning of voca book }
                string[] word = new string[3];
                word[0] = data[index]["word"].ToString();
                word[1] = data[index]["mean"].ToString();
                word[2] = data[index]["origin"].ToString();
                // Create a random variable that will be the key to the word, and save the word to the dictionary
                float key = Random.value;
                randWords[key] = word;
                keys.Add(key);
            }
            // Key sort for random ordering
            keys.Sort();
        }
        // Range text that shows range of memorization or test
        MemorizationManager.instance.uiTestRange.text = "(" + textFrontNum.ToString() + " ~ " + textBackNum.ToString() + ")";
        PageLoad(textFrontNum, textBackNum, 0, true);

        // UI Change
        MemorizationManager.instance.EnterMemorization();
    }

    // Off Text UI when range error
    IEnumerator OffError()
    {
        yield return wait;

        MemorizationManager.instance.uiRangeError.SetActive(false);
    }

    // Page data update
    public void PageLoad(int textFrontNum, int textBackNum, int add, bool isInit)
    {
        // Index for row separation
        pageIndex = (textFrontNum + add - 1) * 30;

        // move to last page
        if (pageIndex < (textFrontNum - 1) * 30)
        {
            pageIndex = (textBackNum - 1) * 30;
            MemorizationManager.instance.add = textBackNum - textFrontNum;
        }
        // move to first page
        else if (pageIndex >= textBackNum * 30)
        {
            pageIndex = (textFrontNum - 1) * 30;
            MemorizationManager.instance.add = 0;
        }

        // memorization
        if (!isTestMode)
        {
            foreach (RectTransform line in uiLines)
            {
                // load word data
                Text word = line.GetChild(1).GetComponent<Text>();
                word.text = data[pageIndex]["word"].ToString();

                // load meaning
                Text textAnswer = line.GetChild(3).GetChild(2).GetComponent<Text>();
                textAnswer.text = data[pageIndex]["mean"].ToString();

                // Hide answer when it is open
                Toggle toggleAnswerCheck = line.GetChild(3).GetComponent<Toggle>();
                toggleAnswerCheck.isOn = false;

                // Inputfield clear
                InputField inputData = line.GetChild(2).GetComponent<InputField>();
                inputData.text = "";

                // Go to next word
                pageIndex++;
            }
            // Hide answers when they are open
            MemorizationManager.instance.toggleAnswerCheckAll.isOn = false;

        }
        // test
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
                    // Save user input data on the current page
                    if (pageIndex - 30 < 0)
                        answer[(textBackNum - 1) * 30 + pageIndex] = inputData.text;
                    else
                        answer[pageIndex - 30] = inputData.text;
                }
                // Load new data
                inputData.text = answer[pageIndex];
                
                // Go to next word
                pageIndex++; keyIndex++;
            }
        }
        // Change page number
        MemorizationManager.instance.uiPageNumber.text = (textFrontNum + MemorizationManager.instance.add).ToString();
    }
}