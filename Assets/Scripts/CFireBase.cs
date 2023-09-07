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
    {   // 파이어 베이스 구동
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        Debug.Log("파이어베이스 연동시도");
        if (dependencyStatus == Firebase.DependencyStatus.Available) {  // 파이어베이스가 사용이 가능할때
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            FirebaseAnalytics.LogEvent(name: "testEvent");  // testEvent 전달 후
            app = Firebase.FirebaseApp.DefaultInstance; // app 변수에 파이어베이스 기본 인스턴스를 넣어준다.
            m_Reference = FirebaseDatabase.DefaultInstance.GetReference("rank");    // 연결된 app에서 머리부분을 rank로 설정 -> ./rank/ 형식의 폴더 위치로 가있음.
            // Set a flag here to indicate whether Firebase is ready to use by your app.
        } else {
            Debug.Log("파이어베이스 실패");
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
    // 파이어베이스에 데이터들을 넣는 방법.
    void WriteUserData(string userId, string username)
    {
        m_Reference.Child(userId).Child("name").SetValueAsync(username);
        m_Reference.Child(userId).Child("score").SetValueAsync(GameManager.gm.score);
        m_Reference.Child(userId).Child("loginTime").SetValueAsync(DateTime.Now.ToString());
    }
    // ========================= 파이어베이스 이벤트 연동 확인 ===============================================
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