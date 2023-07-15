using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.expManager.nextExp[GameManager.instance.level - 1];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                // 0번째 인자값:포맷형태(소수점이 없음)
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                // remainTime을 쓰면 시간이 감소하는 방식으로 구성 가능
                // float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(GameManager.instance.gameTime / 60);
                int sec = Mathf.FloorToInt(GameManager.instance.gameTime % 60);
                // 자릿수 고정을 위한 포맷은 D + 원하는 자릿수 갯수
                myText.text = string.Format("{0:D2}:{1:D2} ", min, sec);
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
