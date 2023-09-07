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
    private int number = 0; // ���� ����Ű�� �ִ� ĳ���� ���� �ε���

    [Header("Cat Prefab")]
    [SerializeField]
    private GameObject cat_prefab;  // ����� ���� ������ ������Ʈ
    [SerializeField]
    private GameObject viewContent; // ����� ���Ե��� ǥ�õ� ������ ����
    [SerializeField]
    private List<CatShopContent> catSlotInfo;   // ����� ���Ե� ������ ��� ����Ʈ

    [Header("Cat name")]
    [SerializeField]
    private TMP_Text character_name_text;   // ����� ���� �̸��� ��Ÿ���� �ؽ�Ʈ
    [SerializeField]
    private TMP_Text character_description; // ����� ���� ������ ��Ÿ���� �ؽ�Ʈ

    [Header("Buy Info")]
    [SerializeField]
    private GameObject[] buttons;   // ��ư ������ 0 : ���� ��ư, 1 : ������ ��ư, 2 : ���� ��ư
    [SerializeField]
    private TMP_Text buy_gold_text; // ���Ž� �ʿ��� ������ ǥ�����ִ� �ؽ�Ʈ
    [SerializeField]
    private TMP_Text warning_message;   // ���� ������ �� �ߴ� ���� �ؽ�Ʈ

    [SerializeField]
    private HorizontalScrollSnap snap;  // ��ũ�ѽ� ����ȿ���� ���ϰ��ִ� ������Ʈ
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
        if (number != snap.CurrentPage) // ����Ű�� ��ȣ�� snap���� ����Ű�� ��ȣ�� �ٸ� ���.
        {
            number = snap.CurrentPage;
            Debug.Log("���� ��ȣ " + number);
            UpdateInfo();   // �ش� ��ȣ �������� ������Ʈ���ش�.
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
        GameManager.gm.current_screen = "CharacterShop";    //  ���� �÷��̾ ���ִ� ���� ĳ���� ����

        character_name_text.text = ItemStatusManager.itemStatusManager.characters[number].character_name;   // �ش� ĳ���� ���� �̸����� ����

        if (ItemStatusManager.itemStatusManager.characters[number].isPurchased) // �̹� ������ ���¶��
        {   // ĳ���� ���� �� ��ư ����
            character_description.text = ItemStatusManager.itemStatusManager.characters[number].character_des.Replace("\n", "<br>");  
            buttons[0].SetActive(false);    // ���� ��ư OFF
            buttons[1].SetActive(false);    // ������ ��ư OFF
            buttons[2].SetActive(true);     // ���� ��ư ON
        }
        else
        {   // �������� ���� ���¶��
            character_description.text = "??? ???"; // ���� �̰���
            buttons[0].SetActive(true);     // ���� ��ư ON
            buttons[1].SetActive(true);     // ������ ��ư ON
            buttons[2].SetActive(false);    // ���� ��ư OFF
            buy_gold_text.text = ItemStatusManager.itemStatusManager.characters[number].Cost.ToString("#,##0"); // ĳ���� ���ſ� �ʿ��� ���
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
    {   // ���� �ʱ� ��ȣ�� �����ϰ� �ִ� ĳ���� ��ȣ.
        number = ItemStatusManager.itemStatusManager.character_num_dic[GameManager.gm.currentCharacter];
        // ������ ��Ͽ� �ִ� ĳ���� ���� ��ŭ ������ �����Ѵ�.
        for (int i = 0; i < ItemStatusManager.itemStatusManager.characters.Length; i++)
        {   
            GameObject cat = Instantiate(cat_prefab);   // ���� ����
            cat.name = ItemStatusManager.itemStatusManager.characters[i].character_name;    // ���� �̸� = ĳ���� �̸�(Ȯ�ο�)
            cat.transform.SetParent(viewContent.transform, false);  // �ش� ���� ��ġ�� scrollsnap ���η� �Ű���
            CatShopContent slot = cat.GetComponent<CatShopContent>();   // ���Կ� ������ ������ ������Ʈ �ҷ�����
            slot.cat.sprite = InGameManager.inGameManager.player.cat_skin.skinSet.catSkins[i].CatSprites[0];    // ���� �̹��� ���� ĳ����
            slot.board.sprite = InGameManager.inGameManager.player.cat_skin.skinSet.catSkins[i].BoardSprites[0];    // ���� �̹��� ���� ����
            if (ItemStatusManager.itemStatusManager.characters[i].isPurchased)  // �̹� ������ ���¶��
            {   // ĳ���� ��� �����ֱ�
                slot.cat.color = new Color(1f, 1f, 1f);
                slot.board.color = new Color(1f, 1f, 1f);
            }
            else
            {   // �������� ���� ���¶��
                // �׸��� ���
                slot.cat.color = new Color(0, 0, 0);
                slot.board.color = new Color(0, 0, 0);
            }
            catSlotInfo.Add(slot);  // ����Ʈ�� ������Ʈ�� ���� �߰�
        }

        snap.StartingScreen = number;   
    }

    public bool CheckCanBuyCat()    // ������ �� �ִ� ĳ���Ͱ� �����ϴ��� Ȯ���ϴ� �Լ�
    {   // ���� ���� ĳ���Ͱ� ���� �� ����ȭ�鿡 �˸� ǥ��
        for (int i = 0; i < ItemStatusManager.itemStatusManager.characters.Length; i++)
        {
            if (ItemStatusManager.itemStatusManager.characters[i].isPurchased) continue;
            if (ItemStatusManager.itemStatusManager.characters[i].Cost <= GameManager.gm.gold) return true;
        }
        return false;
    }

    public void Next_Button()   // ���� ĳ���� ��ư
    {
        number++;   // ��ȣ ����. ��ȣ�� ��ü ĳ���� �������� ũ�ų� ������ 0���� �ʱ�ȭ
        // ĳ���� �̸� ������Ʈ
        character_name_text.text = ItemStatusManager.itemStatusManager.characters[number].character_name;//cats[number].character_name;
        // ĳ���� ��ư ������Ʈ
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

    public void Previous_Button()   // ���� ĳ���� ��ư
    {
        number--;   // ��ȣ ����. ��ȣ�� 0���� ������ ��ü ĳ���� �������� ���������� �ʱ�ȭ
        if (number < 0) number = ItemStatusManager.itemStatusManager.characters.Length - 1;
        // ĳ���� �̸� ������Ʈ
        character_name_text.text = ItemStatusManager.itemStatusManager.characters[number].character_name;//cats[number].character_name;
        // ĳ���� ��ư ������Ʈ
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
    {   // ���Ž�
        if (GameManager.gm.gold < ItemStatusManager.itemStatusManager.characters[number].Cost)
        {   // �÷��̾ �����ϰ� �ִ� ��尡 �����ϰ����ϴ� ��뺸�� ���� ��� ��� �˸�â
            StopCoroutine(WarningMessage());
            IEnumerator i = WarningMessage();
            StartCoroutine(i);
        }
        else
        {   // ��������� ����� ���. ��� ��ŭ �氨
            GameManager.gm.gold -= ItemStatusManager.itemStatusManager.characters[number].Cost;
            InGameManager.inGameManager.uiManager.Gold_Top_Panel_On();
            ItemStatusManager.itemStatusManager.characters[number].isPurchased = true;  // ���� �Ϸ� ǥ��
            buttons[0].SetActive(false);    // ���� ��ư OFF
            buttons[1].SetActive(false);    // ������ ��ư OFF
            buttons[2].SetActive(true);     // ���� ��ư ON
            catSlotInfo[number].cat.color = new Color(1f, 1f, 1f);  // ĳ���� �� ���� ���� ON
            catSlotInfo[number].board.color = new Color(1f, 1f, 1f);
            character_description.text = ItemStatusManager.itemStatusManager.characters[number].character_des.Replace("\n", "<br>");  

            UserDataManager.userDataManager.SaveData(); // ������ ������ ����
        }
    }

    public void UpdateInfo()
    {   // ���� ���� ��ȣ�� �������� �̸�, ��ư���� ������Ʈ���ش�.
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

    public void PutOnSkin() // ��Ų ���� ��ư
    {
        snap.StartingScreen = number;   // ���� ȭ���� ���� ��ȣ�� ����
        GameManager.gm.currentCharacter = ItemStatusManager.itemStatusManager.characters[number].character_name;    // ���� �������� ĳ���ͷ� ����
        InGameManager.inGameManager.player.SetSkinNum();    // ��Ų ��ȣ ����
        UserDataManager.userDataManager.SaveData(); // ����� ���� ����
        GameManager.gm.current_screen = "MainMenu"; // ���� ȭ������ �̵�
    }

    public void CancelButton()
    {   // ������ ��ư
        GameManager.gm.current_screen = "MainMenu";
    }

    IEnumerator WarningMessage()
    {
        warning_message.text = "���� �����մϴ�.";
        yield return new WaitForSeconds(1.5f);
        warning_message.text = "";
    }
}