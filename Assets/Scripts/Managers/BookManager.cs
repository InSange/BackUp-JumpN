using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public GameObject TopPanel;
    public GameObject BottomPanel;
    [Header("Cook")]
    [SerializeField]
    private int cookNum = 0;    // 현재 요리 번호
    [SerializeField]
    private TMP_Text cookNameText;  // 현재 요리 이름
    [SerializeField]
    Image cookImage;                // 현재 요리 이미지
    [SerializeField]
    GameObject[] pageButton;        // 요리 페이지 버튼 0 : previous button, 1 : next button
    [SerializeField]
    GameObject[] buttonSet;         // 버튼 세트 (요리하기 버튼 세트{요리하기, 확인}, 나가기 버튼 세트{확인})
    [SerializeField]
    Image warn_image;               // 경고 이미지
    [SerializeField]
    GameObject canMakeAlertIcon;
    [SerializeField]
    TMP_Text cookEvalText;          // 요리 평가 텍스트
    [SerializeField]
    GameObject stuffsView;          // 재료 확인란 오브젝트
    [SerializeField]
    GameObject descView;            // 요리 설명란 오브젝트
    [SerializeField]
    GameObject cookSlotPrefab;      // 요리 슬롯 프리팹
    [SerializeField]
    GameObject cookViewContent;     // 요리 슬롯이 들어갈 위치
    [SerializeField]
    List<CookSlot> cookSlotContents = new List<CookSlot>(); // 모든 요리 슬롯콘텐츠들

    [SerializeField]
    Scrollbar cookScrollbar;        // 스크롤바
    [SerializeField]
    int scrollNum;                  // 스크롤 번호
    [SerializeField]
    TMP_Text cookNumText;           // 현재 스크롤 번호 / 전체 요리 개수
    [SerializeField]
    int cookLength;                 // 전체 요리 개수

    [Header("요리 판넬")]
    [SerializeField]
    private GameObject cookPanel;   // 요리 시작
    [SerializeField]
    private GameObject cookingPanel;    // 요리중
    [SerializeField]
    private GameObject cookresultPanel; // 요리 결과
    [SerializeField]
    private TMP_Text cooking_text;  // 요리중 text
    [SerializeField]
    private string[] cooking_texts = {"엄마\n배고파요\n(요리하는 중.)", "엄마\n배고파요\n(요리하는 중..)", "엄마\n배고파요\n(요리하는 중...)"}; // 나중에 시트 데이터로 변경해주세요.
    [SerializeField]
    WaitForSeconds cooking_time = new WaitForSeconds(0.2f); // 요리 애니메이션 시간
    [SerializeField]
    private TMP_Text rewardCookNameText;    // 요리 보상 이름
    [SerializeField]
    private TMP_Text rewardCookDescText;    // 요리 설명 이름
    [SerializeField]
    private Image rewardCookImage;  // 요리 결과 이미지
    [SerializeField]
    int pageNum = 0;    // 요리 도감 현재 페이지 번호 -> 스크롤 번호

    [Header("Ingredient")]
    [SerializeField]
    GameObject IngredientSlotPrefab;    // 재료 도감 슬롯 프리팹
    [SerializeField]
    GameObject ViewContent; // 프리팹을 넣을 뷰 콘텐츠
    [SerializeField]
    List<IngredientSlot> ingredientSlotInfo;    // 재료 도감 슬롯을 관리할 리스트

    void Awake()
    {   // 화면 크기 조절
        float defaultVal = InGameManager.inGameManager.defaultScreenSizeX / InGameManager.inGameManager.defaultScreenSizeY;
        float curVal = InGameManager.inGameManager.screenRect.rect.width / InGameManager.inGameManager.screenRect.rect.height;
        if(curVal < defaultVal)
        {
            RectTransform bottomPanelRect = BottomPanel.GetComponent<RectTransform>();
            bottomPanelRect.sizeDelta = new Vector2(bottomPanelRect.rect.width, bottomPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
            RectTransform topPanelRect = TopPanel.GetComponent<RectTransform>();
            topPanelRect.sizeDelta = new Vector2(topPanelRect.rect.width, topPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
        }
    }

    void OnEnable()
    {   // 플레이어 현재 화면 
        GameManager.gm.current_screen = "BookShop";
        cookNum = scrollNum;    // scrollNum값으로 스크롤뷰 위치를 변경한다.
        if(cookNum == 0)    // 첫 번째 페이지라면
        {   
            pageButton[0].SetActive(false); // 이전 페이지 버튼 OFF
            pageButton[1].SetActive(true);  // 다음 페이지 버튼 ON
        }
        else if(cookNum == cookLength-1)    // 마지막 페이지라면
        {
            pageButton[0].SetActive(true);  // 이전 페이지 버튼 ON
            pageButton[1].SetActive(false); // 다음 페이지 버튼 OFF
        }
        else
        {   // 그 외는 이전, 다음 페이지 버튼 ON
            pageButton[0].SetActive(true);  
            pageButton[1].SetActive(true);
        }
        IngredientDataUpdate();
        CookDataUpdate(cookNum);
        cookNumText.text = (cookNum+1) + "/" + cookLength +"p"; // 현재 페이지
    }

    public void Init()
    {   // 재료 슬롯 초기화
        ingredientSlotInfo = new List<IngredientSlot>();
        CookDataUpdate(0);
        IngredientDataInit();
        pageNum=0;
        cookScrollbar.value = pageNum;
        cookScrollbar.onValueChanged.AddListener((float val) => ScrollbarCallback(val));    // 값이 바뀔때 변경 된 값을 적용하는 기능 추가
        cookLength = ItemStatusManager.itemStatusManager.cooks.Length;  // 요리 개수
    }
    // 요리 가능한 요리 확인후 알림
    public bool CheckCanReceiveCook()
    {
        for(int i = 0; i < cookLength; i++)
        {
            if(ItemStatusManager.itemStatusManager.cooks[i].isPurchased) continue;
            if(ItemStatusManager.itemStatusManager.cooks[i].CheckMake()) return true;
        }
        return false;
    }
    // 요리 데이터 업데이트
    public void CookDataUpdate(int num)
    {   // 요리가 구매되었으면 요리 설명과 UI를 바꿔준다.
        cookNameText.text = "엄마의<br>" + ItemStatusManager.itemStatusManager.cooks[num].GetName();
        if(ItemStatusManager.itemStatusManager.cooks[num].isPurchased)
        {
            cookEvalText.text = ItemStatusManager.itemStatusManager.cooks[num].GetDescription();
            stuffsView.gameObject.SetActive(false);
            descView.gameObject.SetActive(true);
            buttonSet[0].SetActive(false);
            buttonSet[1].SetActive(true);
        }
        else    // 구매 되지 않은 상태라면 요구하는 재료 UI 바꿔준다.
        {
            //cookDesText.text = ItemStatusManager.itemStatusManager.cooks[num].GetIngredientInfo();
            bool flag = updateCookInfo(num);
            descView.gameObject.SetActive(false);
            stuffsView.gameObject.SetActive(true);
            buttonSet[0].SetActive(true);
            buttonSet[1].SetActive(false);
            if(flag) canMakeAlertIcon.SetActive(true);
            else canMakeAlertIcon.SetActive(false);
        }
        //cookImage.sprite = ItemStatusManager.itemStatusManager.cooks[num].GetSprite();
    }
    // 요리 정보 업데이트
    public bool updateCookInfo(int num)
    {
        bool canMake = true;
        for(int i = 0; i < ItemStatusManager.itemStatusManager.cooks[num].needIngredients.Count; i++)
        {   // 요리만들 수 있는 재료들을 표시함. 요구하는 재료 개수보다 많을 시 슬롯을 끄고, 적을 시 슬롯들을 더 추가해주는 방식으로 제작됨.
            if(canMake && ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetAmount() < ItemStatusManager.itemStatusManager.cooks[num].needIngredientsNum[i]) canMake = false;
            if(cookSlotContents.Count <= i)
            {
                GameObject cook = Instantiate(cookSlotPrefab);
                cook.name = ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetName();
                cook.transform.SetParent(cookViewContent.transform, false);
                CookSlot slot = cook.GetComponent<CookSlot>();
                //slot.SetImage(ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetSprite());
                slot.ingredientInfoText.text = ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetName() + " : " + 
                 (ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetAmount() < ItemStatusManager.itemStatusManager.cooks[num].needIngredientsNum[i] ? "<#FF4248>" +ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetAmount() + "</color>" + "/"  : ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetAmount() + "/")
                 + ItemStatusManager.itemStatusManager.cooks[num].needIngredientsNum[i];

                cookSlotContents.Add(slot);
            }
            else
            {
                if(cookSlotContents[i].gameObject.activeSelf == false) cookSlotContents[i].gameObject.SetActive(true);
                cookSlotContents[i].SetImage(ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetSprite());
                cookSlotContents[i].ingredientInfoText.text = ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetName() + " : " + 
                 (ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetAmount() < ItemStatusManager.itemStatusManager.cooks[num].needIngredientsNum[i] ? "<#FF4248>" +ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetAmount() + "</color>" + "/"  : ItemStatusManager.itemStatusManager.cooks[num].needIngredients[i].GetAmount() + "/")
                 + ItemStatusManager.itemStatusManager.cooks[num].needIngredientsNum[i];
            }
        }

        if(cookSlotContents.Count > ItemStatusManager.itemStatusManager.cooks[num].needIngredients.Count)
        {
            for(int i = ItemStatusManager.itemStatusManager.cooks[num].needIngredients.Count; i <cookSlotContents.Count; i++)
            {
                cookSlotContents[i].gameObject.SetActive(false);
            }
        }

        return canMake;
    }
    // 요리 만들기 버튼 기능
    public void MakeCook()
    {   // 요리를 만들 수 있는지 확인후 만들 수 있으면 요리 이미지와 이름, 설명들로 바꿔준다.
        if(ItemStatusManager.itemStatusManager.cooks[cookNum].CheckMake())
        {
            ItemStatusManager.itemStatusManager.cooks[cookNum].ConsumeStuff();
            ItemStatusManager.itemStatusManager.cooks[cookNum].isPurchased = true;
            cookPanel.SetActive(true);
            rewardCookNameText.text = ItemStatusManager.itemStatusManager.cooks[cookNum].GetName();
            rewardCookDescText.text = "어린 냥이야.\n맛있게 먹으렴!";
            rewardCookImage.sprite = ItemStatusManager.itemStatusManager.cooks[cookNum].GetSprite();

            StartCoroutine(Cook_Start());

            CookDataUpdate(cookNum);
            IngredientDataUpdate();
            UserDataManager.userDataManager.SaveData();
        }
        else
        {
            IEnumerator cor = Need_Ingredient();
            StopCoroutine(cor);
            StartCoroutine(cor);
        }
    }
    // 다음 요리 버튼 기능
    public void nextCookPage()
    {
        if(pageButton[0].activeSelf == false) pageButton[0].SetActive(true);
        cookNum = (cookNum+1)%cookLength;
        scrollNum = cookNum;
        cookScrollbar.value = (float)scrollNum / cookLength;
        if(cookNum+1 >= cookLength)
        {
            pageButton[1].SetActive(false);
            return;
        }
        //CookDataUpdate(cookNum);
    }
    // 이전 요리 버튼 기능
    public void previousCookPage()
    {
        if(pageButton[1].activeSelf == false) pageButton[1].SetActive(true);
        cookNum = cookNum-1;
        scrollNum = cookNum;
        cookScrollbar.value = (float)scrollNum / cookLength;
        if(cookNum <= 0) 
        {
            pageButton[0].SetActive(false);
            return;
        }
        //CookDataUpdate(cookNum);
    }
    // 재료데이터 초기설정
    public void IngredientDataInit()
    {
        for(int i = 0; i < ItemStatusManager.itemStatusManager.ingredients.Length; i++)
       {    // 재료 데이터 슬롯들을 모든 재료 개수만큼 추가해줌.
            GameObject ingredient = Instantiate(IngredientSlotPrefab);
            ingredient.name = ItemStatusManager.itemStatusManager.ingredients[i].GetName();
            ingredient.transform.SetParent(ViewContent.transform, false);
            IngredientSlot slot = ingredient.GetComponent<IngredientSlot>();
            slot.SetInfo(ItemStatusManager.itemStatusManager.ingredients[i]);
            slot.BookTextNum();

            ingredientSlotInfo.Add(slot);
       }
    }
    // 재료 데이터 업데이트
    public void IngredientDataUpdate()
    {   
        for(int i = 0; i < ingredientSlotInfo.Count; i++)
       {
            ingredientSlotInfo[i].BookTextNum();
       }
    }
    // 필요 재료들
    public IEnumerator Need_Ingredient()
    {
        warn_image.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        warn_image.gameObject.SetActive(false);
    }
    // 요리 애니메이션
    IEnumerator Cook_Start()
    {
        int count = 0;
        while(count != 12)
        {
            cooking_text.text = cooking_texts[count%3];
            yield return cooking_time;
            count++;
        }

        cookingPanel.SetActive(false);
        cookresultPanel.SetActive(true);
    }

    public void ScrollbarCallback(float val)
    {
        scrollNum = (int)(val*cookLength);
        if(scrollNum+1 >= cookLength) 
        {
            scrollNum = cookLength-1;
            pageButton[1].SetActive(false);
        }
        else if(scrollNum == 0)
        {
            pageButton[0].SetActive(false);
        }
        else
        {
            pageButton[0].SetActive(true);
            pageButton[1].SetActive(true);
        }
        Debug.Log(scrollNum);
        cookNum = scrollNum;
        cookNumText.text = (cookNum+1) + "/" + cookLength +"p";
        CookDataUpdate(cookNum);
    }
}