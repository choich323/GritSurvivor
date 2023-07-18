using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        // Basic Set
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                // ���� �ӵ� ����
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                // �̵� �ӵ� ����
                SpeedUp(); 
                break;
        }
    }

    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach(Weapon weapon in weapons)
        {
            switch (weapon.id)
            {   
                case 0:
                    // �⺻ ���ݼӵ� * ĳ���� ���ʽ�
                    float speed = 200f * Character.WeaponSpeed;
                    weapon.speed = speed + (speed * rate);
                    break;
                default:
                    speed = 1f * Character.WeaponRate;
                    weapon.speed = speed * (1f - rate);
                    break;
            }
        }
    }

    void SpeedUp()
    {
        // �⺻ �̵��ӵ� * ĳ���� ���ʽ�
        float speed = 4 * Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
