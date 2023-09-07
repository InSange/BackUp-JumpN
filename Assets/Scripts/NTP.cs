using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using TMPro;

public class NTP : MonoBehaviour
{
    public static NTP ntp;

    [SerializeField]
    string m_ServerUrl = "time.windows.com";
    //string m_ServerUrl = "time.google.com";

    DateTime m_ServerTime = DateTime.MinValue;
    public DateTime ServerTime => m_ServerTime;

    public Action<DateTime> OnTimeUpdated { get; set; }

    Thread m_GetTimeThread;

    private Coroutine m_TimeCorrection;
    // [SerializeField]
    // TMP_Text TimeText;
    // [SerializeField]
    // TMP_Text Device_TimeText;



    private void Awake()
    {
        if (ntp != null && ntp != this)
            Destroy(gameObject);
        else
            ntp = this;

        DontDestroyOnLoad(gameObject);

    }

    private void OnEnable()
    {
        StopThread();
        // 코루틴 사용시 응답까지 프리징이 걸리므로 쓰레드 사용하여 처리
        m_GetTimeThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    var ntpData = new byte[48];
                    ntpData[0] = 0x1B;

                    var addresses = Dns.GetHostEntry(m_ServerUrl).AddressList;
                    var ipEndPoint = new IPEndPoint(addresses[0], 123);

                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        socket.Connect(ipEndPoint);

                        socket.ReceiveTimeout = 3000;

                        socket.Send(ntpData);
                        socket.Receive(ntpData);
                        socket.Close();
                    }

                    const byte serverReplyTime = 40;
                    ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
                    ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

                    intPart = SwapEndianness(intPart);
                    fractPart = SwapEndianness(fractPart);

                    var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                    var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
                    // ToLocalTime() 함수를 통해서 UTC 시간에서 자동으로 한국 시간으로 변경된다.
                    m_ServerTime = networkDateTime.ToLocalTime();
                    OnTimeUpdated?.Invoke(m_ServerTime);
                    Debug.Log("시간 동기화 : " + m_ServerTime);
                }
                catch (Exception e)
                {
                    // 만약 초기값과 다르다면 한번이라도 서버 시간을 받아왔다는 뜻이므로 초기값일 때만 로컬 시간을 대신합니다.
                    if (m_ServerTime == DateTime.MinValue)
                    {
                        m_ServerTime = DateTime.Now;
                        Debug.Log("서버 시간을 가져오는 중 에러가 발생했습니다. 로컬 시간으로 대체합니다. : " + e);
                        // 쓰레드 종료
                        Thread.CurrentThread.Join();
                    }
                    else
                    {
                        Debug.Log("서버 시간을 가져오는 중 에러가 발생했습니다. 서버 시간을 가져온 기록이 있습니다. : " + e);
                    }
                }
                // 시간 서버에서 4초 안에 2번 이상의 호출이 될 경우 차단 될 수 있으니
                // 10초간의 딜레이를 가진다.
                Thread.Sleep(10000);
            }
        });
        m_GetTimeThread?.Start();
        StartTimeCorrection();
    }

    private void OnDisable()
    {
        StopThread();
        StopTimeCorrection();
    }

    void StartTimeCorrection()
    {
        StopTimeCorrection();
        m_TimeCorrection = StartCoroutine(CO_TimeCorrection());
    }

    void StopTimeCorrection()
    {
        if (m_TimeCorrection != null)
            StopCoroutine(m_TimeCorrection);
    }

    IEnumerator CO_TimeCorrection()
    {
        // 1초 시간체크
        var cahcedWait = new WaitForSeconds(1);
        yield return new WaitUntil(() => m_ServerTime != DateTime.MinValue);
        while (true)
        {
            yield return cahcedWait;
            m_ServerTime = m_ServerTime.AddSeconds(1);
            // 기기 시간과 서버 시간 연동을 확인하기 위해서 출력하였습니다.
            //TimeText.text = m_ServerTime.ToString();
            //Device_TimeText.text = DateTime.Now.ToString("yyyy-MM-dd ") + DateTime.Now.ToString("T");
            try
            {
                //Device_TimeText.text = DateTime.Now.ToString("HH:mm:ss");
            }
            catch (System.Exception e) { Debug.Log(e); }
            if (GameManager.gm.current_screen == "IngredientShop")   // 상점 갱신 시간 _ 만약 플레이어의 화면이 재료상점이라면 갱신시간을 체크해준다.
            {
                InGameManager.inGameManager.ingredientShopManager.Renew_Ingredient_Time();
            }
            //Debug.Log("시간 보정 : " + m_ServerTime);
        }
    }

    void StopThread()
    {
        // 쓰레드 즉시 종료
        if (m_GetTimeThread != null)
            m_GetTimeThread?.Abort();
    }

    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }

    // 서버 시간을 끌어쓰기위한 함수
    public DateTime getServerTime()
    {
        return m_ServerTime;
    }
}