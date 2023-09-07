using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI.Extensions;

public enum QuestType
{
    JumpQuest = 1,  // 점프 횟수 누적 퀘스트
    AvoidQuest,     // 장애물 회피 퀘스트
    GoldQuest,      // 누적 골드 퀘스트
    BuyQuest,       // 재료 구매 횟수 퀘스트
    CostumeQuest,   // 캐릭터 구매 개수 퀘스트
    CookQuest,      // 요리 완성 개수 퀘스트
    RelayQuest,     // 이어 달리기 퀘스트
    EventQuest = 999    // 이벤트 퀘스트
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager questManager = null;

    //public bool dataSetReady = false;

    public Dictionary<string, Quest> questDataBase = new Dictionary<string, Quest>();   // 퀘스트 데이터베이스

    public List<Quest> clearQuests = new List<Quest>();     // 퀘스트 목록 -> 날짜 순으로 정렬하기 위해서 사용
    public Dictionary<string, Quest> clearDicts = new Dictionary<string, Quest>();  // 해당 퀘스트가 클리어 되었는지 확인하기 위해 사용
    // list로 N번 방문대신 딕셔너리로 사용 시간 복잡도 O(1). 퀘스트 슬롯 생성할때 quest는 클리어 날짜가 없음. 반면 클리어 퀘스트는 클리어 날짜가 존재해서 List에 저장시 서로 quest는 일치하지
    // 않다고 떠서 대신 딕셔너리로 사용하게 되었음.

    public bool isQuestDataSetReady = true;

    void Awake()
    {
        if (questManager != null && questManager != this)
            Destroy(gameObject);
        else
            questManager = this;

        DontDestroyOnLoad(gameObject);

        questDataBase.Clear();
    }

    public void Set_Quest_Info()
    {
        string startKey;    // 퀘스트 키 값을 탐색해줄 변수 값 선언
        char[] seperators = new char[] { '{', '}' };
        for (int i = 0; i < GameManager.gm.dataLoader.InitQuestData.Count; i++)  // 퀘스트 시작 데이터 키 값을 가지고 있는데이터의 개수만큼 호출
        {
            startKey = GameManager.gm.dataLoader.InitQuestData[i];   // 시작 퀘스트들의 키 값을 넣어준다.
            bool flag = true;

            while (GameManager.gm.dataLoader.questData.ContainsKey(startKey))
            {   // 퀘스트 데이터에 해당 키값이 포함되어 있을 경우. 퀘스트를 만들어서 퀘스트 데이터베이스 및 퀘스트 종류 별 데이터를 넣어준다.
                //Debug.Log(startKey + "의 키를 가진 업적 데이터로드");
                var keyNumber = GameManager.gm.dataLoader.questData[startKey][0].Split('_');
                string questDesText = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.questData[startKey][3]][(int)GameManager.gm.countryLang];
                string questNameText = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.questData[startKey][2]][(int)GameManager.gm.countryLang];
                var questDes = questDesText.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);
                var questName = questNameText.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);
                questDesText = questDes[0] + GameManager.gm.dataLoader.questData[startKey][5] + questDes[2];
                questNameText = questName[0] + int.Parse(keyNumber[2]);

                Quest quest = new Quest(GameManager.gm.dataLoader.questData[startKey][0],
                                                    questNameText,
                                                    questDesText,
                                                    GameManager.gm.dataLoader.questData[startKey][4],
                                                    int.Parse(GameManager.gm.dataLoader.questData[startKey][5].Replace(",", "")), GameManager.gm.dataLoader.questData[startKey][7],
                                                    int.Parse(GameManager.gm.dataLoader.questData[startKey][8].Replace(",", "")), GameManager.gm.dataLoader.questData[startKey][11]);
                quest.TypeSet();

                questDataBase[startKey] = quest;

                startKey = GameManager.gm.dataLoader.questData[startKey][11];
            }
        }
    }

    public void Load_QuestStatus(QuestInfo questInfo)
    {   // 클리어 퀘스트 데이터들을 세이브 파일에서 불러온다.
        clearQuests = questInfo.clear_quest_List;
        Debug.Log("퀘스트 정보들 개수 " + clearQuests.Count);
        for (int i = 0; i < clearQuests.Count; i++)
        {
            clearDicts[clearQuests[i].key] = clearQuests[i];
        }
    }

    public QuestInfo Get_QuestStatus()
    {   // 클리어한 퀘스트 데이터들을 세이브 파일에 저장할때 사용한다.
        QuestInfo questInfo = new QuestInfo(clearQuests);
        Debug.Log("클리어한 퀘스트 정보들 " + questInfo.clear_quest_List.Count);
        return questInfo;
    }
}