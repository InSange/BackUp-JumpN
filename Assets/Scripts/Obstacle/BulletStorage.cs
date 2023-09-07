using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ��ֹ� �����͸� �����ϴ� ��ũ���ͺ� ������Ʈ
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BulletStorage", order = 2)]
public class BulletStorage : ScriptableObject
{   // ���� �Ѿ��� �������� ��� ���ͼ� ����ҷ��� �߾���.
    [SerializeField] public List<BulletData> bulletDatas = new List<BulletData>();

    public int GetSize()
    {   // ��ֹ� ������ �� ���� ��ȯ �Լ�
        return bulletDatas.Count;
    }
}

[System.Serializable]
public class BulletData
{   // �ҷ� ���� . �̹����� �ݶ��̴� ũ��
    public Sprite bulletImage;
    public float colX;
}