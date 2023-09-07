using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class QuestInfo
{
    //public string[] clear_quest_List;
    public List<Quest> clear_quest_List;
    public List<string> clear_quest_key_List;

    public QuestInfo()
    {
        //clear_quest_List = new string[] {};
        clear_quest_List = new List<Quest>();
        clear_quest_key_List = new List<string>();
    }
    public QuestInfo(List<Quest> clearQuest)
    {
        clear_quest_List = new List<Quest>();
        clear_quest_key_List = new List<string>();

        for(int i = 0; i < clearQuest.Count; i++)
        {
            clear_quest_List.Add(clearQuest[i]);
            clear_quest_key_List.Add(clearQuest[i].key);
            Debug.Log("������ ���� " + clear_quest_List[i].key);
        }
    }
}

[System.Serializable]
public class StuffShopInfo
{
    public string[] stuffKeys = new string[6];
    public int[] stuffSellCnts = new int[6];
    public int[] stuffDisRates = new int[6];
    public bool[] stuffCanBuys = new bool[6];

    public StuffShopInfo()
    {
        for(int i = 0; i < 6; i++)
        {
            stuffKeys[i] = ItemStatusManager.itemStatusManager.ingredients[i].GetkeyID();
            Debug.Log("���� �ε��� ��ȣ" + i + " �̸� " + ItemStatusManager.itemStatusManager.ingredients[i].keyID);
        }
        for(int i = 0; i < 6; i++) stuffSellCnts[i] = 1;
        for(int i = 0; i < 6; i++) stuffDisRates[i] = 0;
        for(int i = 0; i < 6; i++) stuffCanBuys[i] = true;
    }

    public StuffShopInfo(List<IngredientSlot> stuffSlotData)
    {
        for(int i = 0; i < stuffSlotData.Count; i++)
        {
            stuffKeys[i] = stuffSlotData[i].GetInfo().GetkeyID();
            stuffSellCnts[i] = stuffSlotData[i].sell_Count;
            stuffDisRates[i] = stuffSlotData[i].discound_Percent;
            stuffCanBuys[i] = stuffSlotData[i].canBuy;
        }
    }
}

[System.Serializable]
public class ItemInfo
{
    [Header("Map")]
    public bool[] maps = new bool[11] {true, false, false, false, false, false, false, false, false, false,
                                        false};
    [Header("Character")]
    public bool[] characters = new bool[15] {true,false,false,false,false,false,false,false,false,false,
                                        false,false,false,false,false};
    [Header("Cook")]
    public bool[] cooks = new bool[21] {false,false,false,false,false,false,false,false,false,false,
                                        false,false,false,false,false,false,false,false,false,false,
                                        false};

    [Header("Ingredient")]
    public int[] ingredients = new int[25] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                             0, 0, 0, 0, 0 }; 

    public ItemInfo()
    {
        // ��
        maps[0] = true;
        for(int i = 1; i < maps.Length; i++)
        {
            maps[i] = false;
        }

        // ĳ����
        characters[0] = true;
        for(int i = 1; i < characters.Length; i++)
        {
            characters[i] = false;
        }
        // �丮
        for(int i = 0; i < cooks.Length; i++)
        {
            cooks[i] = false;
        }

        // ���
        for(int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i] = 0;
        }
    }

    public ItemInfo(bool[] _maps, bool[] _characters, bool[] _cooks, int[] _ingredients)
    {
        // ��
        for(int i = 0; i < maps.Length; i++)
        {
            maps[i] = _maps[i];
        }

        // ĳ����
        for(int i = 0; i < characters.Length; i++)
        {
            characters[i] = _characters[i];
        }
        // �丮
        for(int i = 0; i < cooks.Length; i++)
        {
            cooks[i] = _cooks[i];
        }

        // ���
        for(int i = 0; i < _ingredients.Length; i++)
        {
            ingredients[i] = _ingredients[i];
        }
    }
}

public class UserData
{
    public string m_nickname = "";   // �÷��̾� �г���
    public int m_gold = 0;  // �÷��̾� ���
    public int m_stackedGold = 0; // �÷��̾� ���� ���
    public float m_score = 0;   // �÷��̾� �ְ� ����
    public QuestInfo questInfo = new QuestInfo(); // �÷��̾� ������ ���� ��������
    public ItemInfo itemInfo = new ItemInfo();   // �÷��̾� �����۵鿡 ���� ��������
    public StuffShopInfo stuffShopInfo; // �÷��̾� ��� ������ ���� ��������
    public bool tutorial = false;   // �÷��̾� Ʃ�丮�� Ŭ���� ����

    //public string currentMap = "";
    public int currentMap = 0;  // �÷��̾ ������ �� ��ȣ
    public string currentCharacter = "";    // �÷��̾ ������ ĳ���� �̸�
    public int ingredientShopCharacterTouch = 0;    // ������ ĳ���� ��ġ Ƚ��
    public int buyStuffcnt = 0; // �÷��̾� ��� ���� Ƚ��
    public int playerJumpCnt = 0; // �÷��̾� ���� ���� Ƚ��
    public int avoidCnt = 0;    // �÷��̾� ��ֹ� ȸ�� ���� Ƚ��

    public string LastLogOutTime = "";   // ������ �α׾ƿ� �ð�
    public string LastLoginDateTime = "";    // ������ �α��� ��¥
    public string LastLoginTime = "";    // ������ �α��� �ð�
    public string Last_ingredientShop_intime = ""; // ���� ���������� ���ð�

    public int Day_Attendance = 0;  // �÷��̾� �⼮��¥
    public bool get_attendance_reward = false;
}

[System.Serializable]
public class UserDataManager : MonoBehaviour
{
    public static UserDataManager userDataManager = null;
    private static readonly string PrivateKey = "1718hy9dsf0jsdlfjds0pa9ids78ahgf81h32re";

    UserData player = new UserData();
    string path;
    string filename = "save.atree";

    public bool doesNotExistSaveFile = false;
    bool CallingSucceeded = false;
    public bool bGoodToLoad = true;
    public bool loadFinish = false;

    private void Awake()
    {
        if(userDataManager != null && userDataManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            userDataManager = this;
        }
        DontDestroyOnLoad(gameObject);

        path = Application.persistentDataPath + "/";
        print(path);

        StartCoroutine(init());
    }

    public IEnumerator init()
    {
        yield return new WaitForEndOfFrame();
        while(!CallingSucceeded)
        {
            if(GameManager.gm.getIsLoadingFinish() && bGoodToLoad)
            {
                Debug.Log("�ҷ����� ����");
                player = LoadData();
                //CreateNewData();
                GameManager.gm.playerData = player;
                CallingSucceeded = true;
                //InGameManager.inGameManager.uiManager.TESTSET();
                //SaveData();
            }
        }
    }

    #region Save_And_Load
    public void SaveData()
    {
        if(GameManager.gm.isGuest) return;
        var ItInfo = ItemStatusManager.itemStatusManager;

        player.itemInfo = ItemStatusManager.itemStatusManager.Get_ItemStatus(); // ������ ������ �ҷ�����
        player.questInfo = QuestManager.questManager.Get_QuestStatus();   // ����Ʈ ������ �ҷ�����
        player.stuffShopInfo = InGameManager.inGameManager.ingredientShopManager.GetStuffShopInfo();
        //player.questInfo.SetClearData(player.questInfo.achive_clear_list);
        //Debug.Log(player.questInfo.achive_clear_data);

        player.m_gold = GameManager.gm.gold;    // �÷��̾� ��� �ҷ�����
        player.m_stackedGold = GameManager.gm.stackedGold;  // �÷��̾� ���� ��� �ҷ�����
        player.m_score = GameManager.gm.score;  // �÷��̾� �ְ� ���� �ҷ�����
        player.tutorial = GameManager.gm.tutorial;  // �÷��̾� Ʃ�丮�� Ŭ���� Ȯ�� �ҷ�����
        player.ingredientShopCharacterTouch = GameManager.gm.ingredient_char_touch; // �÷��̾� ������ �ҸӴ� ��ġ Ƚ�� �ҷ�����
        player.buyStuffcnt = GameManager.gm.buyCnt; // �÷��̾� ��� ���� Ƚ�� �ҷ�����
        player.playerJumpCnt = GameManager.gm.jumpCnt;  // �÷��̾� ���� ���� Ƚ��
        player.avoidCnt = GameManager.gm.avoidCnt;  // �÷��̾� ��ֹ� ���� ȸ�� Ƚ��

        player.currentMap = GameManager.gm.mapNum;  // ���� �÷��̾ ������ �� �ҷ�����
        player.currentCharacter = GameManager.gm.currentCharacter;  // ���� �÷��̾ ������ ĳ���� �ҷ�����
        player.get_attendance_reward = GameManager.gm.GetAttendanceReward;

        player.Last_ingredientShop_intime = InGameManager.inGameManager.ingredientShopManager.Last_in_time; // �÷��̾��� ������ ������ ���Խð� �ҷ�����
        player.LastLogOutTime = NTP.ntp.getServerTime().ToString(); // �÷��̾� �α׾ƿ� �ð� �ҷ�����
        Debug.Log("�α׾ƿ� �ð� : " + player.LastLogOutTime);
        //player.Day_Attendance;

        string jsonString = SaveData_ToJson(player);
        string encryptString = Encrypt(jsonString);
        SaveFile(encryptString);

        //printData(true);
    }

    private static void SaveFile(string jsonData)
    {
        using (FileStream fs = new FileStream(GetPath(), FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            fs.Write(bytes, 0, bytes.Length);
        }
    }

    public UserData LoadGuestData()
    {
        CreateNewData();
        return player;
    }

    //data load test
    public UserData LoadData()
    {
        if(bGoodToLoad)
        {
            if(!File.Exists(GetPath()))
            {
                print("������ �������� �ʽ��ϴ�. �� ���������� �����մϴ�.");
                doesNotExistSaveFile = true;
                CreateNewData();
                return player;
            }
            else
            {
                string encryptedData = LoadFile(GetPath());
                string decryptedData = Decrypt(encryptedData);

                Debug.Log("decryptedData : " + decryptedData);

                UserData previousPlayerData = JsonToData(decryptedData);

                GameManager.gm.gold = previousPlayerData.m_gold;    // ��� �ҷ�����
                GameManager.gm.stackedGold = previousPlayerData.m_stackedGold;  // ���� ��� �ҷ�����
                GameManager.gm.currentCharacter = previousPlayerData.currentCharacter;  // ���� ĳ���� ���� �ҷ�����
                GameManager.gm.tutorial = previousPlayerData.tutorial;  // Ʃ�丮�� Ŭ���� ���� �ҷ�����
                GameManager.gm.score = previousPlayerData.m_score;  // �ְ� ���� �ҷ�����
                GameManager.gm.mapNum = previousPlayerData.currentMap;  // ���� �� ���� �ҷ�����
                GameManager.gm.ingredient_char_touch = previousPlayerData.ingredientShopCharacterTouch; // ��� ���� �ҸӴ� ��ġ Ƚ�� �ҷ�����
                GameManager.gm.buyCnt = previousPlayerData.buyStuffcnt; // ��� ���� Ƚ�� �ҷ�����
                GameManager.gm.jumpCnt = previousPlayerData.playerJumpCnt;  // �÷��̾� ���� ���� Ƚ�� �ҷ�����
                GameManager.gm.avoidCnt = previousPlayerData.avoidCnt;  // �÷��̾� ��ֹ� ���� ȸ�� Ƚ�� �ҷ�����

                GameManager.gm.GetAttendanceReward = Calculate_TimeDifference(previousPlayerData);  // �⼮ ���� �޾ƾߵǴ��� �ð� Ȯ��

                previousPlayerData.LastLoginTime = NTP.ntp.getServerTime().ToString();  // �÷��̾� �α��� �ð�
                Debug.Log("�÷��̾� ������ �α��� �ð� " + previousPlayerData.LastLoginTime);
                previousPlayerData.LastLoginDateTime = NTP.ntp.getServerTime().ToString("yyyy-MM-dd");  // �÷��̾� ������ �α��� ��¥
                Debug.Log("�÷��̾� ������ �α��� ��¥" + previousPlayerData.LastLoginDateTime);

                GameManager.gm.shop_need_chage = Ingredient_need_renew(previousPlayerData);    // ���� ������ �ʿ��Ѱ�?

//                test_txt.text = "������ �ҷ����� �Ϸ�." + (previousPlayerData.tutorial == true ? "Ʃ�丮�� �Ϸ� ����" : "Ʃ�丮�� ����");
                loadFinish = true;

                ItemStatusManager.itemStatusManager.Load_ItemStatus(previousPlayerData.itemInfo);

                QuestManager.questManager.Load_QuestStatus(previousPlayerData.questInfo);

                return previousPlayerData;
            }
        }

        return null;
    }

    private static string LoadFile(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);

            string jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            return jsonString;
        }
    }
    #endregion

    #region About_Json
    private static string SaveData_ToJson(UserData player)
    {
        string data = JsonUtility.ToJson(player);

#if UNITY_EDITOR
        File.WriteAllText(GetPath_Debug(), data);
#endif
        return data;
    }

    private UserData JsonToData(string jsonData)
    {
        UserData data = JsonUtility.FromJson<UserData>(jsonData);

        return data;
    }
    #endregion

    #region  Encrypt_And_Decrypt
    private static string Encrypt(string data)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateEncryptor();
        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Convert.ToBase64String(results, 0, results.Length);
    }

    private static string Decrypt(string data)
    {
        byte[] bytes = System.Convert.FromBase64String(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateDecryptor();
        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }

    private static RijndaelManaged CreateRijndaelManaged()
    {
        byte[] keyArray = Encoding.UTF8.GetBytes(PrivateKey);
        var result = new RijndaelManaged();

        var newKeysArray = new byte[16];
        Array.Copy(keyArray, 0, newKeysArray, 0, 16);

        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }

    static string GetPath()
    {
        Debug.Log("��θ� �˷��� : " + Application.persistentDataPath);
        return Path.Combine(Application.persistentDataPath, "save.atree");
    }

    static string GetPath_Debug()
    {
        return Path.Combine(Application.persistentDataPath, "save.json");
    }
    #endregion

    public void CreateNewData()    // ���������� �������� ������ ���ο� �����͸� �����ϰ� ��� ����.
    {
        UserData player = new UserData();
        Debug.Log("�� ������ ����");
        GameManager.gm.tutorial = false;    // Ʃ�丮�� Ŭ���� x

        player.currentCharacter = "�����";   // �⺻ ĳ���� ����
        GameManager.gm.currentCharacter = player.currentCharacter; 
        
        player.currentMap = 0;  // �⺻ �� ����
        GameManager.gm.mapNum = player.currentMap;

        player.m_gold = 0;  // ��� 0���� ����
        GameManager.gm.gold = player.m_gold;

        player.m_stackedGold = 0;   // ���� ��� 0���� ����
        GameManager.gm.stackedGold = player.m_stackedGold;

        player.m_score = 0; // �ְ� ���� 0���� ����
        GameManager.gm.score = player.m_score;

        player.ingredientShopCharacterTouch = 0;    // ��� ���� ��ġȽ�� 0���� �ʱ�ȭ
        GameManager.gm.ingredient_char_touch = player.ingredientShopCharacterTouch;

        player.buyStuffcnt = 0; // ��� ���� ����Ƚ�� 0���� �ʱ�ȭ
        GameManager.gm.buyCnt = player.buyStuffcnt;

        player.playerJumpCnt = 0;   // �÷��̾� ���� ���� Ƚ�� 0���� �ʱ�ȭ
        GameManager.gm.jumpCnt = player.playerJumpCnt;

        player.avoidCnt = 0;
        GameManager.gm.avoidCnt = player.avoidCnt;

        player.itemInfo = new ItemInfo();   // ��� ���� �ʱ�ȭ
        ItemStatusManager.itemStatusManager.Load_ItemStatus(player.itemInfo);
        player.questInfo = new QuestInfo(); // ����Ʈ ���� �ʱ�ȭ
        QuestManager.questManager.Load_QuestStatus(player.questInfo);

        player.LastLoginTime = NTP.ntp.getServerTime().ToString();  // �÷��̾� �α��� �ð�
        Debug.Log("�÷��̾� ������ �α��� �ð� " + this.player.LastLoginTime);
        player.LastLoginDateTime = NTP.ntp.getServerTime().ToString("yyyy-MM-dd");
        Debug.Log("�÷��̾� ������ �α��� ��¥" + this.player.LastLoginDateTime);

        GameManager.gm.shop_need_chage = true;
        GameManager.gm.GetAttendanceReward = false;
        
        this.player = player;
        //SaveData();
    }

    public IEnumerator AsyncSave()
    {
        while(!GameManager.gm.isGuest)
        {
            yield return new WaitForSecondsRealtime(10f);
            SaveData();
            Debug.Log("�ڵ����� �Ǿ����ϴ�.");
            //save_txt.text = "�ڵ� ���� �Ǿ����ϴ�." + (++count);
        }
    }

    bool Calculate_TimeDifference(UserData playerData) // �⼮���� ���� �Ǵ��� ���� ������ �α��ΰ� ���� �α��� �ð��� �����Լ�
    {
        var playerLastLoginTime = int.Parse(playerData.LastLoginDateTime.Replace("-", "")); // �÷��̾� ���̺� �� ����� �ð����� ��¥�� ���� �� int�� ĳ����
        var DateTime_Now = int.Parse(NTP.ntp.getServerTime().ToString("yyyy-MM-dd").Replace("-", ""));
        Debug.Log("�÷��̾� ������ �α��� " + playerLastLoginTime + ", ���� ��¥ " + DateTime_Now);
        //var DateTime_Now = int.Parse(NTP.ntp.getServerTime().ToShortDateString().Replace("-", "")); // ���� �ð����� ��¥�� ���� �� int�� ĳ����
        //debug
        //var DateTime_Now = int.Parse(DateTime.Now.ToShortDateString().Replace("-", ""));


        if (playerLastLoginTime < DateTime_Now) // �⵵(4�ڸ�)+��(2�ڸ�)+��(2�ڸ�) �� 8�ڸ� int�� �� �� �� ����ð� ���� �� ũ�� �� �����̴� true����
            return false;

        return true;
    }

    bool Ingredient_need_renew(UserData Lastplayer)
    {
        if(Lastplayer.Last_ingredientShop_intime == "") 
        {
            Debug.Log("���� �ð� ���� ! �׳� �����մϴ�.");
            return true;
        }

        TimeSpan timecal = NTP.ntp.getServerTime() - DateTime.Parse(Lastplayer.Last_ingredientShop_intime); //DateTime.Parse(Lastplayer.LastLogOutTime);
        int m = DateTime.Parse(Lastplayer.Last_ingredientShop_intime).Minute;
        Debug.Log("�ð��� : " + NTP.ntp.getServerTime() + " �÷��̾� ������ ���� ���� �ð� : " + Lastplayer.Last_ingredientShop_intime + " �� : " + m);
        Debug.Log("timecal : " + timecal + "�� : " + timecal.Minutes*60 + " �� : " + timecal.Seconds);

        if(timecal.Minutes>=30) {   Debug.Log("30���� �������ϴ�!"); return true;   }
        else if(m < 30 && m + timecal.Minutes >= 30) {      Debug.Log("�нð��� 30�� �̸����� ��������!"); return true;    }
        else if(m >= 30 && m + timecal.Minutes >= 60) {     Debug.Log("�нð��� 30�� �̻󿡼� ��������!"); return true;     }

        Debug.Log("���� ����� ������ �ʿ�� �����ϴ�!");
        return false;
    }
}


[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}