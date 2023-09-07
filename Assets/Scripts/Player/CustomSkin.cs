using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSkin : MonoBehaviour
{   
    public CharacterSkin skinSet; // 스킨 데이터를 지니고있는 스크립터블 오브젝트
    public int skinNr; // 현재 스킨 번호
    public List<CatSkins> skins;   
    [SerializeField]
    private Sprite[] sprites;
    public GameObject cat;  // 고양이 오브젝트
    public GameObject skate_board;  // 스케이트 보드 오브젝트

    Image catImage; // 고양이 이미지
    Image skate_Image;  // 스케이트 이미지
    
    void Start()
    {
        catImage = cat.GetComponent<Image>();
        skate_Image = skate_board.GetComponent<Image>();
    }

    // Update is called once per frame
    void LateUpdate()   
    {   // 프레임이 끝날때마다 각 애니메이션에 해당되는 이미지들을 바꿔줌.
        SkinChoice();
    }

    void SkinChoice()
    {
        //Debug.Log("스킨 이름 : " + catImage.sprite.name + ", " + skate_Image.sprite.name);
        string catSpriteName = catImage.sprite.name;    // 스킨 이름
        string boardSpriteName = skate_Image.sprite.name;   // 보드 이름
        int catSpriteNum = int.Parse(catSpriteName.Substring(catSpriteName.Length - 2)) - 1;    // 스킨 이름에 해당하는 넘버 값
        int boardSpriteNum = int.Parse(boardSpriteName.Substring(boardSpriteName.Length - 2)) - 1;  // 보드 이름에 해당하는 넘버 값

        catImage.sprite = skinSet.catSkins[skinNr].CatSprites[catSpriteNum];    // 고양이 스킨 이름에 해당하는 번호의 스킨 적용
        skate_Image.sprite = skinSet.catSkins[skinNr].BoardSprites[boardSpriteNum]; // 스케이트 스킨 이름에 해당하는 번호의 스킨 적용
    }
}