using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookSlot : MonoBehaviour
{   // 요리에 필요한 재료들을 나타내는 슬롯
    public Image ingredientImage;   // 필요한 재료 이미지
    public TMP_Text ingredientInfoText; // 필요한 재료 텍스트

    public void SetImage(Sprite sprite) // 재료 이미지 세팅
    {
        ingredientImage.sprite = sprite;
    }

}
