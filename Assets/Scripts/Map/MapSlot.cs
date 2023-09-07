using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// �� �������� ���Ǵ� �� ��ư
public class MapSlot : MonoBehaviour
{
    [SerializeField]
    GameObject lockImage;   // ��� ���� �̹��� ������Ʈ
    [SerializeField]
    int mapNum; // �ش� ������ ����Ű�� �� �ѹ�
    [SerializeField]
    TMP_Text mapName;   // ���� �� �̸�
    [SerializeField]
    public TMP_Text lock_text;  // �������~ ǥ��
    [SerializeField]
    GameObject Non_Select_Image;    // �������� ���� �� �̹���

    public void Click()
    {
        if(ItemStatusManager.itemStatusManager.maps[mapNum].isPurchased) // �� ������ Ŭ������ �� �̹� ���� ������ ���¶��?
        {
            InGameManager.inGameManager.mapShopManager.EquipUI(mapNum); // ���� �����Ұ��� ���Ұ��� Ȯ���ϴ� UI����
            return;
        }
        else if(!ItemStatusManager.itemStatusManager.maps[mapNum].isPurchased)  // �������� ���� ���¶��?
        {
            InGameManager.inGameManager.mapShopManager.NeedBuyUI(mapNum);   // ���� ��ư�� ��� ��ư UI ����
            return ;
        }
    }

    public void SetName(string name)    // ���Կ� �ο����� �� ���� �� �̸��� �����Ѵ�.
    {
        mapName.text = name;
    }

    public void SetNum(int num) // ���Կ� �ο����� ��ȣ�� ����
    {   // �ش� ��ȣ�� i��° ���Կ� ���Ͽ� ������ �����ϱ� ���� ��ȣ ����
        mapNum = num;
    }

    public int GetNum()
    {
        return mapNum;
    }

    public void SetLock()
    {
        lockImage.SetActive(true);
    }

    public void UnLock()
    {
        lockImage.SetActive(false);
    }

    public void NoSelect()
    {
        Non_Select_Image.SetActive(true);
    }

    public void Select()
    {
        Non_Select_Image.SetActive(false);
    }
}
