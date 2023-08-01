using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();
        // UI 보여주기
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();

        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        // UI가 나타났을 때 배경음악을 필터링
        // 이 경우 이펙트 소리까지 필터링 될 수 있으므로 오디오 매니저에서 예외처리
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        // UI 숨기기
        rect.localScale = Vector3.zero;

        GameManager.instance.Resume();

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // 모든 아이템 비활성화
        foreach(Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        // 랜덤 3개만 활성화
        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
            {
                break;
            }
        }

        for(int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];

            // 이미 완성된 아이템은 소비 아이템으로 대체
            if (ranItem.level == ranItem.data.damages.Length)
            {
                // 회복아이템이 여러 개면 랜덤 레인지로 정해주기
                ranItem = items[4];
            }

            ranItem.gameObject.SetActive(true);
        }
    }
}
