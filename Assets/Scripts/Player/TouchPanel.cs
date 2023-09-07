using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// �÷��̾� ���� ���� �ǳ�
public class TouchPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField]
    Player player;

    public void OnPointerDown(PointerEventData data)
    {
        player.Jump();
    }

    public void OnPointerUp(PointerEventData data)
    {

    }

    public void OnPointerClick(PointerEventData data)
    {
        //player.Jump();
    }
}
