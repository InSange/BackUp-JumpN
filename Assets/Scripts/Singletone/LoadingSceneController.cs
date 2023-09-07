using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.ComponentModel;

public class LoadingSceneController : MonoBehaviour
{   // �ε�ȭ�� ��ġ��.
    public static LoadingSceneController sharedInstance = null;
    public static string nextScene; // �Ѿ� �������ϴ� ���� �̸��� �����ϴ� ����

    [SerializeField]
    Image progressBar;  // ���α׷��� �� �̹��� ������Ʈ ����.

    [Header("�ε� �ӵ�����")]
    [Description("�ε� �ӵ�������ġ")]
    [SerializeField]
    float LoadingSpeed = 4f;    // �ε� �ӵ� ����

    private void Start()
    {
        if (sharedInstance != null && sharedInstance != this)
            Destroy(gameObject);
        else
            sharedInstance = this;

        nextScene = "InGame";   // nextScene�� �ΰ��� �� �̸��� ����.
        GameManager.gm.dataLoader.DataSetReady();   // �ε�ȭ���� �����ϰԵǸ� tsv���Ͽ��� �ҷ��� �����͵��� �ΰ��ӿ� ��������ش�.
        DontDestroyOnLoad(this);
        StartCoroutine(LoadScene());    // �ε� ����.
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "InGame") // ���� ���� InGame ���̶�� �ε� ��Ʈ�ѷ� ������Ʈ�� �������ش�.
        {
            GameManager.gm.dataLoader.InGameDataSetReady();
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string SceneName)
    {   // �ε�ȭ���� �ҷ����� �ε��� ������ SceneName ������ �Ѿ��. �����̱⿡ ��𿡼��� ȣ�� ����.
        nextScene = SceneName;
        SceneManager.LoadScene("Loading");
    }


    IEnumerator LoadScene()
    {
        yield return null;
        // �񵿱�� �ҷ��� ���� �ε��Ѵ�. �񵿱�� �ε��Ҷ� ȭ���� �����Ǵ� ���� ��ȭ������.
        UnityEngine.AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;    // next ������ �� �غ� �ȵǾ���.

        float timer = 0.0f;
        while(!op.isDone)
        {
            yield return new WaitUntil(() => GameManager.gm.dataLoader.isDataReady == true);    // ��� tsv������ �ҷ����������� ����Ѵ�.

            timer += Time.deltaTime;
            if(op.progress < 0.9f) { // ���� �� ä�����.
                progressBar.fillAmount = op.progress;
            }
            else {
                progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, 1f, Time.deltaTime / LoadingSpeed);
                if (progressBar.fillAmount >= 1f)   // ���� �ٰ� �� á����
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}