using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Item : MonoBehaviour
{   // 맨 처음 만들었던 Item 상속 클래스로 캐릭터, 맵, 재료, 요리들의 기본 변수들을 작성해주었습니다. 언제든지 수정 가능
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
