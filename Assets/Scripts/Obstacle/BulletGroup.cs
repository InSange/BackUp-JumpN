using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGroup : MonoBehaviour
{
    //[SerializeField]
    //private BulletStorage bulletStorage;
    [SerializeField]
    private float currentbulletSpeed = 3.5f;    // 장애물의 현재 속도.
    [SerializeField]
    private float bulletSpeed = 3.5f;   // 장애물의 기본 속도
    [SerializeField]
    private Rigidbody2D rigid;  // 장애물 물리
    [SerializeField]
    private RectTransform bullet_rect;  // 장애물 위치
    [SerializeField]
    private int size;   // 장애물 크기 -> 1단 점프로 피할 수 있는 사이즈는 0, 2단 점프로 피할 수 있는 사이즈는 1
    [SerializeField]
    public bool isHit = false;  // 플레이어 피격 확인용 변수
    [SerializeField]
    private List<Bullet> bullets;   // 장애물 그룹에 속한 장애물들 총 9개

    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        bullet_rect = gameObject.GetComponent<RectTransform>();
        rigid.velocity = Vector2.zero;
    }

    void OnEnable()
    {
        //rigid.AddForce(Vector2.left*currentbulletSpeed, ForceMode2D.Impulse);
        Debug.Log("출격 속도 " + currentbulletSpeed);
        rigid.velocity = Vector2.left * currentbulletSpeed;   // 장애물 그룹이 활성화되면 속력을 가해준다.
    }

    public void SettingBullet(float randomVal)
    {
        int randomNum;  // 장애물 그룹은 1단 점프, 2단 점프로 나뉜다. 그 안에서 패턴이 존재하기 때문에 패턴을 랜덤으로 택하기 위한 변수.

        if (randomVal < 0.5) // 파라미터로 받은 randomVal 값이 0.5보다 작다면
        {
            size = 0;   // 1단 점프 패턴
            randomNum = Random.Range(0, Pattern.oneSize);   // 1단 점프 패턴들 중에서 1택
                                                            //        Debug.Log("원 불렛 패턴 넘버 : " + randomNum);
            Debug.Log($"[{string.Join(",", Pattern.OneJumpPattern[randomNum].ToArray())}]");

            for (int i = 1; i <= 9; i++) // 패턴에 해당하는 장애물들 활성화
            {
                if (Pattern.OneJumpPattern[randomNum].Contains(i))
                {
                    //int num = Random.Range(0, bulletStorage.GetSize());
                    //bullets[i-1].image.sprite = bulletStorage.bulletDatas[num].bulletImage;
                    //bullets[i-1].col.size = new Vector2(bulletStorage.bulletDatas[num].colX, 30);
                    bullets[i - 1].Bullet_On();
                }
                else bullets[i - 1].Bullet_Off();
            }
        }
        else
        {   // 1단 점프와 동일
            size = 1;   // 2단 점프
            randomNum = Random.Range(0, Pattern.towSize);   // 패턴들 중 하나 택 일
            Debug.Log("투 불렛 패턴 넘버 : " + randomNum);
            Debug.Log($"[{string.Join(",", Pattern.TwoJumpPattern[randomNum].ToArray())}]");

            for (int i = 1; i <= 9; i++)
            {
                if (Pattern.TwoJumpPattern[randomNum].Contains(i)) bullets[i - 1].Bullet_On(); // 패턴 해당? 할성화
                else bullets[i - 1].Bullet_Off();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.tag == "Border")
        {   // 플레이어가 장애물을 피해 장애물이 끝에 도달하게 되었을 때
            //Destroy(gameObject);
            Debug.Log("점수획득 " + (1 + size));
            if (isHit == false) InGameManager.inGameManager.uiManager.AddScore(1 + size);    // 플레이어가 맞지 않았다면 점수 획득
            ObstacleManager.obstacleManager.ReturnObject(this); // 오브젝트 풀링으로 파괴하는게 아닌 오브젝트 비활성화를 해준다.
            bullet_rect.anchoredPosition = Vector2.zero;    // 위치 초기화.
        }
    }

    public void SetBulletSpeedLevel(float speedlevel)
    {   // 불렛 속도 조절. -> 기본 속도에 단계별로 가속해준다.
        currentbulletSpeed = bulletSpeed + speedlevel;
    }
}
// 불렛 패턴 데이터.
public static class Pattern
{
    public static List<List<int>> OneJumpPattern = new List<List<int>>
    {
        new List<int> {4, 7},
        new List<int> {5, 7},
        new List<int> {7, 8},
        new List<int> {4},
        new List<int> {4, 5},
        new List<int> {4, 8}
    };

    public static List<List<int>> TwoJumpPattern = new List<List<int>>
    {
        new List<int> {1, 4, 7},
        new List<int> {2, 5, 7},
        new List<int> {2, 3, 5, 6, 7},
        new List<int> {2, 4, 7},
        new List<int> {2, 4, 5, 7},
        new List<int> {2, 3, 4, 5 , 6, 7},
        new List<int> {2, 4, 5, 6, 8},
        new List<int> {5, 6, 7, 8},
        new List<int> {2, 4, 6, 8}
    };

    public static int oneSize = OneJumpPattern.Count;
    public static int towSize = TwoJumpPattern.Count;

    // 무작위 패턴 36s 패턴 8s 1:1 2:1 3:1 3:2 2:2
}