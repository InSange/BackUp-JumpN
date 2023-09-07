using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.iOS;
//using UnityEngine.SocialPlatforms.GameCenter;

public class PlayGamesConnect : MonoBehaviour
{
    [SerializeField]
    GameObject Go_Google;   // 구글 플레이 로그인 버튼
    [SerializeField]
    GameObject Go_Apple;    // 애플 로그인 버튼
    [SerializeField]
    GameObject Go_Guest;    // 게스트

    string path;    // 로컬 파일 위치
    string filename;    // 파일명

    [SerializeField]
    TMP_Text __google__;    // 로그인 테스트용 텍스트

    bool isGuest = false;   // 게스트 확인용 변수



    //LoginScene
    public GameObject Agreement;    // 게스트로 이용시 이용약관에 대해서
    public GameObject TimeCheck;    // 현재 시간 확인

    bool TermsAndConditions;    // 이용 약관 체크 확인.
    bool PersonalInformation;   // 개인 정보 수집 동의 체크 확인.


    private void Start()
    {
        path = Application.persistentDataPath + "/";    // 로컬 저장소 위치 입력
        filename = "save.atree";    // 세이브 파일 명

#if UNITY_ANDROID   // 기기가 안드로이드일 때
        Go_Google.SetActive(true);  // 구글 로그인 버튼 ON
        Go_Guest.SetActive(true);   // 게스트 버튼 ON
        // 플레이게임즈 플랫폼을 사용하기 위해서 활성화 시켜줌.
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        if (haveSaveFile());    // 세이브 파일이 존재한다면
            //Login();

#elif UNITY_IOS     // 기기가 아이폰일 때
        Go_Apple.SetActive(true);   // 애플 로그인 버튼 ON
        Go_Guest.SetActive(true);   // 게스트 버튼 ON
#endif
    }

    public bool haveSaveFile()  // path 경로에 filename 이름의 세이브 파일이 존재하는지 확인하는 함수
    {
        try
        {   // data 정보를 불러온다.
            string data = File.ReadAllText(path + filename);
            print("파일을 찾았어요!");
            return true;
        }
        catch (System.Exception )
        {
            print("파일이 없는데요?");
            return false;
        }
    }

    public void Login() // 안드로이드 및 아이폰 로그인 버튼을 눌렸을 시.
    {   // 로그인시 게스트는 아님.
        if (!isGuest)
        {
    #if UNITY_ANDROID   // 안드로이드일 때 구글 플레이 계정이 인증(확인) 되지 않았을 때
            if (!PlayGamesPlatform.Instance.localUser.authenticated)
            {   //UnityEngine.Social.localUser 는 Androiod와 iOS 두 플랫폼을 모두 지원하고
                //현재 앱의 Platform을 알아서 판단하여
                //Android는 구글 플레이 게임 서비스의 api를
                //iOS는 애플 게임선터의 api를 호출하도록 할 수 있습니다.
                //localUser를 통해 사용자 인증을 진행하고, 사용자이름, 식별자, 미성년여부 등을 얻어올 수 있다.
                Social.localUser.Authenticate((bool success) =>
                {   //사용자 인증 성공시 로딩화면
                    if (success)
                    {
                        __google__.text = Social.localUser.id + " : " + Social.localUser.userName;
                        MoveToNextScene();
                    }
                    else
                        __google__.text = "Failed";
                });
            }
    #elif UNITY_IOS // 안드로이드와 동일
            if(Social.localUser.authenticated == true)
            {
                __google__.text = "ios Success";
            }
            else
            {
                Social.localUser.Authenticate((bool success) =>
                {
                    if(success)
                    {
                        __google__.text = Social.localUser.id + " : " + Social.localUser.userName;
                        MoveToNextScene();
                    }
                    else
                    {
                        __google__.text = "Failed";
                    }
                });
            }
    #endif
        }
        else if(isGuest)    // 게스트면 바로 로딩창으로.
        {
            GameManager.gm.isGuest = true;
            UserDataManager.userDataManager.LoadGuestData();
            MoveToNextScene();
        }
    }

    public void LogOut()    // 로그아웃.
    {
#if UNITY_ANDROID
        ((PlayGamesPlatform)Social.Active).SignOut();
        __google__.text = "구글로그아웃";
#elif UNITY_IOS // IOS는 로그아웃 제공하지않음.

#endif
    }

    public void ShowLeaderBoard()
    {
        __google__.text = "ShowLeaderBoard";

        Social.ShowLeaderboardUI(); // 리더보드 불러오기용
    }

    public void AddLeaderBoard()
    {
    }

    void MoveToNextScene()
    {
        LoadingSceneController.LoadScene("InGame"); 
    }

    public void NextLoadScene()
    {
        LoadingSceneController.LoadScene("InGame");
    }
    public void GuestPopup()    // 게스트 버튼 클릭시 게스트 true 후 로그인 진행
    {
        isGuest = true;
        Login();
    }


    public void Check_Agreement()
    {
        if(TermsAndConditions && PersonalInformation)   // 체크 항목이 둘다 되어있는가
        {
            TimeCheck.SetActive(true);  // 시간 확인
            Agreement.SetActive(false); // 동의서 OFF
        }
    }

    public void SetTermsAndConditions(bool isTrue)
    {
        TermsAndConditions = isTrue;    // 로그인할때 동의서 확인하는 함수
    }

    public void SetPersonalInformation(bool isTrue)
    {
        PersonalInformation = isTrue;   // 로그인할때 동의서 확인하는 함수
    }
}
