using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Ingredient : Item
{
    [SerializeField]
    int amount = 0; // 재료 수량
    [SerializeField]
    string ingredientName;  // 재료 이름
    [SerializeField]
    Sprite sprite;  // 재료 이미지
    [SerializeField]
    float sales_probability;    // 할인률

    public void SetAmount(int n)
    {   // 재료 수량이 변경될 경우 -> 구매하거나 요리를 만들어서 소모했을 때 수량 업데이트 기능
        amount = n;
    }
    public int GetAmount()
    {   // 재료 현재 수량 가져오기
        return amount;
    }

    public void SetProbability(float num)
    {   // 할인률 설정하기
        sales_probability = num;
    }
    public float GetProbability()
    {   // 할인률 데이터 가져오기
        return sales_probability;
    }

    public string GetName()
    {   // 재료 이름 가져오기
        return ingredientName;
    }
    public void SetName(string name)
    {   // 재료 이름 설정하기
        ingredientName = name;
        gameObject.name = name;
    }
    public Sprite GetSprite()
    {   // 재료 이미지 가져오기 -> 재료 상점, 재료 도감
        return sprite;
    }
}
