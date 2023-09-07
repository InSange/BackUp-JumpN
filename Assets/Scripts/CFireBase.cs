using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Analytics;
using System;

public class CFireBase : MonoBehaviour
{
    DatabaseReference m_Reference;
    FirebaseApp app;
    
    void Start()
    {   // ���̾� ���̽� ����
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        Debug.Log("���̾�̽� �����õ�");
        if (dependencyStatus == Firebase.DependencyStatus.Available) {  // ���̾�̽��� ����� �����Ҷ�
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            FirebaseAnalytics.LogEvent(name: "testEvent");  // testEvent ���� ��
            app = Firebase.FirebaseApp.DefaultInstance; // app ������ ���̾�̽� �⺻ �ν��Ͻ��� �־��ش�.
            m_Reference = FirebaseDatabase.DefaultInstance.GetReference("rank");    // ����� app���� �Ӹ��κ��� rank�� ���� -> ./rank/ ������ ���� ��ġ�� ������.
            // Set a flag here to indicate whether Firebase is ready to use by your app.
        } else {
            Debug.Log("���̾�̽� ����");
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
    }   
 
    void ReadUserData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users")
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
            for ( int i = 0; i < snapshot.ChildrenCount; i++)
                Debug.Log(snapshot.Child(i.ToString()).Child("name").Value + " -> " + snapshot.Child(i.ToString()).Child("score").Value + " login Time : " + snapshot.Child(i.ToString()).Child("loginTime").Value);
            }
        });
    }
    // ���̾�̽��� �����͵��� �ִ� ���.
    void WriteUserData(string userId, string username)
    {
        m_Reference.Child(userId).Child("name").SetValueAsync(username);
        m_Reference.Child(userId).Child("score").SetValueAsync(GameManager.gm.score);
        m_Reference.Child(userId).Child("loginTime").SetValueAsync(DateTime.Now.ToString());
    }
    // ========================= ���̾�̽� �̺�Ʈ ���� Ȯ�� ===============================================
    public void OnClickLogEvent()
    {
        // Log an event with no parameters.
        WriteUserData("3", name);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Login");
    }

    public void OnClickLogEventWithParam()
    {
        // Log an event with no parameters.
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Score", "percent", UnityEngine.Random.Range(0, 100));
    }
}