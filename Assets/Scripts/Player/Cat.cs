using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField]
    private Player player;  // Player ������Ʈ �����ϴ� �Լ�.
    [SerializeField]
    private Rigidbody2D rigid;  // �÷��̾� �̵� �����ϱ� ���� ������ �Լ�

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

    }
    
    public void DoJump(float JumpPower)
    {   // ����̸� ���� ��Ű�� �Լ�.
        rigid.velocity = Vector2.zero; // ���� �Ҷ� ����̰� ������ �ӷ��� 0���� ���� ��
        // ���� ��ġ���� ���� JumpPower ũ�⸸ŭ ���������� ���� �����ش�.
        rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse); 
        GameManager.gm.jumpCnt++;
    }

    void OnCollisionEnter2D(Collision2D obj) {
        if(obj.collider.CompareTag("Skate"))    // ����̰� ���忡 �����ߴٸ�
        {
//            Debug.Log(obj.gameObject.name);
            rigid.velocity = Vector2.zero;  // ����� �ӷ��� 0���� �����
            player.isLand = true;   // ���� �پ�����.
            player.JumpCount = 0;   // ���� Ƚ�� 0���� �ʱ�ȭ
            player.isJump = false;  // ���������� ����
            player.doubleJump = false;  // ���� ���������� ����
            player.anim.SetBool("isJump", false);   // 1�� ���� �ִϸ����� Ʈ���Ű� ����
            player.anim.SetBool("isDoubleJump", false); // 2�� ���� �ִϸ����� Ʈ���Ű� ����
            player.anim.SetBool("isLand", player.isLand);   // �÷��̾ ���忡 �پ��������� �ٲ� isLand �ִϸ��̼� ����
        }
    }
}