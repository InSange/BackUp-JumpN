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

    // �� ĵ������ �ٷ�� �Ŵ��� �׷��
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
    public float speed = 500.0f;    // ���۽� �⺻ �ӵ�

    [Header("Attendance PopUp")]
    [SerializeField]
    private GameObject Attendance_UI;   // �⼮ UI
    public int[] month = new int[12] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};  // ���� ����
    [SerializeField]
    private GameObject attendance_prefab_obj;   // ���� �� ���� �׸� ������
    [SerializeField]
    private GameObject attendance_ViewContent;
    [SerializeField]
    private List<AttendanceSlot> attendanceSlotInfo;

    [Header("Fade Out Image")]
    [SerializeField]
    private Image fade_image;
    public bool fade_clear;

    [Header("Screen GameObjects")]
    [SerializeField] GameObject inGame; // �ΰ��� ĵ������ ��� - ���, �÷��̾� ��ġ, ��ֹ� ��ġ�� ���.
    [SerializeField] GameObject inGameUi; // �ΰ��� UI ĵ������ ��� - ���� ȭ�� UI, �ΰ��� ȭ�� UI ���� ���. 
    [SerializeField] GameObject player_ObstacleUi; // �÷��̾� �� ��ֹ� UI�� ���.  �� ������ ���� �ٴ�. �и� ��Ų ���� : �÷��̾ ��Ÿ���� ĵ������ ����� ��Ÿ���� ĵ���� Ư���� ���� �ٸ��� ����.
    [SerializeField] GameObject mapShop;    // �� ���� ĵ���� ���.
    [SerializeField] GameObject ingredientShop; // ��� ���� ĵ���� ���.
    [SerializeField] GameObject characterShop;  // ĳ���� ���� ĵ���� ���.
    [SerializeField] GameObject bookShop;   // ���� ĵ���� ���.
    [SerializeField] GameObject achive; // ����Ʈ ĵ���� ���.

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
        MainMenuSet();  // ����ȭ�� ����
        InGameSetOnButton();    
        GameManager.gm.current_screen = "MainMenu";
    }

    public void StartGameSet()
    {   // �� ���� ���۽� ���� UI�� ���ǵ� ����, ��� �ο�, �÷��̾� ��ġ ���� ��
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
    {   // �÷��̾� �⺻ ��ġ ���ð� ��� ����. ���� ���� UI ����
        uiManager.EndGameSetUI();
        ObstacleManager.obstacleManager.EndGame();
        player.Player_Set_Position(0);
        UserDataManager.userDataManager.SaveData();
    }

    public void MainMenuSet()
    {   // ����ȭ�� ����. Ʃ�丮���� Ŭ�������� ���� �� Ʃ�丮�� UI�� �����Ѵ�. Ŭ���� ���� �� ����ȭ�� UI ���ð� ����ȭ�� ��� ����
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
    {   // �⼮ ���
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
    {   // �� ����
        uiManager.Map_Change(n);
    }

    public IEnumerator FadeOut()
    {   // ���̵� �ƿ�
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
    {   // ���̵� �ƿ� ó��
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
        Debug.Log("1000��� �߰�");
    }

    public void clearData()
    {
        UserDataManager.userDataManager.CreateNewData();
        UserDataManager.userDataManager.SaveData();
        Application.Quit();
    }
}
