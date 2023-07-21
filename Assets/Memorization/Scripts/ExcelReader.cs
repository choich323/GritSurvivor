using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using UnityEngine.UI;

public class ExcelReade : MonoBehaviour
{
    public Text[] words;

    void Awake()
    {
        string wordFile = @"E:\Programming_Practice\Unity\UndeadSurvivor\UndeadSurvivor\Assets\VOCA (Excel).xlsx";

        using (var stream = File.Open(wordFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                // UI�� ��ġ�� �ܾ��� ������ŭ �ݺ�
                for (int index = 0; index < words.Length; index++)
                {
                    //�ش� ���� 1,2 ���� ������ ��������
                    words[index].text = result.Tables[0].Rows[index][1].ToString();
                    string answer = result.Tables[0].Rows[index][2].ToString();
                }
            }
        }
    }
}
