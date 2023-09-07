using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.ComponentModel;

public class LoadingSceneController : MonoBehaviour
{   // 로딩화면 장치다.
    public static LoadingSceneController sharedInstance = null;
    public static string nextScene; // 넘어 가고자하는 씬의 이름을 저장하는 변수

    [SerializeField]
    Image progressBar;  // 프로그레스 바 이미지 오브젝트 변수.

    [Header("로딩 속도제한")]
    [Description("로딩 속도제한장치")]
    [SerializeField]
    float LoadingSpeed = 4f;    // 로딩 속도 제한

    private void Start()
    {
        if (sharedInstance != null && sharedInstance != this)
            Destroy(gameObject);
        else
            sharedInstance = this;

        nextScene = "InGame";   // nextScene에 인게임 씬 이름을 적용.
        GameManager.gm.dataLoader.DataSetReady();   // 로딩화면이 시작하게되면 tsv파일에서 불러온 데이터들을 인게임에 적용시켜준다.
        DontDestroyOnLoad(this);
        StartCoroutine(LoadScene());    // 로딩 시작.
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "InGame") // 현재 씬이 InGame 씬이라면 로딩 컨트롤러 오브젝트를 제거해준다.
        {
            GameManager.gm.dataLoader.InGameDataSetReady();
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string SceneName)
    {   // 로딩화면을 불러오고 로딩이 끝나면 SceneName 씬으로 넘어간다. 정적이기에 어디에서든 호출 가능.
        nextScene = SceneName;
        SceneManager.LoadScene("Loading");
    }


    IEnumerator LoadScene()
    {
        yield return null;
        // 비동기로 불러올 씬을 로딩한다. 비동기는 로딩할때 화면이 정지되는 렉을 완화시켜줌.
        UnityEngine.AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;    // next 씬으로 들어갈 준비가 안되었음.

        float timer = 0.0f;
        while(!op.isDone)
        {
            yield return new WaitUntil(() => GameManager.gm.dataLoader.isDataReady == true);    // 모든 tsv파일이 불러와질때까지 대기한다.

            timer += Time.deltaTime;
            if(op.progress < 0.9f) { // 진행 바 채우는중.
                progressBar.fillAmount = op.progress;
            }
            else {
                progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, 1f, Time.deltaTime / LoadingSpeed);
                if (progressBar.fillAmount >= 1f)   // 진행 바가 다 찼을시
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}