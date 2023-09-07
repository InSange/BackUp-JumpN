using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMove : MonoBehaviour
{   // 맵 이동 스크립트 -> 인게임에서 사용
    [SerializeField]
    private RectTransform current_pos;  // RectTransform 저장 변수
    [SerializeField]
    private float speed;    // 현재 맵 이동속도
    [SerializeField]
    private float maxSpeed; // 최대 맵 속도 -> 이동속도는 최대 맵 속도까지 증가한다. 레벨이 증가할 수록 최대 맵 속도가 증가
    [SerializeField]
    private float timeUp = 0;   // 속도 증가를 위한 시간 체크

    [SerializeField]
    List<RectTransform> backgroundObj = new List<RectTransform>();  // 배경오브젝트들을 관리
    [SerializeField]
    List<Image> backgroundImage = new List<Image>();    // 배경이미지들을 관리
    [SerializeField]
    private Vector3 startPos = new Vector3(0, 0);   // 맵 처음 시작 위치
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
        current_pos.anchoredPosition = startPos;    // 맵 위치를 처음으로 위치로 세팅
        this.speed = speed; // 처음 스피드로 세팅
        this.maxSpeed = maxSpeed;   // 최대 속도도 세팅

        for(int i = 0; i < backgroundObj.Count; i++)
        {
            posXarray[i].y = backgroundObj[i].anchoredPosition.y;
            backgroundObj[i].anchoredPosition = posXarray[i];
        }
    }

    private void FixedUpdate() {
        if(maxSpeed > speed) 
        {   // 최대속도가 현재 속도보다 클 시에 속도를 점점 높여준다.
            timeUp += Time.deltaTime;
            if(timeUp >= 1.0f)
            {
                speed += 10;
                timeUp = 0;
            }
        }
        if(GameManager.gm.isLive)
        {   // 살아있을 때 계속해서 점수 갱신
            InGameManager.inGameManager.uiManager.ChangeCurrentScore();
            Map_Move(speed);
        }
    }

    public void Map_Move(float _speed)
    {   // speed 값 만큼 계속해서 이동해준다.
        current_pos.anchoredPosition = new Vector3(current_pos.anchoredPosition.x - (_speed * Time.smoothDeltaTime), current_pos.anchoredPosition.y);
        // 맵이 -1080으로 넘어갈때 2160만큼 더해줘서 연속적으로 맵이 이동되는 것 처럼 보이게 바꿈.
        if(current_pos.anchoredPosition.x <= -1080)
        {
            current_pos.anchoredPosition = new Vector3(current_pos.anchoredPosition.x + 2160, current_pos.anchoredPosition.y);
        }
    }

    public void ChangeBackGroundImage(Sprite map_sprite)
    {   // 맵 변경 -> 맵상점에서 장착할 때 배경이 바뀌도록 하는 함수.
        for(int i = 0; i < backgroundImage.Count; i++)
        {
            backgroundImage[i].sprite = map_sprite;
        }
    }

    public void AddSpeed(float speed)
    {   // 최대 속도 증가 함수
        this.maxSpeed += speed;
    }
}
