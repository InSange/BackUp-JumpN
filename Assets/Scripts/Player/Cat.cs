using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField]
    private Player player;  // Player 컴포넌트 저장하는 함수.
    [SerializeField]
    private Rigidbody2D rigid;  // 플레이어 이동 제어하기 위한 리지드 함수

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

    }
    
    public void DoJump(float JumpPower)
    {   // 고양이를 점프 시키는 함수.
        rigid.velocity = Vector2.zero; // 점프 할때 고양이가 가지는 속력을 0으로 만든 뒤
        // 현재 위치에서 위로 JumpPower 크기만큼 순간적으로 힘을 가해준다.
        rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse); 
        GameManager.gm.jumpCnt++;
    }

    void OnCollisionEnter2D(Collision2D obj) {
        if(obj.collider.CompareTag("Skate"))    // 고양이가 보드에 착륙했다면
        {
//            Debug.Log(obj.gameObject.name);
            rigid.velocity = Vector2.zero;  // 고양이 속력을 0으로 만들고
            player.isLand = true;   // 땅에 붙어있음.
            player.JumpCount = 0;   // 점프 횟수 0으로 초기화
            player.isJump = false;  // 점프중이지 않음
            player.doubleJump = false;  // 더블 점프중이지 않음
            player.anim.SetBool("isJump", false);   // 1단 점프 애니메이터 트리거값 변경
            player.anim.SetBool("isDoubleJump", false); // 2단 점프 애니메이터 트리거값 변경
            player.anim.SetBool("isLand", player.isLand);   // 플레이어가 보드에 붙어있음으로 바꿔 isLand 애니메이션 실행
        }
    }
}