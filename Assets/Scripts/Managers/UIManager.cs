using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{   // 터치 판넬
    [Header("TouchPanel")]
    [SerializeField]
    private GameObject touchPanel;  // 플레이어 점프 인식용 터치 판넬
    // 메인메뉴 UI
    [Header("MainMenuPanel")]
    [SerializeField]
    private GameObject mainPanel;   // 메인 화면 판넬
    [SerializeField]
    private TMP_Text scoreText; // 최대 점수 스코어
    [SerializeField]
    private TMP_Text About_Info_Text;   // 우리 정보 버튼용 텍스트

    // 인게임 UI
    [Header("InGamePanel")]
    [SerializeField]
    private GameObject startGamePanel;  // 게임 시작화면 UI 판넬
    [SerializeField]
    private GameObject scoreGroup;  // 현재 점수판
    [SerializeField]
    private TMP_Text currentScoreText;  //  현재 점수
    [SerializeField]
    private int score = 0;  // 점수
    [SerializeField]
    private int change_money;   // 점수 To 돈
    [SerializeField]
    private TMP_Text lifeCount; // 인게임 목숨 개수
    [SerializeField]
    private Button pauseButton; // 일시 정지 버튼
    [SerializeField]
    private GameObject pausePanel;  // 일시 정지 화면
    // 게임종료 UI
    [Header("EndGamePanel")]
    [SerializeField]
    private GameObject endGamePanel;    // 게임 끝 화면
    [SerializeField]
    private GameObject endButton;   // 게임 종료 버튼
    [SerializeField]
    private TMP_Text final_score;   // 최종 점수 텍스트
    [SerializeField]
    private TMP_Text money_change_text; // 최종 돈 변환 텍스트
    // 튜토리얼 UI
    [Header("TutorialPanel")]
    [SerializeField]
    private GameObject TutorialPanel;   // 튜토리얼 UI 판넬

    [Header("그 외 UI")]
    [SerializeField]
    private TMP_Text current_Gold;  // 현재 보유 골드
    [SerializeField]
    private TMP_Text gold_Text; // 현재 보유하고 있는 골드 텍스트
    [SerializeField]
    private GameObject gold_Top_Panel;  // 캐릭터 상점, 맵 상점, 재료 상점에 쓰일 TOP UI (보유 골드 ui, 광고 ui)

    // 도움말
    [Header("도움말")]
    [SerializeField]
    private bool isHelp = false;    // 도움말 ON?
    [SerializeField]
    private TMP_Text[] helpTexts;   // isHelp = True 일때 해당 버튼 소개해주는 text들

    // 배경 UI 관리
    [Header("배경 UI")]
    [SerializeField]
    private Sprite tuto_sprite; // 튜토리얼용 스프라이트

    [SerializeField]
    private MapMove BG_Move;    // 배경 움직임
    [SerializeField]
    private MapMove Bottom_BG_Move; // 아래 배경 움직임
    [SerializeField]
    private GameObject marginBG;    // 일시 정지, 게임 결과용 백그라운드 여백이미지

    [Header("TEST")]
    public TMP_Text[] testTEXT;
    int testNum = 0;

    [Header("알림 아이콘")]
    [SerializeField]
    private GameObject questAlertIcon;  // 완료 가능한 퀘스트 알림
    [SerializeField]
    private GameObject catAlertIcon;    // 구매 가능한 고양이 알림
    [SerializeField]
    private GameObject mapAlertIcon;    // 구매 가능한 맵 알림
    [SerializeField]
    private GameObject cookAlertIcon;   // 만들 수 있는 요리 알림
    [SerializeField]
    private GameObject stuffAlertIcon;  // 재료상점 업데이트 알림

    // 튜토리얼 배경 세팅
    public void Tuto_Back_Set()
    {
        BG_Move.ChangeBackGroundImage(tuto_sprite);
        BG_Move.ResetPos(300.0f, 300.0f);
        Bottom_BG_Move.ResetPos(500.0f, 500.0f);
    }
    // 맵 초기 위치 세팅
    public void Map_init_Set()
    {
        BG_Move.ResetPos(300.0f, 300.0f);
        Bottom_BG_Move.ResetPos(500.0f, 500.0f);
    }
    // 맵 이미지 변경
    public void Map_Change(int n)
    {
        BG_Move.ChangeBackGroundImage(ItemStatusManager.itemStatusManager.getStage(n + 1).back_sprite);
    }

    void Awake()
    {
        isHelp = false;
        float defaultVal = InGameManager.inGameManager.defaultScreenSizeX / InGameManager.inGameManager.defaultScreenSizeY;
        float curVal = InGameManager.inGameManager.screenRect.rect.width / InGameManager.inGameManager.screenRect.rect.height;
        if(curVal < defaultVal)
        {
            RectTransform goldPanelRect = gold_Top_Panel.GetComponent<RectTransform>();
            goldPanelRect.sizeDelta = new Vector2(goldPanelRect.rect.width, goldPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
        }
    }

    // 튜토리얼 세팅
    public void TutorialSetUI()
    {
        TutorialPanel.SetActive(true);
        Tuto_Back_Set();
    }
    // 알림 아이콘 세팅
    public void AlertUISet()
    {
        Debug.Log("현재 타임을 알려줘 " + InGameManager.inGameManager.ingredientShopManager.Last_in_time);
        if (InGameManager.inGameManager.ingredientShopManager.Last_in_time == "")
        {
            stuffAlertIcon.SetActive(true);
        }
        else
        {
            TimeSpan timecal = NTP.ntp.getServerTime() - DateTime.Parse(InGameManager.inGameManager.ingredientShopManager.Last_in_time); //DateTime.Parse(Lastplayer.LastLogOutTime);
            int m = DateTime.Parse(InGameManager.inGameManager.ingredientShopManager.Last_in_time).Minute;

            if (timecal.Minutes >= 30) { stuffAlertIcon.SetActive(true); }
            else if (m < 30 && m + timecal.Minutes >= 30) { stuffAlertIcon.SetActive(true); }
            else if (m >= 30 && m + timecal.Minutes >= 60) { stuffAlertIcon.SetActive(true); }
            else stuffAlertIcon.SetActive(false);
        }

        if (InGameManager.inGameManager.achiveInfoManager.CheckCanReceiveReward() == true) questAlertIcon.SetActive(true); else questAlertIcon.SetActive(false);

        if (InGameManager.inGameManager.characterShopManager.CheckCanBuyCat()) catAlertIcon.SetActive(true); else catAlertIcon.SetActive(false);

        if (InGameManager.inGameManager.bookManager.CheckCanReceiveCook()) cookAlertIcon.SetActive(true); else cookAlertIcon.SetActive(false);

        if (InGameManager.inGameManager.mapShopManager.CheckCanBuyMap()) mapAlertIcon.SetActive(true); else mapAlertIcon.SetActive(false);
    }

    // 메인메뉴 UI 세팅
    public void MainMenuSetUI()
    {
        scoreText.text = GameManager.gm.score.ToString("#,##0") + "!";
        AlertUISet();
        marginBG.SetActive(false);
        endGamePanel.SetActive(false);
        startGamePanel.SetActive(false);
        mainPanel.SetActive(true);
        touchPanel.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        Gold_Top_Panel_Off();
    }
    // 게임시작 UI 세팅
    public void StartGameSetUI()
    {
        currentScoreText.text = "0";
        score = 0;
        GameManager.gm.isLive = true;
        mainPanel.SetActive(false);
        endGamePanel.SetActive(false);
        startGamePanel.SetActive(true);
        touchPanel.SetActive(true);
        pauseButton.gameObject.SetActive(true);
    }
    // 게임 일시정지
    public void PauseGameButton()
    {
        GameManager.gm.isPuase = true;
        Time.timeScale = 0;
        marginBG.SetActive(true);
        pausePanel.SetActive(true);
        touchPanel.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }
    // 게임 계속하기
    public void ResumeGameButton()
    {
        GameManager.gm.isPuase = false;
        marginBG.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        touchPanel.SetActive(true);
        pauseButton.gameObject.SetActive(true);
    }
    // 게임 종료하기
    public void ExitGameButton()
    {
        GameManager.gm.isPuase = false;
        marginBG.SetActive(false);
        pausePanel.SetActive(false);
        InGameManager.inGameManager.player.life = 0;
        SetLife(0); //Player.player.life
        GameManager.gm.isLive = false;
        InGameManager.inGameManager.EndGameSet();
        Time.timeScale = 1;
        pauseButton.gameObject.SetActive(false);
    }

    // 게임오버 UI 세팅
    public void EndGameSetUI()
    {
        current_Gold.text = GameManager.gm.gold.ToString("#,##0");
        marginBG.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        startGamePanel.SetActive(false);
        mainPanel.SetActive(false);
        endGamePanel.SetActive(true);
        touchPanel.SetActive(false);
        final_score.text = score.ToString("#,##0") + "m";
        if (GameManager.gm.score < score) GameManager.gm.score = score;
        ScoreChangeToMoney();
        //StartCoroutine(EndGameButtonSetUI());
    }
    // 총알 피할시 추가점수
    public void ChangeCurrentScore()
    {
        currentScoreText.text = score.ToString("#,##0");
        if (score / 30 > ObstacleManager.obstacleManager.bullet_level && ObstacleManager.obstacleManager.bullet_level < 4)
        {
            //InGameManager.inGameManager.speed += 200.0f;//ObstacleManager.obstacleManager.bullet_speeds[ObstacleManager.obstacleManager.bullet_level];
            BG_Move.AddSpeed(200.0f);
            Bottom_BG_Move.AddSpeed(300.0f);
            ObstacleManager.obstacleManager.bullet_level++;
            Debug.Log("속도 업 : " + InGameManager.inGameManager.speed + ", " + score / 30);
        }
    }
    // 점수 획득
    public void AddScore(int bonus)
    {
        score += bonus;
        GameManager.gm.avoidCnt++;
    }
    // 라이프 이미지 설정
    public void SetLife(int n)
    {
        lifeCount.text = "x " + n;
    }
    // 점수를 돈으로 환산하는 함수
    public void ScoreChangeToMoney()
    {
        change_money = 0;

        final_score.text = score.ToString("#,##0");

        if (score <= 500) change_money = score;
        else change_money = 500 + score - 500;

        GameManager.gm.gold += change_money;
        GameManager.gm.stackedGold += change_money;

        money_change_text.text = "+" + change_money.ToString("#,##0");
        current_Gold.text = GameManager.gm.gold.ToString("#,##0");
        endButton.SetActive(true);
    }

    // 튜토리얼 UI 온
    public void Tuto_UI_ON()
    {
        StartCoroutine(Tuto_UI());
    }
    IEnumerator Tuto_UI()
    {
        startGamePanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        scoreGroup.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        scoreGroup.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        pauseButton.gameObject.SetActive(true);
    }
    // 도움말 온
    public void Help_Button()
    {
        if (!isHelp)
        {
            for (int i = 0; i < helpTexts.Length; i++)
            {
                helpTexts[i].gameObject.SetActive(true);
            }
            //About_Info_Text.text = "? ????";
            About_Info_Text.gameObject.SetActive(false);
            isHelp = true;
        }
        else
        {
            for (int i = 0; i < helpTexts.Length; i++)
            {
                helpTexts[i].gameObject.SetActive(false);
            }
            //About_Info_Text.text = "ABOUT\nUS";
            About_Info_Text.gameObject.SetActive(true);
            isHelp = false;
        }
    }
    public void Gold_Top_Panel_On() // 골드 판넬 ON
    {
        if(gold_Top_Panel.activeSelf == false) gold_Top_Panel.SetActive(true);
        gold_Text.text = GameManager.gm.gold.ToString("#,##0");
    }

    public void Gold_Top_Panel_Off()    // 골드 판넬 OFF
    {
        gold_Top_Panel.SetActive(false);
    }
    // 출석보상 받기
    public void GetAttendanceReward()
    {
        GameManager.gm.GetAttendanceReward = true;
    }
}