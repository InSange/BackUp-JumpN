using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private BulletGroup group;  // 해당 장애물이 속하는 그룹
    [SerializeField]
    public Image image; // 장애물 이미지    
    [SerializeField]
    public BoxCollider2D col;   // 장애물 피격 크기

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
        if(obj.name == "Cat")   // 고양이랑 부딫 혔을때
        {
            InGameManager.inGameManager.player.Damaged(); // 플레이어 life 감손
            group.isHit = true; // 점수 획득 x
            Bullet_Off();   // 오브젝트 비활성
        }
    }
}
