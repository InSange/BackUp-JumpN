using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class QuestSlot : MonoBehaviour
{
    [Header("업적 인포 매니저")]
    public AchiveInfoManager manager;   // 퀘스트 슬롯을 직접적으로 관리해주는 매니저 연결

    [Header("업적 정보")]
    public Quest quest; // 퀘스트 정보

    [Header("업적 콘텐츠 슬롯")]
    public TMP_Text tile_Text;  // 퀘스트 이름 텍스트
    public TMP_Text content_Text;   // 퀘스트 내용 텍스트
    public Image cur_progressBar;   // 현재 진행바
    public TMP_Text progress_Text;  // 진행표시 텍스트 ex) 1/50
    public TMP_Text reward_Text;    // 보상 텍스트
    public GameObject progressPanel;    // 진행바 관련 오브젝트 집합 관리
    public TMP_Text[] gold_Text;    // 보상 골드 텍스트

    [Header("업적 상태")]
    public Button[] buttons;    // 0 : 보상, 1 : 받기, 2 : 완료

    [Header("보상 받을 수 있는지")]
    public bool canReceiveReward = false;   // 받을 수 있는지 확인하는 불 변수


    public void SetInfo(Quest info) // 퀘스트 정보 세팅
    {
        quest = info;   // 퀘스트 정보 입력
        SetSlot();  // 입력 받은 정보 퀘스트 슬롯에 적용
    }

    public void SetSlot()
    {
        tile_Text.text = quest.quest_Name;  // 퀘스트 이름 적용
        reward_Text.text = quest.re_name + "\n" + quest.re_val; // 퀘스트 보상 적용
        content_Text.text = quest.quest_Des;    // 퀘스트 설명 적용

        gold_Text[0].text = quest.re_val.ToString();    // 퀘스트 버튼 골드 정보 적용
        gold_Text[1].text = quest.re_val.ToString();

        switch(quest.type)
        {
            case QuestType.JumpQuest:   // 점프 누적 횟수 퀘스트
                progress_Text.text = GameManager.gm.jumpCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.jumpCnt/(float)quest.cond_val;
                break;
            case QuestType.AvoidQuest:  // 회피 누적 횟수 퀘스트
                progress_Text.text = GameManager.gm.avoidCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.avoidCnt/(float)quest.cond_val;
                break;
            case QuestType.GoldQuest:   // 골드 누적 퀘스트
                progress_Text.text = GameManager.gm.stackedGold + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.stackedGold/(float)quest.cond_val;
                break;
            case QuestType.BuyQuest:    // 쟤료 구입 누적 퀘스트
                progress_Text.text = GameManager.gm.buyCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.buyCnt/(float)quest.cond_val;
                break;
            case QuestType.CostumeQuest:    // 코스튬 수집 개수 퀘스트
                int costumeCnt = 0;
                for(int i = 0; i < ItemStatusManager.itemStatusManager.characters.Length; i++)
                {
                    if(ItemStatusManager.itemStatusManager.characters[i].isPurchased) costumeCnt++;
                }

                progress_Text.text = costumeCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = costumeCnt/(float)quest.cond_val;
                break;
            case QuestType.CookQuest:   // 요리 개수 퀘스트
                int cookCnt = 0;
                for(int i = 0; i < ItemStatusManager.itemStatusManager.cooks.Length; i++)
                {
                    if(ItemStatusManager.itemStatusManager.cooks[i].isPurchased) cookCnt++;
                }

                progress_Text.text = cookCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = cookCnt/(float)quest.cond_val;
                break;
            case QuestType.EventQuest:  // 이벤트용 퀘스트
                break;
            default:
                break;
        }
    }

    public void SetUI(bool flag)    // 퀘스트 클리어 유무에 따라 설정하는 UI 세팅이 다름
    {
        if(flag)    // 클리어 되지 않았다면 진행바와 현재 상태를 표시한다.
        {
            progressPanel.SetActive(true);
            cur_progressBar.gameObject.SetActive(true);
            reward_Text.gameObject.SetActive(true);

            if(cur_progressBar.fillAmount >= 1)
            {
                buttons[0].gameObject.SetActive(false);
                buttons[1].gameObject.SetActive(true);
                buttons[2].gameObject.SetActive(false);
                canReceiveReward = true;
            }
            else
            {
                buttons[0].gameObject.SetActive(true);
                buttons[1].gameObject.SetActive(false);
                buttons[2].gameObject.SetActive(false);
                canReceiveReward = false;
            }
        }
        else    // 클리어 되었더라면 진행바를 없애고 완료 버튼만 표시해준다.
        {
            progressPanel.SetActive(false);
            cur_progressBar.gameObject.SetActive(false);
            reward_Text.gameObject.SetActive(false);

            buttons[0].gameObject.SetActive(false);
            buttons[1].gameObject.SetActive(false);
            buttons[2].gameObject.SetActive(true);
        }
    }

    public void GetReward() // 보상 획득 함수.
    {
        if(quest.re_name == "rewardid_1")   // 보상 이름에따라 획득하는 것이 다름. 해당 보상은 골드
        {
            buttons[0].gameObject.SetActive(false);
            buttons[1].gameObject.SetActive(false);
            buttons[2].gameObject.SetActive(true);
            GameManager.gm.gold += quest.re_val;    // 골드 추가
            GameManager.gm.stackedGold += quest.re_val;
            quest.clearDate = NTP.ntp.getServerTime();  // 퀘스트 클리어 시각
            quest.isClear = true;   // 퀘스트 클리어

            QuestManager.questManager.clearQuests.Add(quest);   // 클리어한 퀘스트 목록에 추가

            if(quest.nextquestkey == "-1")  // 만약 다음 퀘스트가 존재하지 않을 시
            {   // 해당 퀘스트 키값을 가진 퀘스트 슬롯란을 없애준다.
                manager.questSlotsInfo.Remove(manager.questSlotsInfo.Find(x => x.quest.key == this.quest.key));
                manager.AddClearSlot(this);
                Destroy(this.gameObject);   // 그리고 현재 슬롯을 없애준다.
            }
            else
            {
                SetInfo(QuestManager.questManager.questDataBase[quest.nextquestkey]);   // 그다음 퀘스트 정보로 갱신해준다.
                manager.SlotUpdate();
            }
            UserDataManager.userDataManager.SaveData(); // 현재 변경된 퀘스트 데이터들을 저장해준다.
        }
    }
}
