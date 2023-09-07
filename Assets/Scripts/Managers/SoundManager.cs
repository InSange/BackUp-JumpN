using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{   // ���� �Ŵ���
    public static SoundManager soundManager = null;
    public float masterVolumeSFX = 1f;  // ȿ����
    public float masterVolumeBGM = 1f;  // �����

    [SerializeField]
    AudioClip[] BGMClip;    // ��� Ŭ����
    [SerializeField]
    AudioClip[] audioClip;  // ȿ�� Ŭ����
    [SerializeField]
    Dictionary<string ,AudioClip> audioClipsDic;    // Ű������ Ŭ���� ã��
    [SerializeField]
    Dictionary<string, AudioClip> bgmClipsDic;
    [SerializeField]
    AudioSource sfxPlayer;  // ȿ���� ��� �÷��̾�
    [SerializeField]
    AudioSource bgmPlayer;  // ����� ��� �÷��̾�

    [SerializeField]
    string curBgm;  // ���� ���
    
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
    {   // �ش��ϴ� ����� �����ϴ� �Լ�
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
    {   // ȿ���� ����
        sfxPlayer.PlayOneShot(audioClipsDic[sfx]);
    }

    public void ButtonClickSFX()
    {   // ��ư ȿ����
        SetSFX("click");
    }
}
