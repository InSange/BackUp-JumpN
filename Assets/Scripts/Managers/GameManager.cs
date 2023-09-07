using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager gm = null;

    public enum Country
    {
        Korean = 1,
        English
    };

    public bool isGuest = false;    // 게스트 로그인 인가?

    [Header("Data")]
    [SerializeField]
    private bool isLoadingFinish = false;   // 데이터 불러왔는지 확인하는 변수.
    //public DataLoader dataLoader;
    public DataTest dataLoader;
    public UserData playerData;
    public bool tutorial = false;
    public Country countryLang = Country.English;
    public bool isPuase = false;

    public int gold = 1;        // 골드
    public int stackedGold = 0; // 누적 골드
    public float score = 1;     // 최고 점수
    public int ingredient_char_touch = 0;   // 재료 상점 캐릭터 터치횟수
    public int buyCnt = 0;  // 재료 상점 구매 횟수
    public int jumpCnt = 0; // 점프 누적 횟수
    public int avoidCnt = 0;    // 장애물 회피 누적 횟수

    [Header("Player Command")]
    [SerializeField]
    public string current_screen;

    [Header("IngredientShopInfo")]
    public bool shop_need_chage;

    [Header("Map")]
    //public string currentMap;
    public int mapNum;

    [Header("Player")]
    [SerializeField]
    public bool isLive = false;
    public string currentCharacter = "BgSkinName0";

    [Header("Attendance")]
    [SerializeField]
    public bool GetAttendanceReward = false;

    void Awake()
    {
        if (gm != null && gm != this)
            Destroy(gameObject);
        else
            gm = this;

        DontDestroyOnLoad(gameObject);

        Debug.Log("국가 시스템 정보" + Application.systemLanguage);
        LangCheck();
        Init();
        isLoadingFinish = true;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)) 
        {   // 뒤로 나가기 버튼 눌렸을때
            switch(current_screen)
            {
                case "login":
                    UserDataManager.userDataManager.SaveData();
                    Application.Quit();
                    break;
                case "MainMenu":
                    UserDataManager.userDataManager.SaveData(); 
                    Application.Quit();
                    break;
                case "Tutorial":
                    UserDataManager.userDataManager.SaveData();
                    Application.Quit();
                    break;
                case "InGame":
                    InGameManager.inGameManager.uiManager.PauseGameButton();
                    break;
                default:
                    InGameManager.inGameManager.exitShop();
                    break;
            }
        }
#endif
    }

    public void Init()
    {
        current_screen = "login";
    }

    public void setIsLoadingFinish(bool __Signal)
    {
        isLoadingFinish = __Signal;
    }

    public bool getIsLoadingFinish()
    {
        return isLoadingFinish;
    }

    public void LangCheck()
    {   // 기기 별 언어 시스템 적용
        SystemLanguage lang = Application.systemLanguage;

        switch (lang)
        {
            case SystemLanguage.Korean:
                countryLang = Country.Korean;
                break;
            default:
                countryLang = Country.English;
                break;
        }
    }
}