using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{   // 장애물 관리하는 매니저
    public static ObstacleManager obstacleManager = null;

    [SerializeField]
    public Queue<BulletGroup> bulletPool = new Queue<BulletGroup>();    // 오브젝트 풀링
    public Queue<GameObject> bulletObject = new Queue<GameObject>();    // 오브젝트 풀링 관리

    [Header("Bullet")]
    [SerializeField]
    private GameObject bulletPrefab;    // 장애물 프리팹
    [SerializeField]
    private RectTransform BulletStartPosition;  // 장애물 스폰 위치
    [SerializeField]
    IEnumerator bullet_coroutine;
    [SerializeField] bool isfire = false;   // 게임 시작시 장애물 생성 조건
    public int bulletNum = 20;  // 발사할 장애물 개수

    public float[] bullet_speeds = new float[5] {0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
    //public float[] bullet_speeds = new float[5] {1f, 2f, 3f, 4f, 5f};

    public int bullet_level = 0;    // 장애물 단계
    void Awake()
    {
        if(obstacleManager != null && obstacleManager != this)
            Destroy(gameObject);
        else
            obstacleManager = this;
    }

    public void StartGame()
    {
        if(GameManager.gm.isLive)   // 게임이 시작되고 플레이어가 살아 있을 때
        {   // 게임 시작
            Init(bulletNum);
            isfire = true;
            bullet_coroutine = BulletSpawn();
            StartCoroutine(bullet_coroutine);
        }
    }

    public void EndGame()
    {   // 게임 종료
        bullet_level = 0;
        isfire = false;
        StopCoroutine(bullet_coroutine);
        while(bulletObject.Count != 0)
        {   // 메모리 관리를 위해 모든 오브젝트들을 제거해준다.
            Destroy(bulletObject.Dequeue());
        }
    }

    private void Init(int num)
    {
        //Debug.Log("불렛 파괴 전 " + bulletPool.Count);
        bulletPool.Clear(); // 불렛 오브젝트 풀링 초기화.
        //Debug.Log("불렛 파괴 후 " + bulletPool.Count);
        for(int i = 0; i < num; i++)    // 새로운 불렛을 만들어서 큐에 추가.
        {
            bulletPool.Enqueue(CreateNewBullet());
        }
        //Debug.Log("불렛 장전 " + bulletPool.Count);
    }

    private BulletGroup CreateNewBullet()
    {   // 새로운 장애물을 인스턴스화 시킨 후 비활성화한다. 생성된 오브젝트를 관리하기 위해 오브젝트 큐에 저장.
        var newBullet = Instantiate(bulletPrefab).GetComponent<BulletGroup>();
        newBullet.gameObject.SetActive(false);
        newBullet.transform.SetParent(BulletStartPosition, false);
        bulletObject.Enqueue(newBullet.gameObject);
        return newBullet;
    }

    public BulletGroup GetBullet()
    {   // 장애물을 꺼낼때 사용.
        var bullet = obstacleManager.bulletPool.Dequeue();
        if(bullet_level >= 5) bullet_level = 4;
        bullet.SetBulletSpeedLevel(bullet_speeds[bullet_level]);

        return bullet;
    }

    public void ReturnObject(BulletGroup bullet)
    {   // 사용된 장애물들은 비활성 후 다시 오브젝트 풀에다가 넣어서 재활용해준다.
        if(bullet.gameObject.name == "Tuto_bullet")
        {
            bullet.gameObject.SetActive(false); return;
        }
        bullet.gameObject.SetActive(false);

        obstacleManager.bulletPool.Enqueue(bullet);
    }

    IEnumerator BulletSpawn()
    {   // 큐에 저장되어있는 장애물들을 활성화 시켜 발사
        float waitTime = 1.5f;
        float patternRandomNum;

        yield return new WaitForSeconds(3);
        while(isfire) {
            patternRandomNum = Random.value;

            if(patternRandomNum < 0.5) waitTime = 1.2f;
            else waitTime = 1.5f;

            var group = GetBullet();
            group.transform.SetParent(BulletStartPosition, false);
            group.SettingBullet(patternRandomNum);
            group.gameObject.SetActive(true);

            yield return new WaitForSeconds(waitTime);
        }
    }
    public GameObject TutoBullet()
    {
        return Instantiate(bulletPrefab, BulletStartPosition);
    }   
}