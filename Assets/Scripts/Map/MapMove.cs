using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMove : MonoBehaviour
{   // �� �̵� ��ũ��Ʈ -> �ΰ��ӿ��� ���
    [SerializeField]
    private RectTransform current_pos;  // RectTransform ���� ����
    [SerializeField]
    private float speed;    // ���� �� �̵��ӵ�
    [SerializeField]
    private float maxSpeed; // �ִ� �� �ӵ� -> �̵��ӵ��� �ִ� �� �ӵ����� �����Ѵ�. ������ ������ ���� �ִ� �� �ӵ��� ����
    [SerializeField]
    private float timeUp = 0;   // �ӵ� ������ ���� �ð� üũ

    [SerializeField]
    List<RectTransform> backgroundObj = new List<RectTransform>();  // ��������Ʈ���� ����
    [SerializeField]
    List<Image> backgroundImage = new List<Image>();    // ����̹������� ����
    [SerializeField]
    private Vector3 startPos = new Vector3(0, 0);   // �� ó�� ���� ��ġ
    [SerializeField]
    private Vector3[] posXarray = { new Vector3(-2160, 0),
                            new Vector3(-1080, 0),
                            new Vector3(0, 0),
                            new Vector3(1080, 0),
                            new Vector3(2160, 0)};

    void Awake()
    {
        current_pos = GetComponent<RectTransform>();
    }

    public void ResetPos(float speed, float maxSpeed) {
        current_pos.anchoredPosition = startPos;    // �� ��ġ�� ó������ ��ġ�� ����
        this.speed = speed; // ó�� ���ǵ�� ����
        this.maxSpeed = maxSpeed;   // �ִ� �ӵ��� ����

        for(int i = 0; i < backgroundObj.Count; i++)
        {
            posXarray[i].y = backgroundObj[i].anchoredPosition.y;
            backgroundObj[i].anchoredPosition = posXarray[i];
        }
    }

    private void FixedUpdate() {
        if(maxSpeed > speed) 
        {   // �ִ�ӵ��� ���� �ӵ����� Ŭ �ÿ� �ӵ��� ���� �����ش�.
            timeUp += Time.deltaTime;
            if(timeUp >= 1.0f)
            {
                speed += 10;
                timeUp = 0;
            }
        }
        if(GameManager.gm.isLive)
        {   // ������� �� ����ؼ� ���� ����
            InGameManager.inGameManager.uiManager.ChangeCurrentScore();
            Map_Move(speed);
        }
    }

    public void Map_Move(float _speed)
    {   // speed �� ��ŭ ����ؼ� �̵����ش�.
        current_pos.anchoredPosition = new Vector3(current_pos.anchoredPosition.x - (_speed * Time.smoothDeltaTime), current_pos.anchoredPosition.y);
        // ���� -1080���� �Ѿ�� 2160��ŭ �����༭ ���������� ���� �̵��Ǵ� �� ó�� ���̰� �ٲ�.
        if(current_pos.anchoredPosition.x <= -1080)
        {
            current_pos.anchoredPosition = new Vector3(current_pos.anchoredPosition.x + 2160, current_pos.anchoredPosition.y);
        }
    }

    public void ChangeBackGroundImage(Sprite map_sprite)
    {   // �� ���� -> �ʻ������� ������ �� ����� �ٲ�� �ϴ� �Լ�.
        for(int i = 0; i < backgroundImage.Count; i++)
        {
            backgroundImage[i].sprite = map_sprite;
        }
    }

    public void AddSpeed(float speed)
    {   // �ִ� �ӵ� ���� �Լ�
        this.maxSpeed += speed;
    }
}
