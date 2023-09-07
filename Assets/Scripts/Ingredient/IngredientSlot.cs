using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSlot : MonoBehaviour
{
    [SerializeField]
    private GameObject buyButton;  // 구매 버튼
    [SerializeField]
    private GameObject soldOutButton;   // 품절 버튼
    [SerializeField]
    private Image image;    // 판매 재료 이미지
    [SerializeField]
    private TMP_Text name_Text; // 판매 재료 이름 텍스트
    [SerializeField]
    private TMP_Text num_Text;  // 판매 재료 수량 텍스트
    [SerializeField]
    private GameObject discountOBJ;
    [SerializeField]
    private TMP_Text discount_Text; //  판매 재료 할인률 텍스트
    [SerializeField]
    private TMP_Text gold_Text; // 판매 재료 가격 텍스트
    [SerializeField]
    private Ingredient ingredientInfo;  // 판매 재료 데이터
    [SerializeField]
    public int sell_Count;  // 판매 재료 개수
    [SerializeField]
    public int discound_Percent = 0;    // 판매 재료 할인률
    [SerializeField]
    public bool canBuy;    // 구매가 가능한가
    [SerializeField]
    private int price = 0;  // 가격

    public void SetImage(Sprite sprite) // 이미지 적용 -> 재료 상점 슬롯 및 재료 도감 슬롯
    {
        image.sprite = sprite;
    }

    public void SetName(string name)    // 재료 이름 적용 -> 재료 상점 슬롯 및 재료 도감 슬롯
    {
        name_Text.text = name;
    }

    public void SetNum()    // 재료 개수 적용 -> 재료 상점 슬롯
    {
        num_Text.text = sell_Count.ToString();
    }

    public void BookTextNum()   // 재료 보유 개수 적용 -> 재료 도감 슬롯
    {
        num_Text.text = ingredientInfo.GetAmount() + "개";
    }

    public void SetGold()   // 판매 총 가격 -> 재료 상점 슬롯 (할인률을 적용한 판매 가격 * 판매 개수)
    {
        price = (ingredientInfo.Cost - (ingredientInfo.Cost * discound_Percent / 100)) * sell_Count;
        gold_Text.text = price.ToString();
    }

    public void SetDis()    // 할인 정보 표시 -> 재료 상점 슬롯 (할인률이 0퍼이면 아이콘 표시 x)
    {
        if (discound_Percent == 0) discountOBJ.SetActive(false);
        else
        {
            discount_Text.text = discound_Percent + "%";
            discountOBJ.SetActive(true);
        }
    }

    public void SetInfo(Ingredient Info)    // 정보 세팅 -> 재료 도감 슬롯
    {
        ingredientInfo = Info;
        //SetImage(ingredientInfo.GetSprite());
        SetName(ingredientInfo.GetName());
    }

    public void SetInfo(Ingredient Info, int cnt, int dis, bool isSell) // 판매 정보 세팅 -> 재료 상점 슬롯
    {
        ingredientInfo = Info;
        sell_Count = cnt;
        discound_Percent = dis;
        canBuy = isSell;

        SetImage(ingredientInfo.GetSprite());
        SetName(ingredientInfo.GetName());
        SetGold();
        SetDis();
        SetNum();

        if (canBuy) { buyButton.SetActive(true); soldOutButton.SetActive(false); }
        else { buyButton.SetActive(false); soldOutButton.SetActive(true); }
    }

    public Ingredient GetInfo() // 현재 슬롯 재료 정보 가져오기
    {
        return ingredientInfo;
    }

    public void Buy_Ingredient()    // 구매 버튼 함수
    {
        if (GameManager.gm.gold >= price && canBuy)
        {
            ingredientInfo.SetAmount(ingredientInfo.GetAmount() + sell_Count);
            GameManager.gm.gold -= price;
            canBuy = false;
            buyButton.SetActive(false);
            soldOutButton.SetActive(true);
            InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();
            GameManager.gm.buyCnt++;
            UserDataManager.userDataManager.SaveData();
        }
    }
}