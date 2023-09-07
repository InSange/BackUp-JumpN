using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager inGameManager = null;

    public float defaultScreenSize = 0.005208333f;
    public RectTransform screenRect;
    public float defaultScreenSizeX = 1080.0f;
    public float defaultScreenSizeY = 1920.0f;

    // 각 캔버스를 다루는 매니저 그룹들
    [Header("UIManager")]
    public UIManager uiManager;
    [Header("IngredientShopManager")]
    public IngredientShopManager ingredientShopManager;
    [Header("AchiveInfoManager")]
    public AchiveInfoManager achiveInfoManager;
    [Header("CharacterShopManager")]
    public CharacterShopManager characterShopManager;
    [Header("BookManager")]
    public BookManager bookManager;
    [Header("MapShopManager")]
    public MapShopManager mapShopManager;

    [Header("Player")]
    public Player player;

    [Header("Map")]
    [SerializeField]
    public float speed = 500.0f;    // 시작시 기본 속도

    [Header("Attendance PopUp")]
    [SerializeField]
    private GameObject Attendance_UI;   // 출석 UI
    public int[] month = new int[12] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};  // 월별 요일
    [SerializeField]
    private GameObject attendance_prefab_obj;   // 요일 별 보상 항목 프리팹
    [SerializeField]
    private GameObject attendance_ViewContent;
    [SerializeField]
    private List<AttendanceSlot> attendanceSlotInfo;

    [Header("Fade Out Image")]
    [SerializeField]
    private Image fade_image;
    public bool fade_clear;

    [Header("Screen GameObjects")]
    [SerializeField] GameObject inGame; // 인게임 캔버스를 담당 - 배경, 플레이어 위치, 장애물 위치를 담당.
    [SerializeField] GameObject inGameUi; // 인게임 UI 캔버스를 담당 - 메인 화면 UI, 인게임 화면 UI 들을 담당. 
    [SerializeField] GameObject player_ObstacleUi; // 플레이어 및 장애물 UI를 담당.  위 세개는 같이 다님. 분리 시킨 이유 : 플레이어를 나타내는 캔버스는 배경을 나타내는 캔버스 특성과 서로 다르기 때문.
    [SerializeField] GameObject mapShop;    // 맵 상점 캔버스 담당.
    [SerializeField] GameObject ingredientShop; // 재료 상점 캔버스 담당.
    [SerializeField] GameObject characterShop;  // 캐릭터 상점 캔버스 담당.
    [SerializeField] GameObject bookShop;   // 도감 캔버스 담당.
    [SerializeField] GameObject achive; // 퀘스트 캔버스 담당.

    void Awake()
    {
        if(inGameManager != null && inGameManager != this)
            Destroy(gameObject);
        else   
            inGameManager = this;

        DontDestroyOnLoad(gameObject);

        fade_clear = false;
        StartCoroutine(UserDataManager.userDataManager.AsyncSave());
        //MainMenuSet();
    }

    public void exitShop()
    {  
        switch(GameManager.gm.current_screen)
        {
            case "MapShop":
                mapShop.SetActive(false);
                break;
            case "BookShop":
                bookShop.SetActive(false);
                break;
            case "CharacterShop":
                characterShop.SetActive(false);
                break;
            case "IngredientShop":
                ingredientShop.SetActive(false);
                break;
            case "Achive":
                achive.SetActive(false);
                break;
        }
        MainMenuSet();  // 메인화면 세팅
        InGameSetOnButton();    
        GameManager.gm.current_screen = "MainMenu";
    }

    public void StartGameSet()
    {   // 인 게임 시작시 게임 UI랑 스피드 설정, 목숨 부여, 플레이어 위치 변경 등
        GameManager.gm.current_screen = "InGame";
        speed = 500.0f;
        player.life = 3;
        player.isGodTime = false;
        uiManager.StartGameSetUI();
        uiManager.SetLife(player.life);
        ObstacleManager.obstacleManager.StartGame();
        player.Player_Set_Position(1);
        uiManager.Map_init_Set();
    }

    public void EndGameSet()
    {   // 플레이어 기본 위치 세팅과 결과 저장. 게임 종료 UI 세팅
        uiManager.EndGameSetUI();
        ObstacleManager.obstacleManager.EndGame();
        player.Player_Set_Position(0);
        UserDataManager.userDataManager.SaveData();
    }

    public void MainMenuSet()
    {   // 메인화면 세팅. 튜토리얼을 클리어하지 않을 시 튜토리얼 UI를 시작한다. 클리어 했을 시 메인화면 UI 세팅과 메인화면 브금 실행
        GameManager.gm.current_screen = "MainMenu";
        if(!GameManager.gm.tutorial)
        {
            //rewardPanel.SetActive(true);
            //RewardManager.rewardManager.Tuto_Reward();
            uiManager.TutorialSetUI();
            return;
        }
        InGameSetOnButton();
        uiManager.MainMenuSetUI();
        player.Player_Set_Position(0);
        uiManager.Map_init_Set();
        SoundManager.soundManager.SetBGM("Main");
        //if(!GameManager.gm.GetAttendanceReward) OpenAttendanceUI();
    }

    public void OpenAttendanceUI()
    {   // 출석 기능
        Attendance_UI.SetActive(true);
        if(attendanceSlotInfo.Count > 0) return;
        for(int i = 0; i < month[NTP.ntp.getServerTime().Month-1]; i++)
        {
            GameObject attendance_obj = Instantiate(attendance_prefab_obj);
            attendance_obj.transform.SetParent(attendance_ViewContent.transform, false);
            AttendanceSlot slot = attendance_obj.GetComponent<AttendanceSlot>();

            attendanceSlotInfo.Add(slot);
        }
    }

    public void Map_Change(int n)
    {   // 맵 변경
        uiManager.Map_Change(n);
    }

    public IEnumerator FadeOut()
    {   // 페이드 아웃
        float fadetime = 0f;
        while (fadetime < 0.5f)
        {
            //Debug.Log(fadetime);
            fadetime += 0.05f;
            yield return new WaitForSeconds(0.01f);
            fade_image.color = new Color(0, 0, 0, (fadetime*2));
        }

        //yield return new WaitForSeconds(2.0f);
    }

    public void FadeClear()
    {   // 페이드 아웃 처리
        fade_image.color = new Color(0, 0, 0, 0);
        fade_clear = false;
    }

    public void InGameSetOFFButton()
    {   
        inGame.SetActive(false);
        inGameUi.SetActive(false);
        player_ObstacleUi.SetActive(false);
    }

    public void InGameSetOnButton()
    {
        inGame.SetActive(true);
        inGameUi.SetActive(true);
        player_ObstacleUi.SetActive(true);
    }

    // ---------------------------------- TEST -----------------------------------
    public void GetGold()
    {
        GameManager.gm.gold += 1000;
        GameManager.gm.stackedGold += 1000;
        Debug.Log("1000골드 추가");
    }

    public void clearData()
    {
        UserDataManager.userDataManager.CreateNewData();
        UserDataManager.userDataManager.SaveData();
        Application.Quit();
    }
}
