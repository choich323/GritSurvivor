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
        
        // UI �����ֱ�
        rect.localScale = Vector3.one;

        GameManager.instance.Stop();
    }

    public void Hide()
    {
        // UI �����
        rect.localScale = Vector3.zero;

        GameManager.instance.Resume();
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // ��� ������ ��Ȱ��ȭ
        foreach(Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        // ���� 3���� Ȱ��ȭ
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

            // �̹� �ϼ��� �������� �Һ� ���������� ��ü
            if (ranItem.level == ranItem.data.damages.Length)
            {
                // ȸ���������� ���� ���� ���� �������� �����ֱ�
                ranItem = items[4];
            }

            ranItem.gameObject.SetActive(true);
        }
    }
}
