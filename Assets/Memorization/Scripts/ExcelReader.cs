using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ExcelReader : MonoBehaviour
{
    // �ε��� ����
    string wordFile = @"E:\Programming_Practice\Unity\UndeadSurvivor\UndeadSurvivor\Assets\VOCA (Excel).xlsx";
    
    // �� ������ ������ ������ ���
    public RectTransform[] uiLines;

    public void DataLoad()
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
            textBack = "200";
        }

        // �ϱ� ���� ������ string -> int
        int textFrontNum = int.Parse(textFront);
        int textBackNum = int.Parse(textBack);

        // �ϱ� ������ �߸��� ��� ����ó��
        if (textFrontNum > 200 || textFrontNum <= 0 || textBackNum > 200 || textBackNum <= 0 || textFrontNum > textBackNum)
        {
            MemorizationManager.instance.uiWarningText.SetActive(true);
            return;
        }

        // ���� �ε�
        using (var stream = File.Open(wordFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {   // ���� �б�
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {   // �����ͼ� ����
                var result = reader.AsDataSet();
                // �� ���� �ε���: ������ é�ͺ��� ����
                int index = 0 + (textFrontNum - 1) * 30;
                foreach (RectTransform line in uiLines)
                {
                    // ���� ���� �ؽ�Ʈ ������ �ε�
                    Text[] childrens = line.GetComponentsInChildren<Text>();
                    // �ؽ�Ʈ ������Ʈ�� ���̺� ������ �ֱ�
                    foreach (Text child in childrens)
                    {
                        if (child.name == "Text Word")
                        {
                            child.text = result.Tables[0].Rows[index++][1].ToString();
                            break;
                        }
                    }
                    // �Է¶� ������ �ʱ�ȭ
                    InputField inputData = line.GetComponentInChildren<InputField>();
                    inputData.text = "";
                }
            }
        }
        // UI ��ȯ
        MemorizationManager.instance.EnterMemorization();
    }
}