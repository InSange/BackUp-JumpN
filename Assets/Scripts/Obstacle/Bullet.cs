using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private BulletGroup group;  // �ش� ��ֹ��� ���ϴ� �׷�
    [SerializeField]
    public Image image; // ��ֹ� �̹���    
    [SerializeField]
    public BoxCollider2D col;   // ��ֹ� �ǰ� ũ��

    public void Bullet_On()
    {
        gameObject.SetActive(true);
    }

    public void Bullet_Off()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if(obj.name == "Cat")   // ����̶� �΋H ������
        {
            InGameManager.inGameManager.player.Damaged(); // �÷��̾� life ����
            group.isHit = true; // ���� ȹ�� x
            Bullet_Off();   // ������Ʈ ��Ȱ��
        }
    }
}
