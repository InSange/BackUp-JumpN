using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Map : Item
{   // 맵 이름과 이미지들을 관리
    [SerializeField]
    public string stageName;
    [SerializeField]
    public Sprite back_sprite;
    [SerializeField]
    public int price;

}
