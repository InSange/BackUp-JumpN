using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSkin : MonoBehaviour
{   
    public CharacterSkin skinSet; // ��Ų �����͸� ���ϰ��ִ� ��ũ���ͺ� ������Ʈ
    public int skinNr; // ���� ��Ų ��ȣ
    public List<CatSkins> skins;   
    [SerializeField]
    private Sprite[] sprites;
    public GameObject cat;  // ����� ������Ʈ
    public GameObject skate_board;  // ������Ʈ ���� ������Ʈ

    Image catImage; // ����� �̹���
    Image skate_Image;  // ������Ʈ �̹���
    
    void Start()
    {
        catImage = cat.GetComponent<Image>();
        skate_Image = skate_board.GetComponent<Image>();
    }

    // Update is called once per frame
    void LateUpdate()   
    {   // �������� ���������� �� �ִϸ��̼ǿ� �ش�Ǵ� �̹������� �ٲ���.
        SkinChoice();
    }

    void SkinChoice()
    {
        //Debug.Log("��Ų �̸� : " + catImage.sprite.name + ", " + skate_Image.sprite.name);
        string catSpriteName = catImage.sprite.name;    // ��Ų �̸�
        string boardSpriteName = skate_Image.sprite.name;   // ���� �̸�
        int catSpriteNum = int.Parse(catSpriteName.Substring(catSpriteName.Length - 2)) - 1;    // ��Ų �̸��� �ش��ϴ� �ѹ� ��
        int boardSpriteNum = int.Parse(boardSpriteName.Substring(boardSpriteName.Length - 2)) - 1;  // ���� �̸��� �ش��ϴ� �ѹ� ��

        catImage.sprite = skinSet.catSkins[skinNr].CatSprites[catSpriteNum];    // ����� ��Ų �̸��� �ش��ϴ� ��ȣ�� ��Ų ����
        skate_Image.sprite = skinSet.catSkins[skinNr].BoardSprites[boardSpriteNum]; // ������Ʈ ��Ų �̸��� �ش��ϴ� ��ȣ�� ��Ų ����
    }
}