using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class DataTest : MonoBehaviour
{
    [SerializeField] TSVData tsvData;  // 구글 시트들

    public Dictionary<string, List<string>> sinaData = new Dictionary<string, List<string>>();  // 1 시나리오 데이터 저장
    public Dictionary<string, List<string>> localData = new Dictionary<string, List<string>>(); // 2 지역/국가별 언어 데이터 저장
    public Dictionary<string, List<string>> backGroundData = new Dictionary<string, List<string>>();    // 3 배경 데이터 저장
    public Dictionary<string, List<string>> stuffData = new Dictionary<string, List<string>>(); // 4 재료 데이터 저장
    public Dictionary<string, List<string>> stuffGradeData = new Dictionary<string, List<string>>();    // 5 재료 등급 데이터 저장
    public Dictionary<string, List<string>> storeData = new Dictionary<string, List<string>>(); // 6 상점 데이터 저장
    public Dictionary<string, List<string>> cookData = new Dictionary<string, List<string>>();  // 7 요리 데이터 저장
    public Dictionary<string, List<string>> questData = new Dictionary<string, List<string>>(); // 8 퀘스트 데이터 저장
    public Dictionary<string, List<string>> questConditionData = new Dictionary<string, List<string>>(); // 9 퀘스트 조건 데이터 저장
    public Dictionary<string, List<string>> rewardData = new Dictionary<string, List<string>>();    // 10 보상 데이터 저장
    public Dictionary<string, List<string>> tipData = new Dictionary<string, List<string>>();   // 11 팁 데이터 저장
    public Dictionary<string, List<string>> skinData = new Dictionary<string, List<string>>();  // 12 캐릭터 스킨 데이터 저장
    public Dictionary<string, List<string>> resourceData = new Dictionary<string, List<string>>();  // 13 ui 및 각종 데이터 정보 저장

    public bool isDataReady = false;    // 모든 구글 데이터들을 다 불러왔는지 확인하는 변수
    private bool isQuest = false;   // 퀘스트 데이터를 불러올 차례인지 확인하는 변수.
    public List<string> InitQuestData = new List<string>(); // 초기 퀘스트 데이터들 저장하는 변수

    void Awake()
    {
        sinaData = ReadTSV(tsvData.sinaDataText);
        localData = ReadTSV(tsvData.localDataText);
        backGroundData = ReadTSV(tsvData.backGroundDataText);
        stuffData = ReadTSV(tsvData.stuffDataText);
        stuffGradeData = ReadTSV(tsvData.stuffGradeDataText);
        storeData = ReadTSV(tsvData.storeDataText);
        cookData = ReadTSV(tsvData.cookDataText);
        isQuest = true;
        questData = ReadTSV(tsvData.questDataText);
        isQuest = false;
        questConditionData = ReadTSV(tsvData.questConditionDataText);
        rewardData = ReadTSV(tsvData.rewardDataText);
        tipData = ReadTSV(tsvData.tipDataText);
        skinData = ReadTSV(tsvData.skinDataText);
        resourceData = ReadTSV(tsvData.resourceDataText);

        isDataReady = true;
        //Invoke("DataSetReady", 2);  // 인게임 테스트용
    }

    public void DataSetReady()
    {
        ItemStatusManager.itemStatusManager.ItemDataLoad(); // 재료, 캐릭터, 맵, 요리에 대한 정보들을 오브젝트에 저장해주기 위한 함수를 실행
        QuestManager.questManager.Set_Quest_Info();   // 퀘스트 정보들을 전처리하기 위한 함수를 실행

        //InGameDataSetReady();
    }

    // 인게임시 이전 플레이 데이터들을 재료 상점 데이터 및 캐릭터 상점, 도감 상점, 퀘스트 정보들, 플레이어 스킨들에 적용하는 함수.
    public void InGameDataSetReady()
    {
        Debug.Log("이건 언제 호출될까 초기 데이터" + (GameManager.gm.isGuest ? "게스트 입니다." : "게스트가 아닙니다."));
        InGameManager.inGameManager.ingredientShopManager.GetStuffData();   // 재료 상점 데이터 불러오기. ( 팔고있던 재료들에 대한 정보 불러오기 )
        InGameManager.inGameManager.characterShopManager.Init();    // 캐릭터 상점 세팅 ( 구매 및 장착할 수 있는 캐릭터 스킨들 불러옴 )
        InGameManager.inGameManager.player.SetSkinNum();    // 플레이어 스킨 적용
        InGameManager.inGameManager.achiveInfoManager.QuestDataInit(); // 퀘스트 데이터 불러오기 ( 현재 진행한 퀘스트들에 대한 정보들 로드 )
        InGameManager.inGameManager.bookManager.Init(); // 도감에 대한 정보들 불러오기.

        if (GameManager.gm.tutorial) InGameManager.inGameManager.MainMenuSet();  // 튜토리얼이 클리어 되어있으면 바로 메인화면 로딩.
        else    // 아니라면 튜토리얼 실행.
        {
            SoundManager.soundManager.SetBGM("Tutorial");   // 튜토리얼 브금.
            //InGameManager.inGameManager.TutorialSet();
            GameManager.gm.current_screen = "Tutorial"; // 현재 화면 튜토리얼
            GameManager.gm.GetAttendanceReward = false; // 아직 출석 보상 받지 않음.
            InGameManager.inGameManager.uiManager.TutorialSetUI();  // 튜토리얼 UI 설정
        }
    }

    Dictionary<string, List<string>> ReadTSV(TextAsset textData)    // TSV 형식의 파일 데이터를 읽어와서 줄바꿈이나 엔터를 기준으로 단어들을 쪼개 배열로 만들어줌.
    {
        string fullText = textData.text;    // TSV 데이터 읽어옴

        char[] seperators = new char[] { '\r', '\n' };
        var b = fullText.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);   // seperators에 있는 데이터들이 fullText에 있으면 모두 split 해준다.

        return SplitData(b);    // 쪼개진 데이터 배열을 dict 형식으로 반환해준다.
    }

    Dictionary<string, List<string>> SplitData(string[] strOrigin)  // strOrigin 데이터들을 dict 형식으로 바꿔주는 함수.
    {
        string strKeyValue = "";    // 가리키고 있는 데이터 키 값을 담는 변수
        Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
        //var keyName = strOrigin[1].Split('\t');

        for (int i = 2; i < strOrigin.Length; i++)
        {
            var a = strOrigin[i].Split('\t');   // '\t'를 기준으로 쪼개준다.
            strKeyValue = a[0]; // 맨 첫 번째 데이터는 해당 데이터의 키 값.
            if (data.ContainsKey(a[0])) Debug.Log(a[0] + "키가 중복됩니다."); // 키가 있으면 생략
            else data.Add(a[0], new List<string>());    // 해당 키가 dict에 없다면 새로 키값을 위한 List를 생성해준다.
            if (isQuest) // 퀘스트 데이터들을 추출 중이라면?
            {
                var keyNumber = a[0].Split('_');    // 퀘스트 데이터의 문자열 부분과 숫자 부분이 '_'로 구분되어 있기 때문에 '_'로 쪼개준다.
                if (int.Parse(keyNumber[2]) == 1)    // 숫자부분이 1이면 해당 퀘스트 집합의 첫 퀘스트이다.
                {
                    //Debug.Log("초기 퀘스트 데이터 " + a[0]);
                    InitQuestData.Add(a[0]);   // 첫 퀘스트를 초기 퀘스트를 저장해놓는 리스트에 넣어준다.

                    //Debug.Log(InitQuestData.Count + " 안에 있는 데이터 개수 " + InitQuestData[InitQuestData.Count-1].Length + " 키 값 : " + InitQuestData[InitQuestData.Count-1][0]);
                }
            }
            for (int j = 0; j < a.Length; j++)
            {
                data[a[0]].Add(a[j].Replace("\\n", "<br>"));   // key값이 해당하는 라인에 존재하는 데이터들을 key:list 에 넣어준다.
            }
        }

        return data;
    }
}