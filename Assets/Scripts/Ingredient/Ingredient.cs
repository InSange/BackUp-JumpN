using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Ingredient : Item
{
    [SerializeField]
    int amount = 0; // ��� ����
    [SerializeField]
    string ingredientName;  // ��� �̸�
    [SerializeField]
    Sprite sprite;  // ��� �̹���
    [SerializeField]
    float sales_probability;    // ���η�

    public void SetAmount(int n)
    {   // ��� ������ ����� ��� -> �����ϰų� �丮�� ���� �Ҹ����� �� ���� ������Ʈ ���
        amount = n;
    }
    public int GetAmount()
    {   // ��� ���� ���� ��������
        return amount;
    }

    public void SetProbability(float num)
    {   // ���η� �����ϱ�
        sales_probability = num;
    }
    public float GetProbability()
    {   // ���η� ������ ��������
        return sales_probability;
    }

    public string GetName()
    {   // ��� �̸� ��������
        return ingredientName;
    }
    public void SetName(string name)
    {   // ��� �̸� �����ϱ�
        ingredientName = name;
        gameObject.name = name;
    }
    public Sprite GetSprite()
    {   // ��� �̹��� �������� -> ��� ����, ��� ����
        return sprite;
    }
}
