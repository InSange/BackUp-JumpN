using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 튜토리얼용 총알 큰 기능은 bullet이랑 크게 다른게 없음. 
public class TutoBullet : MonoBehaviour
{
    [SerializeField]
    private float currentbulletSpeed = 3.5f;    
    [SerializeField]
    private float bulletSpeed = 3.5f;
    [SerializeField]
    private Rigidbody2D rigid;
    [SerializeField]
    private RectTransform bullet_rect;
    [SerializeField]
    private string size;

    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        //rigid.AddForce(Vector2.left*bulletSpeed, ForceMode2D.Impulse);
        bullet_rect = gameObject.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        rigid.AddForce(Vector2.left*currentbulletSpeed, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if(obj.name == "Border")
        {
            Destroy(gameObject);
        }
        else if(obj.name == "Cat")
        {
            InGameManager.inGameManager.player.Damaged();
            Destroy(gameObject);
        }
    }

    public void SetBulletSpeedLevel(float speedlevel)
    {
        currentbulletSpeed = bulletSpeed * speedlevel;
    }

    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }

    public string GetBulletSize()
    {
        return size;
    }
}
