using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TutorialManager : MonoBehaviour, IPointerDownHandler
{
    [Header("Tutorial Screen")]
    [SerializeField]
    private GameObject tuto_theater;    // 애니메이션 부분을 관리하는 오브젝트 그룹
    [SerializeField]
    private Image tutorial_screen;  // 애니메이션을 그려내는 스크린
    [SerializeField]
    private GameObject cookImage;   // 요리 이미지 (튜토리얼 완료할때 쯤)
    [SerializeField]
    public TMP_Text screen_Text;    // 대사 - local 데이터가 -1일때
    [SerializeField]
    public TMP_Text anim_Text;  // 애니메이션 대사
    [SerializeField]
    public Sprite tutorial_image;   // 튜토리얼용 이미지 노란색 배경
    [SerializeField]
    public List<Sprite> animation_Images = new List<Sprite>();  // 애니메이션들을 저장해놓을 공간
    [SerializeField]
    public int tuto_anim_index = 0; // 현재 애니메이션 인덱스 번호
    [SerializeField]
    public string current_tuto = "";    // 현재 튜토리얼 진행 상황
    [SerializeField]
    private string[] tuto_progress = {"Tuto_Anim", "TutoJump", "Tuto_Avoid", "Tuto_UI"};    // 튜토리얼 진행 목차, 애니메이션, 점프, 회피, 게임 실행
    [SerializeField]
    public List<string> sinaID = new List<string>();    // 시나리오 ID들을 저장해놓을 리스트
    [SerializeField]
    public IEnumerator cor_tuto;    // 튜토리얼용 코루틴. 
    [SerializeField]
    private bool canSkip = false;   // 스킵 가능 여부
    [SerializeField]
    private bool isfadeOut = false; // 페이드 아웃 상태

    [Header("Touch Quest UI")]
    [SerializeField]
    public GameObject tuto_bullet;  // 튜토리얼용 불렛
    public Rigidbody2D tuto_bullet_rigid;   // 튜토불렛용 리지드바디
    [SerializeField]
    private bool needJump = false;  // 점프 체크
    [SerializeField]
    private bool one_Jump = false;  // 1단 점프 구간 체크
    [SerializeField]
    private bool double_Jump = false;   // 2단 점프 구간 체크
    [SerializeField]
    private bool avoid = true;  // 총알 피하기 체크
    [SerializeField]
    private TMP_Text Tuto_Text; // 점프 및 회피용 텍스트

    [Header("Fade Out Image")]
    [SerializeField]
    private Image fade_image;   // 페이드 아웃용 이미지 (검은색)

    [Header("Player")]
    [SerializeField]
    private Player player;  // 플레이어 데이터

    [Header("Collider")]
    [SerializeField]
    private BoxCollider2D col;  // 튜토 불렛 콜라이더
    
    void OnEnable()
    {
        col = GetComponent<BoxCollider2D>();
        current_tuto = tuto_progress[0];
        for(int i = 1; i <= GameManager.gm.dataLoader.sinaData.Count; i++)  // 시나리오 키값들을 리스트에다가 저장한다.
        {
            sinaID.Add("scnrid_"+ string.Format("{0:D2}", i));
            animation_Images.Add(tutorial_image);
        }
        Start_Tuto(current_tuto);
    }

    void Start_Tuto(string cur_tuto)
    {   // 튜토리얼 시작하는 함수 현재 튜토리얼 cur_tuto에 따라 진행됨.
        current_tuto = cur_tuto;
        
        if(current_tuto == tuto_progress[0])    // 애니메이션 튜토리얼
        {
            tuto_theater.SetActive(true);   // 애니메이션을 띄울 오브젝트 활성화
            canSkip = true; // 애니메이션은 스킵이 가능하다.
            if(cor_tuto != null) StopCoroutine(cor_tuto);   // 진행 중인 코루틴 정지
            if(current_tuto == tuto_progress[0]) cor_tuto = Tuto_Anim();    // 현재 튜토리얼이 애니메이션이면 애니메이션 코루틴으로 진행
            else cor_tuto = Tuto_Anim();    // 예외도 애니메이션으로
            StartCoroutine(cor_tuto);   // 애니메이션 시작
        }
        else if(current_tuto == tuto_progress[1])   // 점프 튜토리얼
        {
            player.Player_Set_Position(1);  // 플레이어를 인게임 구간에 배치
            StopCoroutine(cor_tuto);
            cor_tuto = Tuto_Jump(); // 점프 튜토리얼 실행
            StartCoroutine(cor_tuto);
        }
        else if(current_tuto == tuto_progress[2])   // 회피 튜토리얼
        {
            player.Player_Set_Position(1);  // 플레이어를 인게임 구간에 배치
            tuto_theater.SetActive(false);
            StopCoroutine(cor_tuto);
            cor_tuto = Tuto_Avoid();    // 회피 튜토리얼 진행
            StartCoroutine(cor_tuto);
        }
        else if(current_tuto == tuto_progress[3])   // 플레이어를 인게임으로 시작
        {
            StopCoroutine(cor_tuto);
            cor_tuto = Tuto_UI();   // 인게임 UI 세팅
            StartCoroutine(cor_tuto);
        }
    }

    void Change_Anim()
    {   // 애니메이션 진행 함수. 다음 애니메이션 항목이 -1이 아니면 애니메이션 진행. -1이면 점프나 회피 튜토리얼이 실행되기에 애니메이션 창 비활성화
        if(GameManager.gm.dataLoader.sinaData[sinaID[tuto_anim_index]][2] == "-1")
        {
            screen_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            screen_Text.gameObject.SetActive(true);
            tutorial_screen.gameObject.SetActive(false);
        }
        else
        {
            anim_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            tutorial_screen.sprite = animation_Images[tuto_anim_index];
            tutorial_screen.gameObject.SetActive(true);
            screen_Text.gameObject.SetActive(false);
        }
    }

    IEnumerator FadeOut()   // 페이드아웃 적용. alpha값을 1까지 올림. (검은색 이미지)
    {
        Debug.Log("페이드 아웃" + sinaID[tuto_anim_index]);
        float fadetime = 0f;
        while (fadetime < 0.5f)
        {
            //Debug.Log(fadetime);
            fadetime += 0.01f;
            yield return new WaitForSeconds(0.01f);
            fade_image.color = new Color(0, 0, 0, (fadetime*2));
        }

        yield return new WaitForSeconds(2.0f);
        tuto_theater.SetActive(false);
        fade_image.color = new Color(0, 0, 0, 0);

        if(current_tuto == tuto_progress[0])    // 애니메이션에서 페이드아웃이 필요할 때 적용하고 페이드아웃이 끝난 뒤 다른 튜토리얼 목차로 변경
        {   // 애니메이션 -> 점프, 점프 -> 애니메이션, 애니메이션 -> 회피 등
            if(sinaID[tuto_anim_index] == "scnrid_07") Start_Tuto(tuto_progress[1]);
            else if(sinaID[tuto_anim_index] == "scnrid_18") Start_Tuto(tuto_progress[2]);
            else if(sinaID[tuto_anim_index] == "scnrid_28")
            {
                GameManager.gm.tutorial = true;
            
                InGameManager.inGameManager.MainMenuSet();
                InGameManager.inGameManager.fade_clear = true;
                InGameManager.inGameManager.FadeClear();

                this.gameObject.SetActive(false);
            }
        }
        else if(current_tuto == tuto_progress[1])
        {   // 점프 튜토리얼이 끝나면 플레이어를 원위치로 복귀후 애니메이션 튜토리얼 다시 실행
            player.Player_Set_Position(0);
            tuto_anim_index++;
            Start_Tuto(tuto_progress[0]);
        }
        else
        {   // 회피, 인게임 튜토리얼 역시 애니메이션 튜토리얼로 다시 실행
            Start_Tuto(tuto_progress[0]);
        }
    }

    IEnumerator Tuto_Anim()
    {   // 애니메이션 튜토리얼에서 점프, 회피, 인게임으로 넘어가기 전까지는 스킵이 가능. 스킵이 불가능할때 페이드 아웃 적용후 다음 목차 튜토리얼 실행
        if(tuto_anim_index < 24)
        {
            if((sinaID[tuto_anim_index] == "scnrid_07" || sinaID[tuto_anim_index] == "scnrid_18") && isfadeOut == false )
            {
                Change_Anim();
                isfadeOut = true;
            }
            else if(isfadeOut)
            {
                canSkip = false;
                isfadeOut = false;
                StartCoroutine(FadeOut());
            }
            else
            {
                Change_Anim();
            }
        }
        else
        {
            if(sinaID[tuto_anim_index] == "scnrid_28")
            {
                canSkip = false;
                cookImage.SetActive(false);
                Change_Anim();
                new WaitForSeconds(2.0f);
                StartCoroutine(FadeOut());
            }
            else
            {
                Change_Anim();
                if(sinaID[tuto_anim_index] == "scnrid_27") cookImage.SetActive(true);
            }
        }

        yield return null;
    }

    IEnumerator Tuto_Avoid()
    {   // 회피 튜토리얼. 튜토리얼용 총알이 나와서 회피를 테스트 한다.
        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];

        yield return new WaitForSeconds(2.0f);

        tuto_bullet.SetActive(true);

        yield return null;
    }

    IEnumerator Tuto_Jump()
    {   // 점프 튜토리얼. 1단 점프, 2단 점프 진행. needJump로 체크
        WaitForSeconds waitForSec = new WaitForSeconds(1.0f);
        yield return waitForSec;

        if(!needJump && (!one_Jump || !double_Jump))
        {
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            if(sinaID[tuto_anim_index] == "scnrid_09" || sinaID[tuto_anim_index] == "scnrid_11") 
            {
                needJump = true;
            }
            else Start_Tuto(current_tuto);
        }
        else if(sinaID[tuto_anim_index] == "scnrid_13")
        {
            StartCoroutine(StoreIn());
        }
        else
        {
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            Start_Tuto(current_tuto);
        }
    }

    IEnumerator StoreIn()
    {   // 상점 들어가는 애니메이션
        WaitForFixedUpdate move_store_frame = new WaitForFixedUpdate();

        yield return new WaitForSeconds(1.0f);

        RectTransform player_rect = player.Get_Player_Rect();
        float player_x = player_rect.anchoredPosition.x;
        float player_y = player_rect.anchoredPosition.y;

        while(player_x < 980)
        {
            player_x += 10.0f;
            player_rect.anchoredPosition = new Vector2(player_x, player_y);
            yield return move_store_frame;
        }

        Tuto_Text.text = "";

        StartCoroutine(FadeOut());
    }

    IEnumerator Tuto_UI()
    {   // 인게임 UI 설명 튜토리얼
        yield return new WaitForSeconds(2.0f);
        InGameManager.inGameManager.uiManager.Tuto_UI_ON();
        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
        yield return new WaitForSeconds(2.0f);

        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
        yield return new WaitForSeconds(2.0f);

        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
        yield return new WaitForSeconds(2.0f);

        Tuto_Text.text = "";
        InGameManager.inGameManager.uiManager.Map_Change(0);
        tuto_anim_index++;
        this.gameObject.SetActive(false);
        InGameManager.inGameManager.StartGameSet();
    }
    
    public void OnPointerDown(PointerEventData data)
    {   // 플레이어 화면 터치를 했을 때 플레이어 점프와 동시에 점프 튜토리얼 체크
        if(current_tuto == tuto_progress[0] && canSkip)
        {
            Debug.Log("애니메이션 스킵!");
            if(isfadeOut == false) tuto_anim_index++;
            Start_Tuto(tuto_progress[0]);
            return;
        }
        if(needJump) player.Jump();

        if(!one_Jump && player.isJump && current_tuto == tuto_progress[1] && needJump)
        {
            one_Jump = true;
            needJump = false;
            Start_Tuto(current_tuto);
        }
        else if(!double_Jump && player.doubleJump && current_tuto == tuto_progress[1] && needJump)
        {
            double_Jump = true;
            Start_Tuto(current_tuto);
        }
        else if(!avoid && current_tuto == tuto_progress[2] && needJump)
        {
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            tuto_bullet_rigid.AddForce(Vector2.left*3.0f, ForceMode2D.Impulse);
            avoid = true;
            needJump = false;
            current_tuto = tuto_progress[3];
            Start_Tuto(current_tuto);
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {   // 튜토 불렛용으로 일정 구간에 들어오면 잠시 멈추었다가 플레이어 점프 시 다시 움직임.
        if(obj.name == "Tuto_bullet")
        {
            tuto_bullet_rigid.velocity = Vector2.zero;
            col.enabled = false;
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            avoid = false;
            needJump = true;
        }
    }
}
