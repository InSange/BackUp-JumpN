# JumpNyang
진행 필요한 작업
- 퀘스트 클리어한 시점순으로 클리어한 퀘스트 목록 나열
- 리더보드 데이터 추가 및 제거 확인
- 캐릭터 현재 등선운동 -> update로 가속력을 더 해주는 식으로 변경이 필요할 것 같음
- 슬롯데이터들은 언어작업이 되어있음. 그 외 UI 언어 작업 필요 -> DataTest의 LangData로 접근하여 설정해줘야 함

<img width="857" alt="image" src="https://github.com/InSange/JumpNyang/assets/51250442/e07878ee-75db-4f2e-b96c-00e906605ac7">
<br>
인게임에서 바로 테스트를 하시고 싶은 경우 Awake의 마지막 Invoke 주석 해제, DataSetReady()함수의 InGameDataSetReady() 주석 해제

<img width="836" alt="image" src="https://github.com/InSange/JumpNyang/assets/51250442/5b769a9f-6baa-4a13-8746-d4e2d413c7c8">
<br>
각 함수 기능들을 적어놨으나 가독성이 적은 부분들만 고려하여 작성하였으니 잘 참고하시면 될 것 같습니다.
<br>
<img width="167" alt="image" src="https://github.com/InSange/JumpNyang/assets/51250442/076c8199-a231-43e4-8282-c9c553fc1f59">
<br>
각 매니저들 기능 ( 각각 캔버스 기능을 담당하는 스크립트들 )
<br>
- AchiveInfoManager - 퀘스트 캔버스 담당 매니저입니다. 진행해야하는 퀘스트들과 클리어한 퀘스트들 슬롯들을 생성하고 표시하는 스크립트 + 리더보드' <br>
- AdMobManager - 애드몹 광고 생성하는 스크립트입니다. <br>
- BookManager - 요리와 재료 도감입니다. 요리 정보들과 재료 정보들을 불러와 슬롯을 생성하고 데이터들을 표시해주는 스크립트 입니다.<br>
- CharacterManager - 캐릭터 상점입니다. 캐릭터 데이터들을 불러와 슬롯화시켜주고 스크롤뷰까지 적용하는 스크립트입니다.<br>
- GameManager - 언어, 플레이어 데이터 정보, 뒤로가기 버튼 등의 민감한 정보들과 기능들을 다루는 스크립트입니다. 플레이어 데이터들을 저장하고 세이브 할때 데이터들을 전달해주는 형식입니다.<br>
- InGameManger - 전체적인 게임 시스템으로 통합 UI관리 및 매니저들을 관리하는 스크립트입니다.<br>
- IngredientShopManager - 재료 상점입니다. 재료 데이터들을 불러와 판매하는 슬롯들을 관리하고 상점 할머니와 상호작용하는 시스템들을 구현한 스크립트입니다.<br>
- ItemSatatusManager - 아이템 통합 관리 매니저입니다. 재료, 캐릭터, 맵, 요리 등의 주 콘텐츠들에 대한 데이터들을 저장하고 다루는 스크립트입니다.<br>
- MapShopManager - 맵 상점입니다. 맵 데이터들을 불러와 슬롯화시키고 구매 판매하는 기능들을 가진 스크립트입니다.<br>
- ObstacleManager - 장애물 매니저입니다. 장애물들을 소환하고 패턴들을 적용시키는 함수 입니다. 기본적으로 오브젝트 풀링기법으로 용량 최적화 관리 시스템입니다.<br>
- QuestManager - 퀘스트 매니저 입니다. AchiveInfoManager는 퀘스트를 시각화 시키고 QuestManager는 퀘스트를 Data를 들고와 전처리 담당이라고 보시면 됩니다.<br>
- SoundManager - 배경 및 효과음 담당 매니저입니다.<br>
- TutorialManager - 튜토리얼 전용 매니저입니다. 원래 튜토리얼은 OnEvent로 델리게이션해주는게 좋으나 인게임과 별도로 구성하기에 따로 빼서 작성해준 스크립트입니다.<br>
- UIManager - ui 통합 관리 매니저 입니다. 메인화면, 인게임화면, 종료화면 등을 담당하고 있으며 ui 언어별로 적용하기위해 사용하는 관리 스크립트입니다.<br>
- UserDataManager - 유저 데이터들을 관리하는 매니저입니다. 세이브 데이터들을 관리하고 작성하며 Json이지만 가장 좋은 방식은 바이너리 변환이 좋다고 최근에 공부한 경험이있는데 나중에 제꺼 보시면서 수정할지 말지 결정하시면 될 것 같습니다.<br>
<br>
파이어베이스 버전 11.2.0<br>
구글모바일애드몹 8.4.1<br>
구글 플레이 게임즈 플러그인 10.14<br>
ui 확장 팩 - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Controls/ScrollSnap<br>

세이브 기능하고 로그인 기능 등 참고하신다고 하셨는데 그냥 쓰시면 됩니다!
