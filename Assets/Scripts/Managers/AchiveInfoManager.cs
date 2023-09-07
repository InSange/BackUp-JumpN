using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveInfoManager : MonoBehaviour
{
    [Header("퀘스트 정보")]
    [SerializeField]
    private GameObject slot_prefab; // 퀘스트 슬롯 프리팹
    [SerializeField]
    private GameObject ScrollViewContent;   // 슬롯이 들어갈 뷰
    [SerializeField]
    public List<QuestSlot> questSlotsInfo;  // 퀘스트 슬롯 목록
    [SerializeField]
    public List<QuestSlot> clearSlotsInfo;  // 클리어한 퀘스트 목록 -> 세이브 파일에서 쓰고 가져오고 하는 역할
    public GameObject TopPanel; // 상단 패널    (퀘스트/리더보드)
    public GameObject BottomPanel;  // 하단 패널    (버튼 세트)

    void Awake()
    {
        // 화면 조정
        float defaultVal = InGameManager.inGameManager.defaultScreenSizeX / InGameManager.inGameManager.defaultScreenSizeY;
        float curVal = InGameManager.inGameManager.screenRect.rect.width / InGameManager.inGameManager.screenRect.rect.height;
        if(curVal < defaultVal)
        {
            RectTransform bottomPanelRect = BottomPanel.GetComponent<RectTransform>();
            bottomPanelRect.sizeDelta = new Vector2(bottomPanelRect.rect.width, bottomPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
            RectTransform topPanelRect = TopPanel.GetComponent<RectTransform>();
            topPanelRect.sizeDelta = new Vector2(topPanelRect.rect.width, topPanelRect.rect.height + (InGameManager.inGameManager.defaultScreenSizeY * (defaultVal - curVal) ));
        }
    }

    void OnEnable()
    {   // 퀘스트 슬롯 데이터들 업데이트
        SlotUpdate();
    }

    public bool CheckCanReceiveReward()
    {   // 퀘스트 데이터들을 최신으로 업데이트 후 완료 가능한 퀘스트가 있을 경우 메인 화면에 알림표시를 나타내준다.
        SlotUpdate();
        for (int i = 0; i < questSlotsInfo.Count; i++)
        {
            if (questSlotsInfo[i].canReceiveReward && questSlotsInfo[i].gameObject.activeSelf == true)
            {
                return true;
            }
        }

        return false;
    }
    // 해당 수정. 퀘스트 진행해야할 것, 퀘스트 클리어한거 분류까지 작업함.
    public void QuestDataInit()
    {
        for (int i = 0; i < GameManager.gm.dataLoader.InitQuestData.Count; i++)
        {   // 퀘스트 데이터를 시트데이터에서 뽑아온다.
            Quest quest = QuestManager.questManager.questDataBase[GameManager.gm.dataLoader.InitQuestData[i]];
            Debug.Log(quest.key + ", " + quest.quest_Name + ", " + quest.quest_Des + ", " + quest.cond + ", " + quest.cond_val);
            while (true)
            {   // 해당 퀘스트의 키를 기준으로 클리어데이터에 해당 키가 있는지 탐색
                if (!QuestManager.questManager.clearDicts.ContainsKey(quest.key))
                {   // 없을 경우 퀘스트 슬롯을 인스턴스화 시켜서 뷰 컨텐츠에 추가해준다.
                    Debug.Log(quest.key + "는 아직 클리어 되어있지 않습니다.");
                    GameObject questSlot = Instantiate(slot_prefab);
                    QuestSlot slot = questSlot.GetComponent<QuestSlot>();
                    slot.manager = this.gameObject.GetComponent<AchiveInfoManager>();
                    slot.SetInfo(quest);
                    questSlotsInfo.Add(slot);
                    break;
                }
                else
                {   // 퀘스트가 이미 클리어 되어 있는 경우 다음 퀘스트 키를 참고하여 슬롯을 만들지 말지 확인한다.
                    Debug.Log(quest.key + "는 클리어 되어 있습니다.");
                    if (quest.nextquestkey != "-1") quest = QuestManager.questManager.questDataBase[quest.nextquestkey];
                    else
                    {
                        GameObject questSlot = Instantiate(slot_prefab);
                        QuestSlot slot = questSlot.GetComponent<QuestSlot>();
                        slot.manager = this.gameObject.GetComponent<AchiveInfoManager>();
                        slot.SetInfo(quest);
                        clearSlotsInfo.Add(slot);
                        break;
                    }
                }
            }
        }
        // 진행중인 퀘스트 목록과 클리어한 퀘스트 목록들을 뷰 컨텐츠에 넣음.
        for (int i = 0; i < questSlotsInfo.Count; i++) questSlotsInfo[i].transform.SetParent(ScrollViewContent.transform, false);
        for (int i = 0; i < clearSlotsInfo.Count; i++) clearSlotsInfo[i].transform.SetParent(ScrollViewContent.transform, false);

        SlotUpdate();
    }

    public void SlotUpdate()
    {   // 모든 퀘스트 데이터 및 수치를 업데이트 시켜준다.
        for (int i = 0; i < questSlotsInfo.Count; i++)
        {
            questSlotsInfo[i].SetSlot();
            questSlotsInfo[i].SetUI(true);
        }

        for (int i = 0; i < clearSlotsInfo.Count; i++)
        {
            clearSlotsInfo[i].SetSlot();
            clearSlotsInfo[i].SetUI(false);
        }
    }

    public void AddClearSlot(QuestSlot slots)
    {   // 퀘스트를 클리어하게 되었을 때 클리어 데이터에 넣어준다.
        // 클리어한 데이터가 마지막 퀘스트였을시에 새로운 클리어용 슬롯을 만들어 추가.
        GameObject questSlot = Instantiate(slot_prefab);
        QuestSlot slot = questSlot.GetComponent<QuestSlot>();
        slot.manager = this;
        slot.SetInfo(slots.quest);
        clearSlotsInfo.Add(slot);
        questSlot.transform.SetParent(ScrollViewContent.transform, false);
        SlotUpdate();
    }

    public void AchiveButton()
    {
        GameManager.gm.current_screen = "Achive";
        SlotUpdate();
    }

    public void LeaderBoardOn()
    {   // 리더보드 User가 로그인되어 있으면 해당 ID로 업적 접근
        if (Social.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // Sign In 성공
                    // 바로 업적 UI 표시 요청
                    Social.ShowAchievementsUI();
                    return;
                }
                else
                {
                    // Sign In 실패 처리
                    return;
                }
            });
        }

        Social.ShowAchievementsUI();
    }
}