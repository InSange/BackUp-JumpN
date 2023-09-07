using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Quest
{  
    [Header("퀘스트 데이터 변수들")]
    public string key = ""; // 구분하는 키 : 독립적인 값이여야 함. 
    public string quest_Name = ""; // 퀘스트 이름
    public string quest_Des = ""; // 퀘스트 설명
    public string cond = ""; // 조건에 필요한 데이터 (재료 퀘스트는 감자 5개필요로함 -> 감자 정보는 아이템 매니저 딕셔너리에 존재)
    public int cond_val = 0;    // 퀘스트를 완료하기 위해 필요한 수치 데이터 cond 개수
    public string re_name = "";  // 보상 키워드 (골드 보상이면 gold)
    public int re_val = 0;  // 보상 수량
    public string nextquestkey = ""; // 다음 퀘스트 (연계형 퀘스트)에 접근하기 위한 키
    public DateTime clearDate; // 퀘스트 클리어 일시
    public bool isClear;
    public QuestType type;

    public Quest(string _key, string name, string desc, string condition, int condition_val, string reward_name, int reward_val, string next_Quest_Key)
    {
        key = _key;
        quest_Name = name;
        quest_Des = desc;
        cond = condition;
        cond_val = condition_val;
        re_name = reward_name;
        re_val = reward_val;
        nextquestkey = next_Quest_Key;
        isClear = false;
    }

    public Quest()
    {
        key = "";
        quest_Name = "";
        cond = "";
        cond_val = 0;
        re_name = "";
        re_val = 0;
        nextquestkey = "";
        isClear = false;
    }

    public void TypeSet()   // 퀘스트 타입 설정
    {
        switch(cond)
        {
            case "qstcndtid_01":
                type = QuestType.JumpQuest;
                break;
            case "qstcndtid_02":
                type = QuestType.AvoidQuest;
                break;
            case "qstcndtid_03":
                type = QuestType.GoldQuest;
                break;
            case "qstcndtid_04":
                type = QuestType.BuyQuest;
                break;
            case "qstcndtid_05":
                type = QuestType.CostumeQuest;
                break;
            case "qstcndtid_06":
                type = QuestType.CookQuest;
                break;
            case "qstcndtid_07":
                type = QuestType.EventQuest;
                break;
            default:
                break;
        }
    }
}
