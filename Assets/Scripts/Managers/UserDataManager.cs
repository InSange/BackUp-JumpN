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
            Debug.Log("데이터 저장 " + clear_quest_List[i].key);
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
            Debug.Log("현재 인덱스 번호" + i + " 이름 " + ItemStatusManager.itemStatusManager.ingredients[i].keyID);
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
        // 맵
        maps[0] = true;
        for(int i = 1; i < maps.Length; i++)
        {
            maps[i] = false;
        }

        // 캐릭터
        characters[0] = true;
        for(int i = 1; i < characters.Length; i++)
        {
            characters[i] = false;
        }
        // 요리
        for(int i = 0; i < cooks.Length; i++)
        {
            cooks[i] = false;
        }

        // 재료
        for(int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i] = 0;
        }
    }

    public ItemInfo(bool[] _maps, bool[] _characters, bool[] _cooks, int[] _ingredients)
    {
        // 맵
        for(int i = 0; i < maps.Length; i++)
        {
            maps[i] = _maps[i];
        }

        // 캐릭터
        for(int i = 0; i < characters.Length; i++)
        {
            characters[i] = _characters[i];
        }
        // 요리
        for(int i = 0; i < cooks.Length; i++)
        {
            cooks[i] = _cooks[i];
        }

        // 재료
        for(int i = 0; i < _ingredients.Length; i++)
        {
            ingredients[i] = _ingredients[i];
        }
    }
}

public class UserData
{
    public string m_nickname = "";   // 플레이어 닉네임
    public int m_gold = 0;  // 플레이어 골드
    public int m_stackedGold = 0; // 플레이어 누적 골드
    public float m_score = 0;   // 플레이어 최고 점수
    public QuestInfo questInfo = new QuestInfo(); // 플레이어 업적에 대한 상태정보
    public ItemInfo itemInfo = new ItemInfo();   // 플레이어 아이템들에 대한 상태정보
    public StuffShopInfo stuffShopInfo; // 플레이어 재료 상점에 대한 상태정보
    public bool tutorial = false;   // 플레이어 튜토리얼 클리어 여부

    //public string currentMap = "";
    public int currentMap = 0;  // 플레이어가 장착한 맵 번호
    public string currentCharacter = "";    // 플레이어가 장착한 캐릭터 이름
    public int ingredientShopCharacterTouch = 0;    // 재료상점 캐릭터 터치 횟수
    public int buyStuffcnt = 0; // 플레이어 재료 구매 횟수
    public int playerJumpCnt = 0; // 플레이어 누적 점프 횟수
    public int avoidCnt = 0;    // 플레이어 장애물 회피 누적 횟수

    public string LastLogOutTime = "";   // 마지막 로그아웃 시간
    public string LastLoginDateTime = "";    // 마지막 로그인 날짜
    public string LastLoginTime = "";    // 마지막 로그인 시간
    public string Last_ingredientShop_intime = ""; // 상점 마지막으로 들어간시각

    public int Day_Attendance = 0;  // 플레이어 출석날짜
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
                Debug.Log("불러오기 시작");
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

        player.itemInfo = ItemStatusManager.itemStatusManager.Get_ItemStatus(); // 아이템 정보들 불러오기
        player.questInfo = QuestManager.questManager.Get_QuestStatus();   // 퀘스트 정보들 불러오기
        player.stuffShopInfo = InGameManager.inGameManager.ingredientShopManager.GetStuffShopInfo();
        //player.questInfo.SetClearData(player.questInfo.achive_clear_list);
        //Debug.Log(player.questInfo.achive_clear_data);

        player.m_gold = GameManager.gm.gold;    // 플레이어 골드 불러오기
        player.m_stackedGold = GameManager.gm.stackedGold;  // 플레이어 누적 골드 불러오기
        player.m_score = GameManager.gm.score;  // 플레이어 최고 점수 불러오기
        player.tutorial = GameManager.gm.tutorial;  // 플레이어 튜토리얼 클리어 확인 불러오기
        player.ingredientShopCharacterTouch = GameManager.gm.ingredient_char_touch; // 플레이어 재료상점 할머니 터치 횟수 불러오기
        player.buyStuffcnt = GameManager.gm.buyCnt; // 플레이어 재료 구매 횟수 불러오기
        player.playerJumpCnt = GameManager.gm.jumpCnt;  // 플레이어 누적 점프 횟수
        player.avoidCnt = GameManager.gm.avoidCnt;  // 플레이어 장애물 누적 회피 횟수

        player.currentMap = GameManager.gm.mapNum;  // 현재 플레이어가 장착한 맵 불러오기
        player.currentCharacter = GameManager.gm.currentCharacter;  // 현재 플레이어가 장착한 캐릭터 불러오기
        player.get_attendance_reward = GameManager.gm.GetAttendanceReward;

        player.Last_ingredientShop_intime = InGameManager.inGameManager.ingredientShopManager.Last_in_time; // 플레이어의 재료상점 마지막 출입시간 불러오기
        player.LastLogOutTime = NTP.ntp.getServerTime().ToString(); // 플레이어 로그아웃 시간 불러오기
        Debug.Log("로그아웃 시간 : " + player.LastLogOutTime);
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
                print("파일이 존재하지 않습니다. 새 프로파일을 생성합니다.");
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

                GameManager.gm.gold = previousPlayerData.m_gold;    // 골드 불러오기
                GameManager.gm.stackedGold = previousPlayerData.m_stackedGold;  // 누적 골드 불러오기
                GameManager.gm.currentCharacter = previousPlayerData.currentCharacter;  // 장착 캐릭터 정보 불러오기
                GameManager.gm.tutorial = previousPlayerData.tutorial;  // 튜토리얼 클리어 유무 불러오기
                GameManager.gm.score = previousPlayerData.m_score;  // 최고 점수 불러오기
                GameManager.gm.mapNum = previousPlayerData.currentMap;  // 장착 맵 정보 불러오기
                GameManager.gm.ingredient_char_touch = previousPlayerData.ingredientShopCharacterTouch; // 재료 상점 할머니 터치 횟수 불러오기
                GameManager.gm.buyCnt = previousPlayerData.buyStuffcnt; // 재료 구매 횟수 불러오기
                GameManager.gm.jumpCnt = previousPlayerData.playerJumpCnt;  // 플레이어 누적 점프 횟수 불러오기
                GameManager.gm.avoidCnt = previousPlayerData.avoidCnt;  // 플레이어 장애물 누적 회피 횟수 불러오기

                GameManager.gm.GetAttendanceReward = Calculate_TimeDifference(previousPlayerData);  // 출석 보상 받아야되는지 시간 확인

                previousPlayerData.LastLoginTime = NTP.ntp.getServerTime().ToString();  // 플레이어 로그인 시간
                Debug.Log("플레이어 마지막 로그인 시간 " + previousPlayerData.LastLoginTime);
                previousPlayerData.LastLoginDateTime = NTP.ntp.getServerTime().ToString("yyyy-MM-dd");  // 플레이어 마지막 로그인 날짜
                Debug.Log("플레이어 마지막 로그인 날짜" + previousPlayerData.LastLoginDateTime);

                GameManager.gm.shop_need_chage = Ingredient_need_renew(previousPlayerData);    // 상점 갱신이 필요한가?

//                test_txt.text = "데이터 불러오기 완료." + (previousPlayerData.tutorial == true ? "튜토리얼 완료 했음" : "튜토리얼 실패");
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
        Debug.Log("경로를 알려줘 : " + Application.persistentDataPath);
        return Path.Combine(Application.persistentDataPath, "save.atree");
    }

    static string GetPath_Debug()
    {
        return Path.Combine(Application.persistentDataPath, "save.json");
    }
    #endregion

    public void CreateNewData()    // 저장파일이 감지되지 않을시 새로운 데이터를 생성하고 즉시 저장.
    {
        UserData player = new UserData();
        Debug.Log("새 데이터 생성");
        GameManager.gm.tutorial = false;    // 튜토리얼 클리어 x

        player.currentCharacter = "어린냥이";   // 기본 캐릭터 적용
        GameManager.gm.currentCharacter = player.currentCharacter; 
        
        player.currentMap = 0;  // 기본 맵 적용
        GameManager.gm.mapNum = player.currentMap;

        player.m_gold = 0;  // 골드 0부터 시작
        GameManager.gm.gold = player.m_gold;

        player.m_stackedGold = 0;   // 누적 골드 0부터 시작
        GameManager.gm.stackedGold = player.m_stackedGold;

        player.m_score = 0; // 최고 점수 0부터 시작
        GameManager.gm.score = player.m_score;

        player.ingredientShopCharacterTouch = 0;    // 재료 상점 터치횟수 0으로 초기화
        GameManager.gm.ingredient_char_touch = player.ingredientShopCharacterTouch;

        player.buyStuffcnt = 0; // 재료 상점 구매횟수 0으로 초기화
        GameManager.gm.buyCnt = player.buyStuffcnt;

        player.playerJumpCnt = 0;   // 플레이어 누적 점프 횟수 0으로 초기화
        GameManager.gm.jumpCnt = player.playerJumpCnt;

        player.avoidCnt = 0;
        GameManager.gm.avoidCnt = player.avoidCnt;

        player.itemInfo = new ItemInfo();   // 재료 정보 초기화
        ItemStatusManager.itemStatusManager.Load_ItemStatus(player.itemInfo);
        player.questInfo = new QuestInfo(); // 퀘스트 정보 초기화
        QuestManager.questManager.Load_QuestStatus(player.questInfo);

        player.LastLoginTime = NTP.ntp.getServerTime().ToString();  // 플레이어 로그인 시간
        Debug.Log("플레이어 마지막 로그인 시간 " + this.player.LastLoginTime);
        player.LastLoginDateTime = NTP.ntp.getServerTime().ToString("yyyy-MM-dd");
        Debug.Log("플레이어 마지막 로그인 날짜" + this.player.LastLoginDateTime);

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
            Debug.Log("자동저장 되었습니다.");
            //save_txt.text = "자동 저장 되었습니다." + (++count);
        }
    }

    bool Calculate_TimeDifference(UserData playerData) // 출석보상 지급 판단을 위한 마지막 로그인과 현재 로그인 시간을 대조함수
    {
        var playerLastLoginTime = int.Parse(playerData.LastLoginDateTime.Replace("-", "")); // 플레이어 세이브 내 저장된 시간에서 날짜만 추출 후 int로 캐스팅
        var DateTime_Now = int.Parse(NTP.ntp.getServerTime().ToString("yyyy-MM-dd").Replace("-", ""));
        Debug.Log("플레이어 마지막 로그인 " + playerLastLoginTime + ", 지금 날짜 " + DateTime_Now);
        //var DateTime_Now = int.Parse(NTP.ntp.getServerTime().ToShortDateString().Replace("-", "")); // 현재 시간에서 날짜만 추출 후 int로 캐스팅
        //debug
        //var DateTime_Now = int.Parse(DateTime.Now.ToShortDateString().Replace("-", ""));


        if (playerLastLoginTime < DateTime_Now) // 년도(4자리)+월(2자리)+일(2자리) 총 8자리 int형 수 비교 후 현재시간 값이 더 크면 더 나중이니 true리턴
            return false;

        return true;
    }

    bool Ingredient_need_renew(UserData Lastplayer)
    {
        if(Lastplayer.Last_ingredientShop_intime == "") 
        {
            Debug.Log("상점 시간 오류 ! 그냥 갱신합니다.");
            return true;
        }

        TimeSpan timecal = NTP.ntp.getServerTime() - DateTime.Parse(Lastplayer.Last_ingredientShop_intime); //DateTime.Parse(Lastplayer.LastLogOutTime);
        int m = DateTime.Parse(Lastplayer.Last_ingredientShop_intime).Minute;
        Debug.Log("시간들 : " + NTP.ntp.getServerTime() + " 플레이어 마지막 상점 출입 시간 : " + Lastplayer.Last_ingredientShop_intime + " 분 : " + m);
        Debug.Log("timecal : " + timecal + "분 : " + timecal.Minutes*60 + " 초 : " + timecal.Seconds);

        if(timecal.Minutes>=30) {   Debug.Log("30분이 지났습니다!"); return true;   }
        else if(m < 30 && m + timecal.Minutes >= 30) {      Debug.Log("분시간이 30분 미만에서 더했을때!"); return true;    }
        else if(m >= 30 && m + timecal.Minutes >= 60) {     Debug.Log("분시간이 30분 이상에서 더했을때!"); return true;     }

        Debug.Log("상점 목록을 갱신할 필요는 없습니다!");
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