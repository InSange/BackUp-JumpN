using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// 맵 상점에서 사용되는 맵 버튼
public class MapSlot : MonoBehaviour
{
    [SerializeField]
    GameObject lockImage;   // 잠금 상태 이미지 오브젝트
    [SerializeField]
    int mapNum; // 해당 슬롯이 가리키는 맵 넘버
    [SerializeField]
    TMP_Text mapName;   // 슬롯 맵 이름
    [SerializeField]
    public TMP_Text lock_text;  // 잠겼지롱~ 표시
    [SerializeField]
    GameObject Non_Select_Image;    // 선택하지 않은 맵 이미지

    public void Click()
    {
        if(ItemStatusManager.itemStatusManager.maps[mapNum].isPurchased) // 맵 슬롯을 클릭했을 때 이미 맵을 구매한 상태라면?
        {
            InGameManager.inGameManager.mapShopManager.EquipUI(mapNum); // 맵을 장착할건지 안할건지 확인하는 UI세팅
            return;
        }
        else if(!ItemStatusManager.itemStatusManager.maps[mapNum].isPurchased)  // 구매하지 않은 상태라면?
        {
            InGameManager.inGameManager.mapShopManager.NeedBuyUI(mapNum);   // 구매 버튼과 취소 버튼 UI 세팅
            return ;
        }
    }

    public void SetName(string name)    // 슬롯에 부여받은 맵 정보 중 이름을 설정한다.
    {
        mapName.text = name;
    }

    public void SetNum(int num) // 슬롯에 부여받은 번호를 설정
    {   // 해당 번호는 i번째 슬롯에 대하여 접근을 용이하기 위한 번호 설정
        mapNum = num;
    }

    public int GetNum()
    {
        return mapNum;
    }

    public void SetLock()
    {
        lockImage.SetActive(true);
    }

    public void UnLock()
    {
        lockImage.SetActive(false);
    }

    public void NoSelect()
    {
        Non_Select_Image.SetActive(true);
    }

    public void Select()
    {
        Non_Select_Image.SetActive(false);
    }
}
