using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{   // 음향 매니저
    public static SoundManager soundManager = null;
    public float masterVolumeSFX = 1f;  // 효과음
    public float masterVolumeBGM = 1f;  // 배경음

    [SerializeField]
    AudioClip[] BGMClip;    // 배경 클립들
    [SerializeField]
    AudioClip[] audioClip;  // 효과 클립들
    [SerializeField]
    Dictionary<string ,AudioClip> audioClipsDic;    // 키값으로 클립들 찾기
    [SerializeField]
    Dictionary<string, AudioClip> bgmClipsDic;
    [SerializeField]
    AudioSource sfxPlayer;  // 효과음 재생 플레이어
    [SerializeField]
    AudioSource bgmPlayer;  // 배경음 재생 플레이어

    [SerializeField]
    string curBgm;  // 현재 브금
    
    void Awake()
    {
        if(soundManager != null && soundManager != this)
            Destroy(gameObject);
        else
            soundManager = this;
        
        DontDestroyOnLoad(gameObject);

        audioClipsDic = new Dictionary<string, AudioClip>();
        bgmClipsDic = new Dictionary<string, AudioClip>();
        foreach(AudioClip a in audioClip)
        {
            audioClipsDic.Add(a.name, a);
        }
        foreach(AudioClip b in BGMClip)
        {
            bgmClipsDic.Add(b.name, b);
        }
        curBgm = "Login";
    }

    public void SetBGM(string bgm)
    {   // 해당하는 브금을 실행하는 함수
        if(curBgm == bgm) return;
        switch(bgm)
        {
            case "Login":
                bgmPlayer.clip = bgmClipsDic[bgm];
                bgmPlayer.Play();
                curBgm = bgm;
                return;
            case "Tutorial":
                bgmPlayer.clip = bgmClipsDic[bgm];
                bgmPlayer.Play();
                curBgm = bgm;
                return;
            case "Main":
                bgmPlayer.clip = bgmClipsDic[bgm];
                bgmPlayer.Play();
                curBgm = bgm;
                return;
            default:
                return;
        }
    }

    public void SetSFX(string sfx)
    {   // 효과음 실행
        sfxPlayer.PlayOneShot(audioClipsDic[sfx]);
    }

    public void ButtonClickSFX()
    {   // 버튼 효과음
        SetSFX("click");
    }
}
