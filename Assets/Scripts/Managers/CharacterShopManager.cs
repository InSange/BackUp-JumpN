using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class CharacterShopManager : MonoBehaviour
{
    public GameObject BottomPanel;
    [SerializeField]
    private int number = 0; // 현재 가리키고 있는 캐릭터 슬롯 인덱스

    [Header("Cat Prefab")]
    [SerializeField]
    private GameObject cat_prefab;  // 고양이 슬롯 프리팹 오브젝트
    [SerializeField]
    private GameObject viewContent; // 고양이 슬롯들이 표시될 콘텐츠 영역
    [SerializeField]
    private List<CatShopContent> catSlotInfo;   // 고양이 슬롯들 정보가 담긴 리스트

    [Header("Cat name")]
    [SerializeField]
    private TMP_Text character_name_text;   // 고양이 슬롯 이름을 나타내는 텍스트
    [SerializeField]
    private TMP_Text character_description; // 고양이 슬롯 설명을 나타내는 텍스트

    [Header("Buy Info")]
    [SerializeField]
    private GameObject[] buttons;   // 버튼 종류들 0 : 구매 버튼, 1 : 나가기 버튼, 2 : 장착 버튼
    [SerializeField]
    private TMP_Text buy_gold_text; // 구매시 필요한 가격을 표시해주는 텍스트
    [SerializeField]
    private TMP_Text warning_message;   // 돈이 부족할 때 뜨는 경고용 텍스트

    [SerializeField]
    private HorizontalScrollSnap snap;  // 스크롤시 스냅효과를 지니고있는 오브젝트
    [SerializeField]
    private GameObject[] pageButton;    // 0 : previous button, 1 : next button

    void Awake()
    {
        float defaultVal = InGameManager.inGameManager.defaultScreenSizeX / InGameManager.inGameManager.defaultScreenSizeY;
        float curVal = InGameManager.inGameManager.screenRect.rect.width / InGameManager.inGameManager.screenRect.rect.height;
        if(curVal < defaultVal)
        {
            RectTransform bottomPanelRect = BottomPanel.GetComponent<RectTransform>();
            bottomPanelRect.sizeDelta = new Vector2(bottomPanelRect.rect.width, bottomPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
        }
    }

    void LateUpdate()
    {
        if (number != snap.CurrentPage) // 가리키는 번호가 snap에서 가리키는 번호와 다를 경우.
        {
            number = snap.CurrentPage;
            Debug.Log("현재 번호 " + number);
            UpdateInfo();   // 해당 번호 슬롯으로 업데이트해준다.
            if(number == 0)
            {
                pageButton[0].SetActive(false);
                pageButton[1].SetActive(true);
            }
            else if(number == ItemStatusManager.itemStatusManager.characters.Length-1)
            {
                pageButton[0].SetActive(true);
                pageButton[1].SetActive(false);
            }
            else
            {
                pageButton[0].SetActive(true);
                pageButton[1].SetActive(true);
            }
        }
    }

    void OnEnable()
    {
        GameManager.gm.current_screen = "CharacterShop";    //  현재 플레이어가 들어가있는 곳은 캐릭터 상점

        character_name_text.text = ItemStatusManager.itemStatusManager.characters[number].character_name;   // 해당 캐릭터 슬롯 이름으로 변경

        if (ItemStatusManager.itemStatusManager.characters[number].isPurchased) // 이미 구입한 상태라면
        {   // 캐릭터 설명 및 버튼 변경
            character_description.text = ItemStatusManager.itemStatusManager.characters[number].character_des.Replace("\n", "<br>");  
            buttons[0].SetActive(false);    // 구매 버튼 OFF
            buttons[1].SetActive(false);    // 나가기 버튼 OFF
            buttons[2].SetActive(true);     // 장착 버튼 ON
        }
        else
        {   // 구입하지 않은 상태라면
            character_description.text = "??? ???"; // 정보 미개봉
            buttons[0].SetActive(true);     // 구매 버튼 ON
            buttons[1].SetActive(true);     // 나가기 버튼 ON
            buttons[2].SetActive(false);    // 장착 버튼 OFF
            buy_gold_text.text = ItemStatusManager.itemStatusManager.characters[number].Cost.ToString("#,##0"); // 캐릭터 구매에 필요한 비용
        }

        if(number == 0)
        {
            pageButton[0].SetActive(false);
            pageButton[1].SetActive(true);
        }
        else if(number == ItemStatusManager.itemStatusManager.characters.Length-1)
        {
            pageButton[0].SetActive(true);
            pageButton[1].SetActive(false);
        }
        else
        {
            pageButton[0].SetActive(true);
            pageButton[1].SetActive(true);
        }

        InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();
    }

    void OnDisable() {
        InGameManager.inGameManager.uiManager.Gold_Top_Panel_Off();
    }


    public void Init()
    {   // 상점 초기 번호는 장착하고 있는 캐릭터 번호.
        number = ItemStatusManager.itemStatusManager.character_num_dic[GameManager.gm.currentCharacter];
        // 아이템 목록에 있는 캐릭터 개수 만큼 슬롯을 생성한다.
        for (int i = 0; i < ItemStatusManager.itemStatusManager.characters.Length; i++)
        {   
            GameObject cat = Instantiate(cat_prefab);   // 슬롯 생성
            cat.name = ItemStatusManager.itemStatusManager.characters[i].character_name;    // 슬롯 이름 = 캐릭터 이름(확인용)
            cat.transform.SetParent(viewContent.transform, false);  // 해당 슬롯 위치는 scrollsnap 내부로 옮겨줌
            CatShopContent slot = cat.GetComponent<CatShopContent>();   // 슬롯에 적용할 데이터 컴포넌트 불러오기
            slot.cat.sprite = InGameManager.inGameManager.player.cat_skin.skinSet.catSkins[i].CatSprites[0];    // 슬롯 이미지 적용 캐릭터
            slot.board.sprite = InGameManager.inGameManager.player.cat_skin.skinSet.catSkins[i].BoardSprites[0];    // 슬롯 이미지 적용 보드
            if (ItemStatusManager.itemStatusManager.characters[i].isPurchased)  // 이미 구매한 상태라면
            {   // 캐릭터 모습 보여주기
                slot.cat.color = new Color(1f, 1f, 1f);
                slot.board.color = new Color(1f, 1f, 1f);
            }
            else
            {   // 구매하지 않은 상태라면
                // 그림자 모양
                slot.cat.color = new Color(0, 0, 0);
                slot.board.color = new Color(0, 0, 0);
            }
            catSlotInfo.Add(slot);  // 리스트에 업데이트한 슬롯 추가
        }

        snap.StartingScreen = number;   
    }

    public bool CheckCanBuyCat()    // 구매할 수 있는 캐릭터가 존재하는지 확인하는 함수
    {   // 구매 가능 캐릭터가 있을 시 메인화면에 알림 표시
        for (int i = 0; i < ItemStatusManager.itemStatusManager.characters.Length; i++)
        {
            if (ItemStatusManager.itemStatusManager.characters[i].isPurchased) continue;
            if (ItemStatusManager.itemStatusManager.characters[i].Cost <= GameManager.gm.gold) return true;
        }
        return false;
    }

    public void Next_Button()   // 다음 캐릭터 버튼
    {
        number++;   // 번호 증가. 번호가 전체 캐릭터 개수보다 크거나 같을시 0으로 초기화
        // 캐릭터 이름 업데이트
        character_name_text.text = ItemStatusManager.itemStatusManager.characters[number].character_name;//cats[number].character_name;
        // 캐릭터 버튼 업데이트
        if (ItemStatusManager.itemStatusManager.characters[number].isPurchased)
        {
            character_description.text = ItemStatusManager.itemStatusManager.characters[number].character_des.Replace("\n", "<br>");  
            buttons[0].SetActive(false);
            buttons[1].SetActive(false);
            buttons[2].SetActive(true);
        }
        else
        {
            character_description.text = "??? ???";
            buttons[0].SetActive(true);
            buttons[1].SetActive(true);
            buttons[2].SetActive(false);
            buy_gold_text.text = ItemStatusManager.itemStatusManager.characters[number].Cost.ToString("#,##0");
        }
        if(pageButton[0].activeSelf == false) pageButton[0].SetActive(true);
        if(number >= ItemStatusManager.itemStatusManager.characters.Length-1) pageButton[1].SetActive(false);
    }

    public void Previous_Button()   // 이전 캐릭터 버튼
    {
        number--;   // 번호 감소. 번호가 0보다 작을시 전체 캐릭터 개수에서 마지막으로 초기화
        if (number < 0) number = ItemStatusManager.itemStatusManager.characters.Length - 1;
        // 캐릭터 이름 업데이트
        character_name_text.text = ItemStatusManager.itemStatusManager.characters[number].character_name;//cats[number].character_name;
        // 캐릭터 버튼 업데이트
        if (ItemStatusManager.itemStatusManager.characters[number].isPurchased)
        {
            character_description.text = ItemStatusManager.itemStatusManager.characters[number].character_des.Replace("\n", "<br>");  
            buttons[0].SetActive(false);
            buttons[1].SetActive(false);
            buttons[2].SetActive(true);
        }
        else
        {
            character_description.text = "??? ???";
            buttons[0].SetActive(true);
            buttons[1].SetActive(true);
            buttons[2].SetActive(false);
            buy_gold_text.text = ItemStatusManager.itemStatusManager.characters[number].Cost.ToString("#,##0");
        }
        if(pageButton[1].activeSelf == false) pageButton[1].SetActive(true);
        if(number <= 0) pageButton[0].SetActive(false);
    }

    public void Buy()
    {   // 구매시
        if (GameManager.gm.gold < ItemStatusManager.itemStatusManager.characters[number].Cost)
        {   // 플레이어가 소지하고 있는 골드가 구매하고자하는 비용보다 적을 경우 경고 알림창
            StopCoroutine(WarningMessage());
            IEnumerator i = WarningMessage();
            StartCoroutine(i);
        }
        else
        {   // 소지비용이 충분할 경우. 비용 만큼 경감
            GameManager.gm.gold -= ItemStatusManager.itemStatusManager.characters[number].Cost;
            InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();
            ItemStatusManager.itemStatusManager.characters[number].isPurchased = true;  // 구매 완료 표시
            buttons[0].SetActive(false);    // 구매 버튼 OFF
            buttons[1].SetActive(false);    // 나가기 버튼 OFF
            buttons[2].SetActive(true);     // 장착 버튼 ON
            catSlotInfo[number].cat.color = new Color(1f, 1f, 1f);  // 캐릭터 및 보드 색상 ON
            catSlotInfo[number].board.color = new Color(1f, 1f, 1f);
            character_description.text = ItemStatusManager.itemStatusManager.characters[number].character_des.Replace("\n", "<br>");  

            UserDataManager.userDataManager.SaveData(); // 구매한 데이터 저장
        }
    }

    public void UpdateInfo()
    {   // 현재 슬롯 번호를 바탕으로 이름, 버튼들을 업데이트해준다.
        character_name_text.text = ItemStatusManager.itemStatusManager.characters[number].character_name;//cats[number].character_name;

        if (ItemStatusManager.itemStatusManager.characters[number].isPurchased)
        {
            character_description.text = ItemStatusManager.itemStatusManager.characters[number].character_des.Replace("\n", "<br>");  
            buttons[0].SetActive(false);
            buttons[1].SetActive(false);
            buttons[2].SetActive(true);
        }
        else
        {
            character_description.text = "??? ???";
            buttons[0].SetActive(true);
            buttons[1].SetActive(true);
            buttons[2].SetActive(false);
            buy_gold_text.text = ItemStatusManager.itemStatusManager.characters[number].Cost.ToString("#,##0");
        }
    }

    public void PutOnSkin() // 스킨 장착 버튼
    {
        snap.StartingScreen = number;   // 시작 화면을 장착 번호로 변경
        GameManager.gm.currentCharacter = ItemStatusManager.itemStatusManager.characters[number].character_name;    // 현재 장착중인 캐릭터로 변경
        InGameManager.inGameManager.player.SetSkinNum();    // 스킨 번호 세팅
        UserDataManager.userDataManager.SaveData(); // 변경된 사항 저장
        GameManager.gm.current_screen = "MainMenu"; // 메인 화면으로 이동
    }

    public void CancelButton()
    {   // 나가기 버튼
        GameManager.gm.current_screen = "MainMenu";
    }

    IEnumerator WarningMessage()
    {
        warning_message.text = "돈이 부족합니다.";
        yield return new WaitForSeconds(1.5f);
        warning_message.text = "";
    }
}