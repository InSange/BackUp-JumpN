using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Player")]
    public float JumpPower = 2.5f;  // 점프 힘
    public float DoubleJumpPower = 2.5f;    // 더블 점프 힘
    public int JumpCount = 0;   // 점프 횟수 카운트 -> 1단 점프? 2단 점프? 비교할 때 사용

    public int life = 3;    // 인게임에 사용되는 라이프
    [SerializeField]
    public bool isGodTime = false;  // 피격시 무적 판정

    [SerializeField]
    public Animator anim;   // 고양이 애니메이터
    [SerializeField]
    private Cat cat;    // 고양이 스크립트
    [SerializeField]
    private Image catImage; // 고양이 이미지
    [SerializeField]
    public bool isJump; // 1단 점프 중 확인하는 변수
    [SerializeField]
    public bool doubleJump; // 2단 점프 중 확인하는 변수
    [SerializeField]
    public bool isLand; // 착륙했는지 확인하는 변수
    [SerializeField]
    public RectTransform[] player_position; // 고양이의 위치는 1.중앙과 2.인게임 위치 두 개가 존재.
    [SerializeField]
    private RectTransform player_rect;  // 고양이 포지션을 다루기 위한 렉트 트랜스폼
    [SerializeField]
    public CustomSkin cat_skin; // 고양이 스킨

    // Update is called once per frame
    void Awake()
    {
        anim = GetComponent<Animator>();
        isJump = false;
        doubleJump = false;
        player_rect = GetComponent<RectTransform>();
        catImage = cat.GetComponent<Image>();

        Debug.Log("플레이어 위치들 " + player_position[0].anchoredPosition + ", " + player_position[1].anchoredPosition);
        //SetSkinNum();
    }

    public void Jump()
    {
        if (!isJump) // 점프 중이지 않을 때
        {
            JumpCount++;
            isJump = true;
            cat.DoJump(JumpPower);
            anim.SetBool("isJump", isJump);
            isLand = false;
        }
        else if (isJump && !doubleJump && anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump") && JumpCount == 1)
        {// 1단 점프 중이고 애니메이션이 Player_Jump가 진행중 일 때 실행
            JumpCount++;
            doubleJump = true;
            cat.DoJump(DoubleJumpPower);
            anim.SetBool("isDoubleJump", doubleJump);
        }
    }

    public void Player_Set_Position(int pos) // 플레이어 위치 세팅 -> 1. 중앙 2. 인게임
    {
        player_rect.anchoredPosition = new Vector2(player_position[pos].anchoredPosition.x * InGameManager.inGameManager.screenRect.localScale.x/InGameManager.inGameManager.defaultScreenSize, player_position[pos].anchoredPosition.y);
    }

    public void Damaged()
    {   // 장애물에 맞았을 때 피격 판정
        if (life > 1 && !isGodTime)  // 목숨이 1개 보다 많고 무적이 아닐 때 라이프 감소
        {
            SoundManager.soundManager.SetSFX("hit");
            life -= 1;
            InGameManager.inGameManager.uiManager.SetLife(life);
            isGodTime = true;
            StartCoroutine(GodTime());
        }
        else if (life == 1 && !isGodTime)    // 목숨이 1개이고 무적이 아닐때 게임 오버
        {
            life -= 1;
            InGameManager.inGameManager.uiManager.SetLife(life);
            GameManager.gm.isLive = false;
            InGameManager.inGameManager.EndGameSet();
        }
    }

    public RectTransform Get_Player_Rect()  // 플레이어 위치 반환하는 함수
    {
        return player_rect;
    }

    public IEnumerator GodTime()    // 무적 판정 1.5초 동안 진행
    {
        for (int i = 0; i < 15; i++)
        {
            if (i % 2 == 0) // 이미지 컬러 색상 변경
            {
                catImage.color = new Color32(255, 255, 255, 90);
            }
            else
            {
                catImage.color = new Color32(255, 255, 255, 180);
            }

            yield return new WaitForSeconds(0.1f);
        }

        catImage.color = new Color32(255, 255, 255, 255);

        isGodTime = false;

        yield return null;
    }

    public void SetSkinNum()    // 스킨 장착할 때 사용하는 함수
    {
        cat_skin.skinNr = ItemStatusManager.itemStatusManager.character_num_dic[GameManager.gm.currentCharacter];
    }
}