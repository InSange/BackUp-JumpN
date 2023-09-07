using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Cook : Item
{
    [SerializeField]
    string cookName;    // 요리 이름
    [SerializeField]
    string cookDescription; // 요리 설명
    [SerializeField]
    public List<Ingredient> needIngredients = new List<Ingredient>();   // 요리에 사용되는 재료들
    [SerializeField]
    public List<int> needIngredientsNum = new List<int>();  // 요리에 사용되는 재료들의 개수
    [SerializeField]
    Sprite sprite;  // 요리 이미지

    public void SetName(string name)
    {
        cookName = name;
    }

    public void SetDescription(string des)
    {
        cookDescription = des;
    }

    public string GetName()
    {
        return cookName;
    }
    public string GetDescription()
    {
        return cookDescription;
    }

    public void SetSprite(Sprite spriteImage)
    {
        sprite = spriteImage;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
    public string GetIngredientInfo()
    {   // 요리를 만드는데 있어서 필요한 재료의 수량
        string info = "";
        for(int i = 0; i < needIngredients.Count; i++)
        {
            info += needIngredients[i].GetName() + " : " + needIngredients[i].GetAmount() + "/" + needIngredientsNum[i] +"\n";
        }

        return info;
    }

    public bool CheckMake()
    {   // 요리를 만들 수 있는지 확인하는 함수
        for(int i = 0; i < needIngredients.Count; i++)
        {
            if(needIngredients[i].GetAmount() < needIngredientsNum[i])
            {
                return false;
            }
        }
        
        return true;
    }

    public void ConsumeStuff()
    {   // 요리를 만들게 되면 소비되는 재료들 (현재 재료 개수 - 요리에 쓰이는 재료 개수)
        for(int i = 0; i < needIngredients.Count; i++)
        {
            needIngredients[i].SetAmount(needIngredients[i].GetAmount() - needIngredientsNum[i]);
        }
    }
}
