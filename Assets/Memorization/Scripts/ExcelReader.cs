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
    // 로드할 파일
    string wordFile = @"E:\Programming_Practice\Unity\UndeadSurvivor\UndeadSurvivor\Assets\VOCA (Excel).xlsx";
    
    // 행 데이터 저장할 프리펩 담기
    public RectTransform[] uiLines;

    public void DataLoad()
    {
        // 단어 암기 챕터 범위(텍스트)
        string textFront = MemorizationManager.instance.ifFront.text;
        string textBack = MemorizationManager.instance.ifBack.text;

        // 시작 챕터가 빈칸인 경우 예외처리
        if (textFront == "")
        {
            MemorizationManager.instance.uiWarningText.SetActive(true);
            return;
        }
        // 끝 챕터가 빈칸인 경우 자동으로 200으로 설정
        if(textBack == "")
        {
            textBack = "200";
        }

        // 암기 범위 데이터 string -> int
        int textFrontNum = int.Parse(textFront);
        int textBackNum = int.Parse(textBack);

        // 암기 범위가 잘못된 경우 예외처리
        if (textFrontNum > 200 || textFrontNum <= 0 || textBackNum > 200 || textBackNum <= 0 || textFrontNum > textBackNum)
        {
            MemorizationManager.instance.uiWarningText.SetActive(true);
            return;
        }

        // 파일 로드
        using (var stream = File.Open(wordFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {   // 파일 읽기
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {   // 데이터셋 구성
                var result = reader.AsDataSet();
                // 행 구분 인덱스: 선택한 챕터부터 시작
                int index = 0 + (textFrontNum - 1) * 30;
                foreach (RectTransform line in uiLines)
                {
                    // 라인 별로 텍스트 데이터 로드
                    Text[] childrens = line.GetComponentsInChildren<Text>();
                    // 텍스트 컴포넌트에 테이블 데이터 넣기
                    foreach (Text child in childrens)
                    {
                        if (child.name == "Text Word")
                        {
                            child.text = result.Tables[0].Rows[index++][1].ToString();
                            break;
                        }
                    }
                    // 입력란 데이터 초기화
                    InputField inputData = line.GetComponentInChildren<InputField>();
                    inputData.text = "";
                }
            }
        }
        // UI 전환
        MemorizationManager.instance.EnterMemorization();
    }
}