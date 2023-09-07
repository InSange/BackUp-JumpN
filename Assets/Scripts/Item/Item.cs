using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Item : MonoBehaviour
{   // �� ó�� ������� Item ��� Ŭ������ ĳ����, ��, ���, �丮���� �⺻ �������� �ۼ����־����ϴ�. �������� ���� ����
    public string keyID = "";
    public string type = ""; // Character, Map, Ingredients, Cook
    public string UnlockCondition = "";
    public int Cost = 0;
    public bool isUnlocked = false;
    public bool isPurchased = false;

    [SerializeField]
    public GameObject go_UnlockItem;
    [SerializeField]
    public TMP_Text UnlockItem_CostText;

    [SerializeField]
    public GameObject go_Lock;

    protected int Stage_Id = 0;

    public virtual void init()
    {

    }

    public virtual void UpdateDataForCharacter()
    {
        type = "Character";
    }

    public  virtual void UpdateDataForMap()
    {
        type = "Map";
    }

    public virtual void UpdateDataForIngredient()
    {
        type = "Ingredient";
    }

    public virtual void UpdateDataForCook()
    {
        type = "Cook";
    }

    public virtual void UseItem()
    {
    }

    public virtual void checkUnlockQualify()
    {
    }

    public virtual void TurnToUnlockItem()
    {
    }

    public virtual void SetCost()
    {
    }

    public string GetkeyID()
    {
        return keyID;
    }
}
