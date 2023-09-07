using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class QuestSlot : MonoBehaviour
{
    [Header("���� ���� �Ŵ���")]
    public AchiveInfoManager manager;   // ����Ʈ ������ ���������� �������ִ� �Ŵ��� ����

    [Header("���� ����")]
    public Quest quest; // ����Ʈ ����

    [Header("���� ������ ����")]
    public TMP_Text tile_Text;  // ����Ʈ �̸� �ؽ�Ʈ
    public TMP_Text content_Text;   // ����Ʈ ���� �ؽ�Ʈ
    public Image cur_progressBar;   // ���� �����
    public TMP_Text progress_Text;  // ����ǥ�� �ؽ�Ʈ ex) 1/50
    public TMP_Text reward_Text;    // ���� �ؽ�Ʈ
    public GameObject progressPanel;    // ����� ���� ������Ʈ ���� ����
    public TMP_Text[] gold_Text;    // ���� ��� �ؽ�Ʈ

    [Header("���� ����")]
    public Button[] buttons;    // 0 : ����, 1 : �ޱ�, 2 : �Ϸ�

    [Header("���� ���� �� �ִ���")]
    public bool canReceiveReward = false;   // ���� �� �ִ��� Ȯ���ϴ� �� ����


    public void SetInfo(Quest info) // ����Ʈ ���� ����
    {
        quest = info;   // ����Ʈ ���� �Է�
        SetSlot();  // �Է� ���� ���� ����Ʈ ���Կ� ����
    }

    public void SetSlot()
    {
        tile_Text.text = quest.quest_Name;  // ����Ʈ �̸� ����
        reward_Text.text = quest.re_name + "\n" + quest.re_val; // ����Ʈ ���� ����
        content_Text.text = quest.quest_Des;    // ����Ʈ ���� ����

        gold_Text[0].text = quest.re_val.ToString();    // ����Ʈ ��ư ��� ���� ����
        gold_Text[1].text = quest.re_val.ToString();

        switch(quest.type)
        {
            case QuestType.JumpQuest:   // ���� ���� Ƚ�� ����Ʈ
                progress_Text.text = GameManager.gm.jumpCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.jumpCnt/(float)quest.cond_val;
                break;
            case QuestType.AvoidQuest:  // ȸ�� ���� Ƚ�� ����Ʈ
                progress_Text.text = GameManager.gm.avoidCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.avoidCnt/(float)quest.cond_val;
                break;
            case QuestType.GoldQuest:   // ��� ���� ����Ʈ
                progress_Text.text = GameManager.gm.stackedGold + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.stackedGold/(float)quest.cond_val;
                break;
            case QuestType.BuyQuest:    // ���� ���� ���� ����Ʈ
                progress_Text.text = GameManager.gm.buyCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = GameManager.gm.buyCnt/(float)quest.cond_val;
                break;
            case QuestType.CostumeQuest:    // �ڽ�Ƭ ���� ���� ����Ʈ
                int costumeCnt = 0;
                for(int i = 0; i < ItemStatusManager.itemStatusManager.characters.Length; i++)
                {
                    if(ItemStatusManager.itemStatusManager.characters[i].isPurchased) costumeCnt++;
                }

                progress_Text.text = costumeCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = costumeCnt/(float)quest.cond_val;
                break;
            case QuestType.CookQuest:   // �丮 ���� ����Ʈ
                int cookCnt = 0;
                for(int i = 0; i < ItemStatusManager.itemStatusManager.cooks.Length; i++)
                {
                    if(ItemStatusManager.itemStatusManager.cooks[i].isPurchased) cookCnt++;
                }

                progress_Text.text = cookCnt + "/" + quest.cond_val;
                cur_progressBar.fillAmount = cookCnt/(float)quest.cond_val;
                break;
            case QuestType.EventQuest:  // �̺�Ʈ�� ����Ʈ
                break;
            default:
                break;
        }
    }

    public void SetUI(bool flag)    // ����Ʈ Ŭ���� ������ ���� �����ϴ� UI ������ �ٸ�
    {
        if(flag)    // Ŭ���� ���� �ʾҴٸ� ����ٿ� ���� ���¸� ǥ���Ѵ�.
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
        else    // Ŭ���� �Ǿ������ ����ٸ� ���ְ� �Ϸ� ��ư�� ǥ�����ش�.
        {
            progressPanel.SetActive(false);
            cur_progressBar.gameObject.SetActive(false);
            reward_Text.gameObject.SetActive(false);

            buttons[0].gameObject.SetActive(false);
            buttons[1].gameObject.SetActive(false);
            buttons[2].gameObject.SetActive(true);
        }
    }

    public void GetReward() // ���� ȹ�� �Լ�.
    {
        if(quest.re_name == "rewardid_1")   // ���� �̸������� ȹ���ϴ� ���� �ٸ�. �ش� ������ ���
        {
            buttons[0].gameObject.SetActive(false);
            buttons[1].gameObject.SetActive(false);
            buttons[2].gameObject.SetActive(true);
            GameManager.gm.gold += quest.re_val;    // ��� �߰�
            GameManager.gm.stackedGold += quest.re_val;
            quest.clearDate = NTP.ntp.getServerTime();  // ����Ʈ Ŭ���� �ð�
            quest.isClear = true;   // ����Ʈ Ŭ����

            QuestManager.questManager.clearQuests.Add(quest);   // Ŭ������ ����Ʈ ��Ͽ� �߰�

            if(quest.nextquestkey == "-1")  // ���� ���� ����Ʈ�� �������� ���� ��
            {   // �ش� ����Ʈ Ű���� ���� ����Ʈ ���Զ��� �����ش�.
                manager.questSlotsInfo.Remove(manager.questSlotsInfo.Find(x => x.quest.key == this.quest.key));
                manager.AddClearSlot(this);
                Destroy(this.gameObject);   // �׸��� ���� ������ �����ش�.
            }
            else
            {
                SetInfo(QuestManager.questManager.questDataBase[quest.nextquestkey]);   // �״��� ����Ʈ ������ �������ش�.
                manager.SlotUpdate();
            }
            UserDataManager.userDataManager.SaveData(); // ���� ����� ����Ʈ �����͵��� �������ش�.
        }
    }
}
