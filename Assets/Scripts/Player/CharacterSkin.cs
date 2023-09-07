using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ĳ���� ��Ų �̹��� ���ҽ����� ������ �ִ� ��ũ���ͺ� ������Ʈ
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterSkinData", order = 1)]
public class CharacterSkin : ScriptableObject
{   // Skins�� ����Ʈ�� ��� ��Ų �����͵��� �����س��� ����.
    public List<Skins> catSkins = new List<Skins>();
}

[System.Serializable]
public class Skins{
    public List<Sprite> CatSprites; // ����� ��Ų ���� ����Ʈ
    public List<Sprite> BoardSprites;   // ���� ��Ų ���� ����Ʈ

    public Skins()
    {
        CatSprites = new List<Sprite>();
        BoardSprites = new List<Sprite>();
    }
}

[System.Serializable]
public class CatSkins{
    public List<Sprite> cat_run;    // �Ϲ� �޸���
    public List<Sprite> cat_jump1;  // 1�� ����
    public List<Sprite> cat_jump2;  // 2�� ����
    public List<Sprite> cat_landing;    // ����� ����
    public List<Sprite> board_run;  // ���� �޸���
    public List<Sprite> board_jump1;    // ���� 1�� ����
    public List<Sprite> board_jump2;    // ���� 2�� ����
    public List<Sprite> board_landing;  // ���� ����

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