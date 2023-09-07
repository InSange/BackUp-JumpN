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
    GameObject Go_Google;   // ���� �÷��� �α��� ��ư
    [SerializeField]
    GameObject Go_Apple;    // ���� �α��� ��ư
    [SerializeField]
    GameObject Go_Guest;    // �Խ�Ʈ

    string path;    // ���� ���� ��ġ
    string filename;    // ���ϸ�

    [SerializeField]
    TMP_Text __google__;    // �α��� �׽�Ʈ�� �ؽ�Ʈ

    bool isGuest = false;   // �Խ�Ʈ Ȯ�ο� ����



    //LoginScene
    public GameObject Agreement;    // �Խ�Ʈ�� �̿�� �̿����� ���ؼ�
    public GameObject TimeCheck;    // ���� �ð� Ȯ��

    bool TermsAndConditions;    // �̿� ��� üũ Ȯ��.
    bool PersonalInformation;   // ���� ���� ���� ���� üũ Ȯ��.


    private void Start()
    {
        path = Application.persistentDataPath + "/";    // ���� ����� ��ġ �Է�
        filename = "save.atree";    // ���̺� ���� ��

#if UNITY_ANDROID   // ��Ⱑ �ȵ���̵��� ��
        Go_Google.SetActive(true);  // ���� �α��� ��ư ON
        Go_Guest.SetActive(true);   // �Խ�Ʈ ��ư ON
        // �÷��̰����� �÷����� ����ϱ� ���ؼ� Ȱ��ȭ ������.
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        if (haveSaveFile());    // ���̺� ������ �����Ѵٸ�
            //Login();

#elif UNITY_IOS     // ��Ⱑ �������� ��
        Go_Apple.SetActive(true);   // ���� �α��� ��ư ON
        Go_Guest.SetActive(true);   // �Խ�Ʈ ��ư ON
#endif
    }

    public bool haveSaveFile()  // path ��ο� filename �̸��� ���̺� ������ �����ϴ��� Ȯ���ϴ� �Լ�
    {
        try
        {   // data ������ �ҷ��´�.
            string data = File.ReadAllText(path + filename);
            print("������ ã�Ҿ��!");
            return true;
        }
        catch (System.Exception )
        {
            print("������ ���µ���?");
            return false;
        }
    }

    public void Login() // �ȵ���̵� �� ������ �α��� ��ư�� ������ ��.
    {   // �α��ν� �Խ�Ʈ�� �ƴ�.
        if (!isGuest)
        {
    #if UNITY_ANDROID   // �ȵ���̵��� �� ���� �÷��� ������ ����(Ȯ��) ���� �ʾ��� ��
            if (!PlayGamesPlatform.Instance.localUser.authenticated)
            {   //UnityEngine.Social.localUser �� Androiod�� iOS �� �÷����� ��� �����ϰ�
                //���� ���� Platform�� �˾Ƽ� �Ǵ��Ͽ�
                //Android�� ���� �÷��� ���� ������ api��
                //iOS�� ���� ���Ӽ����� api�� ȣ���ϵ��� �� �� �ֽ��ϴ�.
                //localUser�� ���� ����� ������ �����ϰ�, ������̸�, �ĺ���, �̼��⿩�� ���� ���� �� �ִ�.
                Social.localUser.Authenticate((bool success) =>
                {   //����� ���� ������ �ε�ȭ��
                    if (success)
                    {
                        __google__.text = Social.localUser.id + " : " + Social.localUser.userName;
                        MoveToNextScene();
                    }
                    else
                        __google__.text = "Failed";
                });
            }
    #elif UNITY_IOS // �ȵ���̵�� ����
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
        else if(isGuest)    // �Խ�Ʈ�� �ٷ� �ε�â����.
        {
            GameManager.gm.isGuest = true;
            UserDataManager.userDataManager.LoadGuestData();
            MoveToNextScene();
        }
    }

    public void LogOut()    // �α׾ƿ�.
    {
#if UNITY_ANDROID
        ((PlayGamesPlatform)Social.Active).SignOut();
        __google__.text = "���۷α׾ƿ�";
#elif UNITY_IOS // IOS�� �α׾ƿ� ������������.

#endif
    }

    public void ShowLeaderBoard()
    {
        __google__.text = "ShowLeaderBoard";

        Social.ShowLeaderboardUI(); // �������� �ҷ������
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
    public void GuestPopup()    // �Խ�Ʈ ��ư Ŭ���� �Խ�Ʈ true �� �α��� ����
    {
        isGuest = true;
        Login();
    }


    public void Check_Agreement()
    {
        if(TermsAndConditions && PersonalInformation)   // üũ �׸��� �Ѵ� �Ǿ��ִ°�
        {
            TimeCheck.SetActive(true);  // �ð� Ȯ��
            Agreement.SetActive(false); // ���Ǽ� OFF
        }
    }

    public void SetTermsAndConditions(bool isTrue)
    {
        TermsAndConditions = isTrue;    // �α����Ҷ� ���Ǽ� Ȯ���ϴ� �Լ�
    }

    public void SetPersonalInformation(bool isTrue)
    {
        PersonalInformation = isTrue;   // �α����Ҷ� ���Ǽ� Ȯ���ϴ� �Լ�
    }
}
