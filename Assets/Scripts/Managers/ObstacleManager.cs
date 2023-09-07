using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{   // ��ֹ� �����ϴ� �Ŵ���
    public static ObstacleManager obstacleManager = null;

    [SerializeField]
    public Queue<BulletGroup> bulletPool = new Queue<BulletGroup>();    // ������Ʈ Ǯ��
    public Queue<GameObject> bulletObject = new Queue<GameObject>();    // ������Ʈ Ǯ�� ����

    [Header("Bullet")]
    [SerializeField]
    private GameObject bulletPrefab;    // ��ֹ� ������
    [SerializeField]
    private RectTransform BulletStartPosition;  // ��ֹ� ���� ��ġ
    [SerializeField]
    IEnumerator bullet_coroutine;
    [SerializeField] bool isfire = false;   // ���� ���۽� ��ֹ� ���� ����
    public int bulletNum = 20;  // �߻��� ��ֹ� ����

    public float[] bullet_speeds = new float[5] {0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
    //public float[] bullet_speeds = new float[5] {1f, 2f, 3f, 4f, 5f};

    public int bullet_level = 0;    // ��ֹ� �ܰ�
    void Awake()
    {
        if(obstacleManager != null && obstacleManager != this)
            Destroy(gameObject);
        else
            obstacleManager = this;
    }

    public void StartGame()
    {
        if(GameManager.gm.isLive)   // ������ ���۵ǰ� �÷��̾ ��� ���� ��
        {   // ���� ����
            Init(bulletNum);
            isfire = true;
            bullet_coroutine = BulletSpawn();
            StartCoroutine(bullet_coroutine);
        }
    }

    public void EndGame()
    {   // ���� ����
        bullet_level = 0;
        isfire = false;
        StopCoroutine(bullet_coroutine);
        while(bulletObject.Count != 0)
        {   // �޸� ������ ���� ��� ������Ʈ���� �������ش�.
            Destroy(bulletObject.Dequeue());
        }
    }

    private void Init(int num)
    {
        //Debug.Log("�ҷ� �ı� �� " + bulletPool.Count);
        bulletPool.Clear(); // �ҷ� ������Ʈ Ǯ�� �ʱ�ȭ.
        //Debug.Log("�ҷ� �ı� �� " + bulletPool.Count);
        for(int i = 0; i < num; i++)    // ���ο� �ҷ��� ���� ť�� �߰�.
        {
            bulletPool.Enqueue(CreateNewBullet());
        }
        //Debug.Log("�ҷ� ���� " + bulletPool.Count);
    }

    private BulletGroup CreateNewBullet()
    {   // ���ο� ��ֹ��� �ν��Ͻ�ȭ ��Ų �� ��Ȱ��ȭ�Ѵ�. ������ ������Ʈ�� �����ϱ� ���� ������Ʈ ť�� ����.
        var newBullet = Instantiate(bulletPrefab).GetComponent<BulletGroup>();
        newBullet.gameObject.SetActive(false);
        newBullet.transform.SetParent(BulletStartPosition, false);
        bulletObject.Enqueue(newBullet.gameObject);
        return newBullet;
    }

    public BulletGroup GetBullet()
    {   // ��ֹ��� ������ ���.
        var bullet = obstacleManager.bulletPool.Dequeue();
        if(bullet_level >= 5) bullet_level = 4;
        bullet.SetBulletSpeedLevel(bullet_speeds[bullet_level]);

        return bullet;
    }

    public void ReturnObject(BulletGroup bullet)
    {   // ���� ��ֹ����� ��Ȱ�� �� �ٽ� ������Ʈ Ǯ���ٰ� �־ ��Ȱ�����ش�.
        if(bullet.gameObject.name == "Tuto_bullet")
        {
            bullet.gameObject.SetActive(false); return;
        }
        bullet.gameObject.SetActive(false);

        obstacleManager.bulletPool.Enqueue(bullet);
    }

    IEnumerator BulletSpawn()
    {   // ť�� ����Ǿ��ִ� ��ֹ����� Ȱ��ȭ ���� �߻�
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