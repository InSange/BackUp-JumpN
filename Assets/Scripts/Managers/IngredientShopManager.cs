using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientShopManager : MonoBehaviour
{
    public GameObject BottomPanel;
    [Header("재료 목록")]
    [SerializeField]
    TMP_Text time_text; // 현재 시간 바탕으로 갱신 이루어짐.
    [SerializeField]
    private List<IngredientSlot> ingredientSlots = new();  // 판매 목록 슬롯들

    int min_set = 0;    // 최소 개수
    int max_set = 0;    // 최대 개수

    [Header("경고 창")]
    [SerializeField]
    Image warn_image;   // 돈이 부족하면 뜨는 경고창
    [SerializeField]
    Image alert_no_material_image;  // 재료가 소진되었으면 뜨는 경고창  

    [Header("상점 출입 시간")]
    [SerializeField]
    public string Last_in_time; // 마지막 상점 입장 시간

    [Header("상점 할머니 고양이 캐릭터")]
    [SerializeField]
    GameObject chatBubble;  // 상점 할머니 말풍선
    [SerializeField]
    TMP_Text chatText;  // 상점 할머니 텍스트
    [SerializeField]
    string friendly = "";   // 친밀도 단계
    [SerializeField]
    List<string> textContents = new List<string>(); // 상점 할머니 대화 목록들
    [SerializeField]
    List<int> disCounttyps = new List<int>();   // 상점 할머니 할인률 목록들
    [SerializeField]
    List<int> touchCountLevel = new List<int>() { 30, 80, 150, 250, 500 };  // 상점 할머니 터치 횟수 친밀도.

    void Start()
    {
        IngredientDataInit();
        //Last_in_time = GameManager.gm.playerData.Last_ingredientShop_intime;
        float defaultVal = InGameManager.inGameManager.defaultScreenSizeX / InGameManager.inGameManager.defaultScreenSizeY;
        float curVal = InGameManager.inGameManager.screenRect.rect.width / InGameManager.inGameManager.screenRect.rect.height;
        if(curVal < defaultVal)
        {
            RectTransform bottomPanelRect = BottomPanel.GetComponent<RectTransform>();
            bottomPanelRect.sizeDelta = new Vector2(bottomPanelRect.rect.width, bottomPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
        }
    }

    void OnEnable() // 재료 상점에 들어오면?
    {
        GameManager.gm.current_screen = "IngredientShop";   // 현재 화면 위치 재료상점
        Last_in_time = NTP.ntp.getServerTime().ToString();  // 마지막 상점 출입 시간 저장
        UserDataManager.userDataManager.SaveData(); // 세이브
        Renew_Ingredient_Time();    // 상점 갱신 시간 확인
        InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();
        ChangeCNT();
    }

    void OnDisable() {
        InGameManager.inGameManager.uiManager.Gold_Top_Panel_Off();
    }

    public void GetStuffData()
    {   // 이전 상점에서 팔던 재료들에 대한 데이터들을 불러오는 함수. 팔고있던 재료 이름, 가격, 할인률
        Debug.Log("플레이어 데이터 상점 출입시간 " + GameManager.gm.playerData.Last_ingredientShop_intime);
        Last_in_time = GameManager.gm.playerData.Last_ingredientShop_intime;
        IngredientDataInit();
    }
    
    // 재료데이터 초기 세팅
    public void IngredientDataInit()
    {   // 상점 할머니 데이터 세팅.
        SetFriendlyData();

        if (GameManager.gm.shop_need_chage)
        {   // 갱신이 필요할 경우 상점 데이터 판매 목록들을 다시 갱신해준다.  
            RenewIngredientShop();
            return;
        }
        for (int i = 0; i < 6; i++)
        {
            // 슬롯에 기존 데이터 적용(재료 정보, 판매 개수, 할인률)
            ingredientSlots[i].SetInfo(ItemStatusManager.itemStatusManager.ingredient_dic[GameManager.gm.playerData.stuffShopInfo.stuffKeys[i]],
                                        GameManager.gm.playerData.stuffShopInfo.stuffSellCnts[i], // 이전 판매 개수를 설정
                                        GameManager.gm.playerData.stuffShopInfo.stuffDisRates[i],    // 이전 판매 할인률을 설정
                                        GameManager.gm.playerData.stuffShopInfo.stuffCanBuys[i]);
        }

    }
    // 상점 갱신 시간을 계산하는 함수
    public void Renew_Ingredient_Time()
    {   // NTP에서 시간을 가져와서 정수형으로 변경.
        int curTime = int.Parse(NTP.ntp.getServerTime().ToString("HHmmss"));
        int cur_m = curTime / 100 % 100;    // 현재 시각의 분
        int cur_s = curTime % 100;    // 현재 시각의 초
        int remain_time = 1800 - (cur_m * 60) - cur_s;    // 30분 마다 상점이 기준되기 때문에 1800초에서 분과 초를 빼준다

        time_text.text = "다음 갱신까지\n" + (remain_time < 0 ? (remain_time + 1800) / 60 : remain_time / 60) + ":" + (remain_time < 0 ? (remain_time + 1800) % 60 : remain_time % 60) + "초 남음";
        if (remain_time == 0)    // 남은 시간이 0일 경우
        {
            RenewIngredientShop();  // 상점 재료를 갱신해준다.
        }
    }
    // 상점 재료들을 갱신해주는 함수
    public void RenewIngredientShop()
    {   // 재료를 갱신해야하기 때문에 모든 칸 초기화(재료, 가격, 할인률).
        Debug.Log("저는 재료상점 데이터들을 갱신해야 겠어욧!");

        int sell_num;   // 판매 개수
        int random_num; // 판매하고자하는 재료 번호
        for (int i = 0; i < 6; i++)
        {   // 재료들 중 하나를 택해서 재료 등급에 따른 판매 확률보다 높을 경우 해당 재료를 넣어준다.
            random_num = Random.Range(0, ItemStatusManager.itemStatusManager.ingredients.Length);
            if (Random.value > ItemStatusManager.itemStatusManager.ingredients[random_num].GetProbability())
            {   // 그게 아니라면 인덱스를 감소 시켜서 다시 한번 적용시켜준다.
                i--;
                continue;
            }
            sell_num = Random.Range(min_set, max_set + 1);    // 판매 개수를 최소 값에서 최대 값 중 랜덤으로 지정

            ingredientSlots[i].SetInfo(ItemStatusManager.itemStatusManager.ingredients[random_num],// random_num에 해당하는 재료 정보를 슬롯에 업데이트해준다.
                                        sell_num,   // 슬롯에 판매 개수를 업데이트해준다.
                                        disCounttyps[Random.Range(0, disCounttyps.Count)],    // 슬롯에 할인률을 업데이트해준다.
                                        true);
        }
    }
    // 현재 상점 데이터들을 세이브 파일로 전달해주는 함수.
    public StuffShopInfo GetStuffShopInfo()
    {
        StuffShopInfo info = new(ingredientSlots);
        return info;
    }

    public void SetFriendlyData()
    {   // 상점할머니 데이터 추가해주는 함수.
        Dictionary<string, List<string>> langData = GameManager.gm.dataLoader.localData;    // 상점에 대한 데이터를 들고온다.
        textContents.Clear();   // 상점 할머니 대사를 저장해주는 리스트를 초기화.
        disCounttyps.Clear();   // 상점 할머니가 제공해주는 할인률에 대한 리스트를 초기화.
        if (GameManager.gm.ingredient_char_touch < 30)   // 30보다 작으면 친함 0단계
        {
            friendly = "친함0";
        }
        else if (GameManager.gm.ingredient_char_touch < 80)  // 80보다 작으면 친함 1단계
        {
            friendly = "친함1";
        }
        else if (GameManager.gm.ingredient_char_touch < 150) // 150보다 작으면 친함 2단계
        {
            friendly = "친함2";
        }
        else if (GameManager.gm.ingredient_char_touch < 250) // 250보다 작으면 친함 3단계
        {
            friendly = "친함3";
        }
        else if (GameManager.gm.ingredient_char_touch < 500) // 500보다 작으면 친함 4단계
        {
            friendly = "친함4";
        }
        else if (GameManager.gm.ingredient_char_touch >= 500)    // 500이상일시 친함 5단계
        {
            friendly = "친함5";
        }

        for (int i = 2; i < 10; i++)
        {   // 각 단계에 따른 상점할머니 대사와 할인률들을 초기화해준 리스트에 추가해준다.
            if (i < 6 && GameManager.gm.dataLoader.storeData[friendly][i] != "-1") textContents.Add(langData[GameManager.gm.dataLoader.storeData[friendly][i]][(int)GameManager.gm.countryLang]);
            else if (i >= 6 && GameManager.gm.dataLoader.storeData[friendly][i] != "-1") disCounttyps.Add(int.Parse(GameManager.gm.dataLoader.storeData[friendly][i]));
        }
        min_set = int.Parse(GameManager.gm.dataLoader.storeData[friendly][10]); // 최소 판매 개수
        max_set = int.Parse(GameManager.gm.dataLoader.storeData[friendly][11]); // 최대 판매 개수
    }

    public void ClickCharacter()
    {   // 상점할머니를 클릭했을 때
        IEnumerator cor = ChatNPC();    // 대사 출력
        StopAllCoroutines();    // 실행중인 코루틴 정지
        StartCoroutine(cor);    // 대사 출력
    }

    public IEnumerator Need_gold()
    {   // 골드가 부족합니다 경고창
        warn_image.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        warn_image.gameObject.SetActive(false);
    }

    public IEnumerator OutOfMaterial()
    {   // 재료가 소진되었다는 경고창
        alert_no_material_image.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        alert_no_material_image.gameObject.SetActive(false);
    }

    public IEnumerator ChatNPC()
    {   // NPC가 말하는 기능
        GameManager.gm.ingredient_char_touch++;
        ChangeCNT();
        if (touchCountLevel.Contains(GameManager.gm.ingredient_char_touch)) SetFriendlyData();
        chatText.text = textContents[Random.Range(0, textContents.Count)];

        chatBubble.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        chatBubble.gameObject.SetActive(false);
    }

    //------------------------------------------------------------------ 테스트 기능 ------------------------------------------------------------------------
    [SerializeField]
    private TMP_Text touchCntText;

    public void ChangeFriendly(int n)
    {
        GameManager.gm.ingredient_char_touch = n;
        SetFriendlyData();
        ChangeCNT();
    }

    public void ChangeCNT()
    {
        touchCntText.text = "터치 횟수 : " + GameManager.gm.ingredient_char_touch;
    }

}