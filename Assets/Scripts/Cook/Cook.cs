using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Cook : Item
{
    [SerializeField]
    string cookName;    // �丮 �̸�
    [SerializeField]
    string cookDescription; // �丮 ����
    [SerializeField]
    public List<Ingredient> needIngredients = new List<Ingredient>();   // �丮�� ���Ǵ� ����
    [SerializeField]
    public List<int> needIngredientsNum = new List<int>();  // �丮�� ���Ǵ� ������ ����
    [SerializeField]
    Sprite sprite;  // �丮 �̹���

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
    {   // �丮�� ����µ� �־ �ʿ��� ����� ����
        string info = "";
        for(int i = 0; i < needIngredients.Count; i++)
        {
            info += needIngredients[i].GetName() + " : " + needIngredients[i].GetAmount() + "/" + needIngredientsNum[i] +"\n";
        }

        return info;
    }

    public bool CheckMake()
    {   // �丮�� ���� �� �ִ��� Ȯ���ϴ� �Լ�
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
    {   // �丮�� ����� �Ǹ� �Һ�Ǵ� ���� (���� ��� ���� - �丮�� ���̴� ��� ����)
        for(int i = 0; i < needIngredients.Count; i++)
        {
            needIngredients[i].SetAmount(needIngredients[i].GetAmount() - needIngredientsNum[i]);
        }
    }
}
