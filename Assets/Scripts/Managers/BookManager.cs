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
    private int cookNum = 0;    // ���� �丮 ��ȣ
    [SerializeField]
    private TMP_Text cookNameText;  // ���� �丮 �̸�
    [SerializeField]
    Image cookImage;                // ���� �丮 �̹���
    [SerializeField]
    GameObject[] pageButton;        // �丮 ������ ��ư 0 : previous button, 1 : next button
    [SerializeField]
    GameObject[] buttonSet;         // ��ư ��Ʈ (�丮�ϱ� ��ư ��Ʈ{�丮�ϱ�, Ȯ��}, ������ ��ư ��Ʈ{Ȯ��})
    [SerializeField]
    Image warn_image;               // ��� �̹���
    [SerializeField]
    GameObject canMakeAlertIcon;
    [SerializeField]
    TMP_Text cookEvalText;          // �丮 �� �ؽ�Ʈ
    [SerializeField]
    GameObject stuffsView;          // ��� Ȯ�ζ� ������Ʈ
    [SerializeField]
    GameObject descView;            // �丮 ����� ������Ʈ
    [SerializeField]
    GameObject cookSlotPrefab;      // �丮 ���� ������
    [SerializeField]
    GameObject cookViewContent;     // �丮 ������ �� ��ġ
    [SerializeField]
    List<CookSlot> cookSlotContents = new List<CookSlot>(); // ��� �丮 ������������

    [SerializeField]
    Scrollbar cookScrollbar;        // ��ũ�ѹ�
    [SerializeField]
    int scrollNum;                  // ��ũ�� ��ȣ
    [SerializeField]
    TMP_Text cookNumText;           // ���� ��ũ�� ��ȣ / ��ü �丮 ����
    [SerializeField]
    int cookLength;                 // ��ü �丮 ����

    [Header("�丮 �ǳ�")]
    [SerializeField]
    private GameObject cookPanel;   // �丮 ����
    [SerializeField]
    private GameObject cookingPanel;    // �丮��
    [SerializeField]
    private GameObject cookresultPanel; // �丮 ���
    [SerializeField]
    private TMP_Text cooking_text;  // �丮�� text
    [SerializeField]
    private string[] cooking_texts = {"����\n����Ŀ�\n(�丮�ϴ� ��.)", "����\n����Ŀ�\n(�丮�ϴ� ��..)", "����\n����Ŀ�\n(�丮�ϴ� ��...)"}; // ���߿� ��Ʈ �����ͷ� �������ּ���.
    [SerializeField]
    WaitForSeconds cooking_time = new WaitForSeconds(0.2f); // �丮 �ִϸ��̼� �ð�
    [SerializeField]
    private TMP_Text rewardCookNameText;    // �丮 ���� �̸�
    [SerializeField]
    private TMP_Text rewardCookDescText;    // �丮 ���� �̸�
    [SerializeField]
    private Image rewardCookImage;  // �丮 ��� �̹���
    [SerializeField]
    int pageNum = 0;    // �丮 ���� ���� ������ ��ȣ -> ��ũ�� ��ȣ

    [Header("Ingredient")]
    [SerializeField]
    GameObject IngredientSlotPrefab;    // ��� ���� ���� ������
    [SerializeField]
    GameObject ViewContent; // �������� ���� �� ������
    [SerializeField]
    List<IngredientSlot> ingredientSlotInfo;    // ��� ���� ������ ������ ����Ʈ

    void Awake()
    {   // ȭ�� ũ�� ����
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
    {   // �÷��̾� ���� ȭ�� 
        GameManager.gm.current_screen = "BookShop";
        cookNum = scrollNum;    // scrollNum������ ��ũ�Ѻ� ��ġ�� �����Ѵ�.
        if(cookNum == 0)    // ù ��° ���������
        {   
            pageButton[0].SetActive(false); // ���� ������ ��ư OFF
            pageButton[1].SetActive(true);  // ���� ������ ��ư ON
        }
        else if(cookNum == cookLength-1)    // ������ ���������
        {
            pageButton[0].SetActive(true);  // ���� ������ ��ư ON
            pageButton[1].SetActive(false); // ���� ������ ��ư OFF
        }
        else
        {   // �� �ܴ� ����, ���� ������ ��ư ON
            pageButton[0].SetActive(true);  
            pageButton[1].SetActive(true);
        }
        IngredientDataUpdate();
        CookDataUpdate(cookNum);
        cookNumText.text = (cookNum+1) + "/" + cookLength +"p"; // ���� ������
    }

    public void Init()
    {   // ��� ���� �ʱ�ȭ
        ingredientSlotInfo = new List<IngredientSlot>();
        CookDataUpdate(0);
        IngredientDataInit();
        pageNum=0;
        cookScrollbar.value = pageNum;
        cookScrollbar.onValueChanged.AddListener((float val) => ScrollbarCallback(val));    // ���� �ٲ� ���� �� ���� �����ϴ� ��� �߰�
        cookLength = ItemStatusManager.itemStatusManager.cooks.Length;  // �丮 ����
    }
    // �丮 ������ �丮 Ȯ���� �˸�
    public bool CheckCanReceiveCook()
    {
        for(int i = 0; i < cookLength; i++)
        {
            if(ItemStatusManager.itemStatusManager.cooks[i].isPurchased) continue;
            if(ItemStatusManager.itemStatusManager.cooks[i].CheckMake()) return true;
        }
        return false;
    }
    // �丮 ������ ������Ʈ
    public void CookDataUpdate(int num)
    {   // �丮�� ���ŵǾ����� �丮 ����� UI�� �ٲ��ش�.
        cookNameText.text = "������<br>" + ItemStatusManager.itemStatusManager.cooks[num].GetName();
        if(ItemStatusManager.itemStatusManager.cooks[num].isPurchased)
        {
            cookEvalText.text = ItemStatusManager.itemStatusManager.cooks[num].GetDescription();
            stuffsView.gameObject.SetActive(false);
            descView.gameObject.SetActive(true);
            buttonSet[0].SetActive(false);
            buttonSet[1].SetActive(true);
        }
        else    // ���� ���� ���� ���¶�� �䱸�ϴ� ��� UI �ٲ��ش�.
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
    // �丮 ���� ������Ʈ
    public bool updateCookInfo(int num)
    {
        bool canMake = true;
        for(int i = 0; i < ItemStatusManager.itemStatusManager.cooks[num].needIngredients.Count; i++)
        {   // �丮���� �� �ִ� ������ ǥ����. �䱸�ϴ� ��� �������� ���� �� ������ ����, ���� �� ���Ե��� �� �߰����ִ� ������� ���۵�.
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
    // �丮 ����� ��ư ���
    public void MakeCook()
    {   // �丮�� ���� �� �ִ��� Ȯ���� ���� �� ������ �丮 �̹����� �̸�, ������ �ٲ��ش�.
        if(ItemStatusManager.itemStatusManager.cooks[cookNum].CheckMake())
        {
            ItemStatusManager.itemStatusManager.cooks[cookNum].ConsumeStuff();
            ItemStatusManager.itemStatusManager.cooks[cookNum].isPurchased = true;
            cookPanel.SetActive(true);
            rewardCookNameText.text = ItemStatusManager.itemStatusManager.cooks[cookNum].GetName();
            rewardCookDescText.text = "� ���̾�.\n���ְ� ������!";
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
    // ���� �丮 ��ư ���
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
    // ���� �丮 ��ư ���
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
    // ��ᵥ���� �ʱ⼳��
    public void IngredientDataInit()
    {
        for(int i = 0; i < ItemStatusManager.itemStatusManager.ingredients.Length; i++)
       {    // ��� ������ ���Ե��� ��� ��� ������ŭ �߰�����.
            GameObject ingredient = Instantiate(IngredientSlotPrefab);
            ingredient.name = ItemStatusManager.itemStatusManager.ingredients[i].GetName();
            ingredient.transform.SetParent(ViewContent.transform, false);
            IngredientSlot slot = ingredient.GetComponent<IngredientSlot>();
            slot.SetInfo(ItemStatusManager.itemStatusManager.ingredients[i]);
            slot.BookTextNum();

            ingredientSlotInfo.Add(slot);
       }
    }
    // ��� ������ ������Ʈ
    public void IngredientDataUpdate()
    {   
        for(int i = 0; i < ingredientSlotInfo.Count; i++)
       {
            ingredientSlotInfo[i].BookTextNum();
       }
    }
    // �ʿ� ����
    public IEnumerator Need_Ingredient()
    {
        warn_image.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        warn_image.gameObject.SetActive(false);
    }
    // �丮 �ִϸ��̼�
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