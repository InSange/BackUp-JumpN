using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Quest
{  
    [Header("����Ʈ ������ ������")]
    public string key = ""; // �����ϴ� Ű : �������� ���̿��� ��. 
    public string quest_Name = ""; // ����Ʈ �̸�
    public string quest_Des = ""; // ����Ʈ ����
    public string cond = ""; // ���ǿ� �ʿ��� ������ (��� ����Ʈ�� ���� 5���ʿ���� -> ���� ������ ������ �Ŵ��� ��ųʸ��� ����)
    public int cond_val = 0;    // ����Ʈ�� �Ϸ��ϱ� ���� �ʿ��� ��ġ ������ cond ����
    public string re_name = "";  // ���� Ű���� (��� �����̸� gold)
    public int re_val = 0;  // ���� ����
    public string nextquestkey = ""; // ���� ����Ʈ (������ ����Ʈ)�� �����ϱ� ���� Ű
    public DateTime clearDate; // ����Ʈ Ŭ���� �Ͻ�
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

    public void TypeSet()   // ����Ʈ Ÿ�� ����
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
