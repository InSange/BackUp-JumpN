using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookSlot : MonoBehaviour
{   // �丮�� �ʿ��� ������ ��Ÿ���� ����
    public Image ingredientImage;   // �ʿ��� ��� �̹���
    public TMP_Text ingredientInfoText; // �ʿ��� ��� �ؽ�Ʈ

    public void SetImage(Sprite sprite) // ��� �̹��� ����
    {
        ingredientImage.sprite = sprite;
    }

}
