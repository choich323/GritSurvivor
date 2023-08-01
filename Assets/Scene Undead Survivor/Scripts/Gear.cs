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
                // 공격 속도 증가
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                // 이동 속도 증가
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
                    // 기본 공격속도 * 캐릭터 보너스
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
        // 기본 이동속도 * 캐릭터 보너스
        float speed = 4 * Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
