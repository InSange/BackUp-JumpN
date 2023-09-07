using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 장애물 데이터를 관리하는 스크립터블 오브젝트
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BulletStorage", order = 2)]
public class BulletStorage : ScriptableObject
{   // 원래 총알을 종류별로 들고 나와서 사용할려고 했었음.
    [SerializeField] public List<BulletData> bulletDatas = new List<BulletData>();

    public int GetSize()
    {   // 장애물 데이터 총 개수 반환 함수
        return bulletDatas.Count;
    }
}

[System.Serializable]
public class BulletData
{   // 불렛 종류 . 이미지와 콜라이더 크기
    public Sprite bulletImage;
    public float colX;
}