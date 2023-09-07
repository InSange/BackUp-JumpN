using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 캐릭터 스킨 이미지 리소스들을 가지고 있는 스크립터블 오브젝트
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterSkinData", order = 1)]
public class CharacterSkin : ScriptableObject
{   // Skins를 리스트로 모든 스킨 데이터들을 저장해놓는 변수.
    public List<Skins> catSkins = new List<Skins>();
}

[System.Serializable]
public class Skins{
    public List<Sprite> CatSprites; // 고양이 스킨 전용 리스트
    public List<Sprite> BoardSprites;   // 보드 스킨 전용 리스트

    public Skins()
    {
        CatSprites = new List<Sprite>();
        BoardSprites = new List<Sprite>();
    }
}

[System.Serializable]
public class CatSkins{
    public List<Sprite> cat_run;    // 일반 달리기
    public List<Sprite> cat_jump1;  // 1단 점프
    public List<Sprite> cat_jump2;  // 2단 점프
    public List<Sprite> cat_landing;    // 고양이 착륙
    public List<Sprite> board_run;  // 보드 달리기
    public List<Sprite> board_jump1;    // 보드 1단 점프
    public List<Sprite> board_jump2;    // 보드 2단 점프
    public List<Sprite> board_landing;  // 보드 착륙

    public CatSkins()
    {
        cat_run = new List<Sprite>();
        cat_jump1 = new List<Sprite>();
        cat_jump2 = new List<Sprite>();
        cat_landing = new List<Sprite>();
        board_run = new List<Sprite>();
        board_jump1 = new List<Sprite>();
        board_jump2 = new List<Sprite>();
        board_landing = new List<Sprite>();
    }
}