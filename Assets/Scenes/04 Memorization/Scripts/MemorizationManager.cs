using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemorizationManager : MonoBehaviour
{
    public static MemorizationManager instance;
    ExcelReader excelReader;

    // ui~ : Image or text without interaction, btn~ : button, if~ : inputfield

    [Header("# Selection UI")]
    public GameObject scrollViewWordGroup;
    public GameObject uiSelect;
    public GameObject uiRangeError;
    public InputField ifFront;
    public InputField ifBack;

    [Header("# Memorization & Test UI")]
    public GameObject btnReturnSelect;
    public GameObject btnLeft;
    public GameObject btnRight;
    public Text uiPageNumber;
    public Text uiTestRange;
    [Header("# Memorization mode only")]
    public Toggle toggleAnswerCheckAll;

    // Warning UI when user wants to change the page
    [Header("# Page Change UI")]
    // Change warning UI Group
    public GameObject uiWarning;
    // UI of page moving warning
    public GameObject uiPageMoveWarning;
    // UI of returning to selection UI
    public GameObject uiReturnSelectWarning;

    [HideInInspector]
    // Save page variation
    public int add = 0;
    // Check the number of lines 
    int lineIndex = 0;
    // Is the button that goes to next page?
    bool isRight;

    // Save bookmark word data(last test incorrection words)(bm: bookmark)
    string[] bmWords;
    string[] bmMeans;
    int bmWordIndex = 0;

    [HideInInspector]
    // Check if bookmark data has ever been loaded
    public bool isLoad = false;
    bool isBookmarkMode = false;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        excelReader = GetComponent<ExcelReader>();
    }

    // Range Selection -> Memorization or Test
    public void EnterMemorization()
    {
        // Turn off selection UI, warning and turn on return button, scroll view
        uiSelect.SetActive(false);
        uiRangeError.SetActive(false);
        btnReturnSelect.SetActive(true);
        scrollViewWordGroup.SetActive(true);

        // Disable page movement when selecting only one chapter
        if (ifFront.text == ifBack.text)
        {
            btnLeft.SetActive(false);
            btnRight.SetActive(false);
        }
        else
        {
            btnRight.SetActive(true);
            btnLeft.SetActive(true);
        }
    }

    public void EnterBookmarkMode()
    {
        isBookmarkMode = true;

        // Load bookmark data
        // Load data only once for the first time
        if (!isLoad) { 
            BookmarkLoad();
        }
        BookmarkSync();

        // Turn off selection UI, warning and turn on return button, scroll view
        uiSelect.SetActive(false);
        uiRangeError.SetActive(false);
        btnReturnSelect.SetActive(true);
        scrollViewWordGroup.SetActive(true);
        // Clear page number
        uiPageNumber.text = "";

        // No page movement required if the number of saved words
        if (bmWords.Length <= 30)
        {
            btnRight.SetActive(false);
            btnLeft.SetActive(false);
        }
        else
        {
            btnRight.SetActive(true);
            btnLeft.SetActive(true);
        }
    }

    // Memorization UI -> Range selection UI
    public void ReturnSelect()
    {
        // Range Clear
        ifFront.text = "";
        ifBack.text = "";
        // Page variation clear
        add = 0;

        // UI clear
        uiSelect.SetActive(true);
        btnReturnSelect.SetActive(false);
        scrollViewWordGroup.SetActive(false);
        uiWarning.SetActive(false);
        uiReturnSelectWarning.SetActive(false);

        // bookmark mode clear
        isBookmarkMode = false;
        bmWordIndex = 0;
    }

    // Return to Main Menu
    public void ReturnMain()
    {
        SceneManager.LoadScene("03 MainMenu");
    }

    public void PageChange()
    {
        // Turn off warning UI
        uiWarning.SetActive(false);
        if(!excelReader.isTestMode)
            uiPageMoveWarning.SetActive(false);

        if (isBookmarkMode)
        {
            if (!isRight)
            {
                bmWordIndex -= 31;
                if(bmWordIndex < 0)
                {
                    // Move first page to last page in bookmark mode
                    bmWordIndex = bmWords.Length - 1;
                }
                bmWordIndex -= bmWordIndex % 30;
            }

            BookmarkSync();
        }
        else
        {
            int textFrontNum = int.Parse(ifFront.text);
            int textBackNum = int.Parse(ifBack.text);

            // next page button
            if (isRight)
            {
                excelReader.PageLoad(textFrontNum, textBackNum, ++add, false);
            }
            // previous page button
            else
            {
                excelReader.PageLoad(textFrontNum, textBackNum, --add, false);
            }
        }
    }

    public void BookmarkLoad()
    {
        if (isBookmarkMode)
        {
            // Load dat from DB
            BackendGameData.Instance.BookmarkDataGet();

            // Array size setting
            bmWords = new string[BackendGameData.userData.words.Count];
            bmMeans = new string[bmWords.Length];

            // Store data as an array, clear index and data from DB
            foreach (string key in BackendGameData.userData.words.Keys)
            {
                bmWords[bmWordIndex] = key;
                bmMeans[bmWordIndex++] = BackendGameData.userData.words[key];
            }
            bmWordIndex = 0;
            BackendGameData.userData.words.Clear();
        }
    }

    // Apply data to each line in bookmark mode
    public void BookmarkSync()
    {
        //  Move last page to first page in bookmark mode
        if (bmWordIndex == bmWords.Length)
        {
            bmWordIndex = 0;
        }
        foreach (RectTransform line in excelReader.uiLines)
        {
            // If there are fewer than 30 words to mark on a page, and there are no more words to mark, blanked
            if (bmWordIndex == bmWords.Length)
            {
                line.GetChild(1).GetComponent<Text>().text = "";
                line.GetChild(3).GetChild(2).GetComponent<Text>().text = "";
            }
            else
            {
                line.GetChild(1).GetComponent<Text>().text = bmWords[bmWordIndex];
                line.GetChild(3).GetChild(2).GetComponent<Text>().text = bmMeans[bmWordIndex++];
            }

            // Hide answer if it is open
            Toggle toggleAnswerCheck = line.GetChild(3).GetComponent<Toggle>();
            toggleAnswerCheck.isOn = false;
            
            // clear user input
            InputField inputData = line.GetChild(2).GetComponent<InputField>();
            inputData.text = "";
        }
        // Hide answers if they are open
        MemorizationManager.instance.toggleAnswerCheckAll.isOn = false;
    }

    // Open all answers
    public void OpenAllAnswer(Toggle toggle)
    {
        foreach (RectTransform line in excelReader.uiLines)
        {
            line.GetChild(3).GetChild(1).gameObject.SetActive(!toggle.isOn); // Touch guide text on toggle
            line.GetChild(3).GetChild(2).gameObject.SetActive(toggle.isOn);  // Answer text on toggle
        }
    }

    // Function to obtain the value of N for the Nth line; 0 <= N <= 29
    public void AnswerIndex(int index)
    {
        lineIndex = index;   
    }

    public void OpenAnswer()
    {
        bool isOn = excelReader.uiLines[lineIndex].GetChild(3).GetComponent<Toggle>().isOn;

        // Check the line index for the number of lines and apply changes
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(1).gameObject.SetActive(!isOn);
        excelReader.uiLines[lineIndex].GetChild(3).GetChild(2).gameObject.SetActive(isOn);
    }

    // Notify when switch page
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

    // Cancel page movement or returning to range selection
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

     public void DirCheck(bool isRight)
    {
        this.isRight = isRight;
    }
}
