using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapShopManager : MonoBehaviour
{
    public GameObject BottomPanel;
    [SerializeField]
    GameObject ViewContent;
    [SerializeField]
    GameObject ScrollViewContent;   
    [SerializeField]
    List<MapSlot> mapSlotInfo;  // 맵 슬롯 목록
    [SerializeField]
    Button buyButton;   // 구매 버튼
    [SerializeField]
    Button cancelButton;    // 홈 버튼
    [SerializeField]
    Button equipButton; // 장착용 버튼
    [SerializeField]
    TMP_Text need_gold; // 구매 필요 금액 텍스트
    public int selectMapNum;

    void Awake()
    {   // 화면 크기에 따른 UI 위치 조절
        float defaultVal = InGameManager.inGameManager.defaultScreenSizeX / InGameManager.inGameManager.defaultScreenSizeY;
        float curVal = InGameManager.inGameManager.screenRect.rect.width / InGameManager.inGameManager.screenRect.rect.height;
        if(curVal < defaultVal)
        {
            RectTransform bottomPanelRect = BottomPanel.GetComponent<RectTransform>();
            bottomPanelRect.sizeDelta = new Vector2(bottomPanelRect.rect.width, bottomPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
        }
    }

    void Start()
    {
        Init();
    }

    void OnEnable()
    {
        selectMapNum = GameManager.gm.mapNum;   // 세이브 파일에 저장되어 있는 맵 번호 적용
        GameManager.gm.current_screen = "MapShop";  // 현재 사용자 위치는 맵 상점
        EquipUI(selectMapNum);  // 장착되어있는 UI 세팅
        SetMapData(GameManager.gm.mapNum);  // 맵 정보들 갱신
        InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();  // 골드 UI 온
    }

    void OnDisable() {
        InGameManager.inGameManager.uiManager.Gold_Top_Panel_Off(); // 골드 UI 오프
    }


    public bool CheckCanBuyMap()
    {
        for(int i = 0; i < ItemStatusManager.itemStatusManager.maps.Length; i++)
        {   // 살수 있는 맵들이 존재할 경우 알림표시 ON
            if(ItemStatusManager.itemStatusManager.characters[i].isPurchased) continue;
            if(ItemStatusManager.itemStatusManager.characters[i].Cost <= GameManager.gm.gold) return true;
        }
        return false;   // 없을 경우 알림표시 OFF
    }

    public void Init()
    {   // 맵 데이터들을 불러와 구매 슬롯들을 만든다.
        for(int i = 0; i < ItemStatusManager.itemStatusManager.maps.Length; i++)
        {   // 맵 슬롯들을 인스턴스화 시켜서 스크롤뷰에 위치시켜준다.
            GameObject map = Instantiate(ViewContent);
            map.name = ItemStatusManager.itemStatusManager.maps[i].stageName;
            map.transform.SetParent(ScrollViewContent.transform, false);
            MapSlot mapSlot = map.GetComponent<MapSlot>();

            Debug.Log(i + "번 째 맵데이터는 " + (ItemStatusManager.itemStatusManager.maps[i].isPurchased == true ? "트루입니다 " : "트루가 아닙니다"));
            if(ItemStatusManager.itemStatusManager.maps[i].isPurchased)
            {   // 해당 맵정보가 구매가 되었을 경우. 잠금 해제
                mapSlot.UnLock();
            }
            // 맵 슬롯 이름과 번호 설정
            mapSlot.SetName(ItemStatusManager.itemStatusManager.maps[i].stageName);
            mapSlot.SetNum(i);
            // 맵 슬롯 관리 리슽트에 추가.
            mapSlotInfo.Add(mapSlot);
        }
        SetMapData(GameManager.gm.mapNum);
    }

    public void SetMapData(int mapNum)
    {   // 현재 선택한 맵 슬롯에 따라 잠금 및 잠금해제 UI들이 변경된다.
        for(int i = 0; i < mapSlotInfo.Count; i++)
        {
            if(mapNum == i && ItemStatusManager.itemStatusManager.maps[i].isPurchased)
            {
                mapSlotInfo[i].Select();
                mapSlotInfo[i].lock_text.gameObject.SetActive(false);
            }
            else if(mapNum == i && !ItemStatusManager.itemStatusManager.maps[i].isPurchased)
            {
                mapSlotInfo[i].Select();
                mapSlotInfo[i].lock_text.gameObject.SetActive(true);
            }
            else if(mapNum != i && ItemStatusManager.itemStatusManager.maps[i].isPurchased)
            {
                mapSlotInfo[i].NoSelect();
                mapSlotInfo[i].lock_text.gameObject.SetActive(false);
            }
            else if(mapNum != i && !ItemStatusManager.itemStatusManager.maps[i].isPurchased)
            {
                mapSlotInfo[i].NoSelect();
                mapSlotInfo[i].lock_text.gameObject.SetActive(false);
            }
        }
    }

    public void NeedBuyUI(int mapNum)
    {   // 구매가 필요한 맵 슬롯 UI 세팅
        selectMapNum = mapNum;

        equipButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        need_gold.text = ItemStatusManager.itemStatusManager.maps[mapNum].price.ToString("#,##0");

        SetMapData(mapNum);
    }

    public void EquipUI(int mapNum)
    {   // 이미 구매가 되어있고 장착을 할 것인지? UI 세팅
        selectMapNum = mapNum;

        equipButton.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        SetMapData(mapNum);
    }

    public void EquipButtonFunc()
    {   // 장착을 하게되면 배경을 변경하고 장착 맵 정보들을 세이브파일에 저장한다.
        GameManager.gm.mapNum = selectMapNum;
        InGameManager.inGameManager.Map_Change(GameManager.gm.mapNum);
        UserDataManager.userDataManager.SaveData();
        GameManager.gm.current_screen = "MainMenu";
    }

    public void BuyButton()
    {   // 구매 버튼을 누를시 골드가 충분하다면 구매 정보 업데이트
        if(GameManager.gm.gold >= ItemStatusManager.itemStatusManager.maps[selectMapNum].price)
        {
            GameManager.gm.gold -= ItemStatusManager.itemStatusManager.maps[selectMapNum].price;

            InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();

            ItemStatusManager.itemStatusManager.maps[selectMapNum].isPurchased = true;
            ItemStatusManager.itemStatusManager.maps[selectMapNum].isUnlocked = true;
            mapSlotInfo[selectMapNum].UnLock();

            EquipUI(selectMapNum);
            UserDataManager.userDataManager.SaveData();
        }
    }

    public void CancelButton()
    {
        GameManager.gm.current_screen = "MainMenu";
    }
}