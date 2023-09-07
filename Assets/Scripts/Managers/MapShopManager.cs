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
    List<MapSlot> mapSlotInfo;  // �� ���� ���
    [SerializeField]
    Button buyButton;   // ���� ��ư
    [SerializeField]
    Button cancelButton;    // Ȩ ��ư
    [SerializeField]
    Button equipButton; // ������ ��ư
    [SerializeField]
    TMP_Text need_gold; // ���� �ʿ� �ݾ� �ؽ�Ʈ
    public int selectMapNum;

    void Awake()
    {   // ȭ�� ũ�⿡ ���� UI ��ġ ����
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
        selectMapNum = GameManager.gm.mapNum;   // ���̺� ���Ͽ� ����Ǿ� �ִ� �� ��ȣ ����
        GameManager.gm.current_screen = "MapShop";  // ���� ����� ��ġ�� �� ����
        EquipUI(selectMapNum);  // �����Ǿ��ִ� UI ����
        SetMapData(GameManager.gm.mapNum);  // �� ������ ����
        InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();  // ��� UI ��
    }

    void OnDisable() {
        InGameManager.inGameManager.uiManager.Gold_Top_Panel_Off(); // ��� UI ����
    }


    public bool CheckCanBuyMap()
    {
        for(int i = 0; i < ItemStatusManager.itemStatusManager.maps.Length; i++)
        {   // ��� �ִ� �ʵ��� ������ ��� �˸�ǥ�� ON
            if(ItemStatusManager.itemStatusManager.characters[i].isPurchased) continue;
            if(ItemStatusManager.itemStatusManager.characters[i].Cost <= GameManager.gm.gold) return true;
        }
        return false;   // ���� ��� �˸�ǥ�� OFF
    }

    public void Init()
    {   // �� �����͵��� �ҷ��� ���� ���Ե��� �����.
        for(int i = 0; i < ItemStatusManager.itemStatusManager.maps.Length; i++)
        {   // �� ���Ե��� �ν��Ͻ�ȭ ���Ѽ� ��ũ�Ѻ信 ��ġ�����ش�.
            GameObject map = Instantiate(ViewContent);
            map.name = ItemStatusManager.itemStatusManager.maps[i].stageName;
            map.transform.SetParent(ScrollViewContent.transform, false);
            MapSlot mapSlot = map.GetComponent<MapSlot>();

            Debug.Log(i + "�� ° �ʵ����ʹ� " + (ItemStatusManager.itemStatusManager.maps[i].isPurchased == true ? "Ʈ���Դϴ� " : "Ʈ�簡 �ƴմϴ�"));
            if(ItemStatusManager.itemStatusManager.maps[i].isPurchased)
            {   // �ش� �������� ���Ű� �Ǿ��� ���. ��� ����
                mapSlot.UnLock();
            }
            // �� ���� �̸��� ��ȣ ����
            mapSlot.SetName(ItemStatusManager.itemStatusManager.maps[i].stageName);
            mapSlot.SetNum(i);
            // �� ���� ���� ����Ʈ�� �߰�.
            mapSlotInfo.Add(mapSlot);
        }
        SetMapData(GameManager.gm.mapNum);
    }

    public void SetMapData(int mapNum)
    {   // ���� ������ �� ���Կ� ���� ��� �� ������� UI���� ����ȴ�.
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
    {   // ���Ű� �ʿ��� �� ���� UI ����
        selectMapNum = mapNum;

        equipButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        need_gold.text = ItemStatusManager.itemStatusManager.maps[mapNum].price.ToString("#,##0");

        SetMapData(mapNum);
    }

    public void EquipUI(int mapNum)
    {   // �̹� ���Ű� �Ǿ��ְ� ������ �� ������? UI ����
        selectMapNum = mapNum;

        equipButton.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        SetMapData(mapNum);
    }

    public void EquipButtonFunc()
    {   // ������ �ϰԵǸ� ����� �����ϰ� ���� �� �������� ���̺����Ͽ� �����Ѵ�.
        GameManager.gm.mapNum = selectMapNum;
        InGameManager.inGameManager.Map_Change(GameManager.gm.mapNum);
        UserDataManager.userDataManager.SaveData();
        GameManager.gm.current_screen = "MainMenu";
    }

    public void BuyButton()
    {   // ���� ��ư�� ������ ��尡 ����ϴٸ� ���� ���� ������Ʈ
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