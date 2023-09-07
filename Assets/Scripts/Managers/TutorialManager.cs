using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TutorialManager : MonoBehaviour, IPointerDownHandler
{
    [Header("Tutorial Screen")]
    [SerializeField]
    private GameObject tuto_theater;    // �ִϸ��̼� �κ��� �����ϴ� ������Ʈ �׷�
    [SerializeField]
    private Image tutorial_screen;  // �ִϸ��̼��� �׷����� ��ũ��
    [SerializeField]
    private GameObject cookImage;   // �丮 �̹��� (Ʃ�丮�� �Ϸ��Ҷ� ��)
    [SerializeField]
    public TMP_Text screen_Text;    // ��� - local �����Ͱ� -1�϶�
    [SerializeField]
    public TMP_Text anim_Text;  // �ִϸ��̼� ���
    [SerializeField]
    public Sprite tutorial_image;   // Ʃ�丮��� �̹��� ����� ���
    [SerializeField]
    public List<Sprite> animation_Images = new List<Sprite>();  // �ִϸ��̼ǵ��� �����س��� ����
    [SerializeField]
    public int tuto_anim_index = 0; // ���� �ִϸ��̼� �ε��� ��ȣ
    [SerializeField]
    public string current_tuto = "";    // ���� Ʃ�丮�� ���� ��Ȳ
    [SerializeField]
    private string[] tuto_progress = {"Tuto_Anim", "TutoJump", "Tuto_Avoid", "Tuto_UI"};    // Ʃ�丮�� ���� ����, �ִϸ��̼�, ����, ȸ��, ���� ����
    [SerializeField]
    public List<string> sinaID = new List<string>();    // �ó����� ID���� �����س��� ����Ʈ
    [SerializeField]
    public IEnumerator cor_tuto;    // Ʃ�丮��� �ڷ�ƾ. 
    [SerializeField]
    private bool canSkip = false;   // ��ŵ ���� ����
    [SerializeField]
    private bool isfadeOut = false; // ���̵� �ƿ� ����

    [Header("Touch Quest UI")]
    [SerializeField]
    public GameObject tuto_bullet;  // Ʃ�丮��� �ҷ�
    public Rigidbody2D tuto_bullet_rigid;   // Ʃ��ҷ��� ������ٵ�
    [SerializeField]
    private bool needJump = false;  // ���� üũ
    [SerializeField]
    private bool one_Jump = false;  // 1�� ���� ���� üũ
    [SerializeField]
    private bool double_Jump = false;   // 2�� ���� ���� üũ
    [SerializeField]
    private bool avoid = true;  // �Ѿ� ���ϱ� üũ
    [SerializeField]
    private TMP_Text Tuto_Text; // ���� �� ȸ�ǿ� �ؽ�Ʈ

    [Header("Fade Out Image")]
    [SerializeField]
    private Image fade_image;   // ���̵� �ƿ��� �̹��� (������)

    [Header("Player")]
    [SerializeField]
    private Player player;  // �÷��̾� ������

    [Header("Collider")]
    [SerializeField]
    private BoxCollider2D col;  // Ʃ�� �ҷ� �ݶ��̴�
    
    void OnEnable()
    {
        col = GetComponent<BoxCollider2D>();
        current_tuto = tuto_progress[0];
        for(int i = 1; i <= GameManager.gm.dataLoader.sinaData.Count; i++)  // �ó����� Ű������ ����Ʈ���ٰ� �����Ѵ�.
        {
            sinaID.Add("scnrid_"+ string.Format("{0:D2}", i));
            animation_Images.Add(tutorial_image);
        }
        Start_Tuto(current_tuto);
    }

    void Start_Tuto(string cur_tuto)
    {   // Ʃ�丮�� �����ϴ� �Լ� ���� Ʃ�丮�� cur_tuto�� ���� �����.
        current_tuto = cur_tuto;
        
        if(current_tuto == tuto_progress[0])    // �ִϸ��̼� Ʃ�丮��
        {
            tuto_theater.SetActive(true);   // �ִϸ��̼��� ��� ������Ʈ Ȱ��ȭ
            canSkip = true; // �ִϸ��̼��� ��ŵ�� �����ϴ�.
            if(cor_tuto != null) StopCoroutine(cor_tuto);   // ���� ���� �ڷ�ƾ ����
            if(current_tuto == tuto_progress[0]) cor_tuto = Tuto_Anim();    // ���� Ʃ�丮���� �ִϸ��̼��̸� �ִϸ��̼� �ڷ�ƾ���� ����
            else cor_tuto = Tuto_Anim();    // ���ܵ� �ִϸ��̼�����
            StartCoroutine(cor_tuto);   // �ִϸ��̼� ����
        }
        else if(current_tuto == tuto_progress[1])   // ���� Ʃ�丮��
        {
            player.Player_Set_Position(1);  // �÷��̾ �ΰ��� ������ ��ġ
            StopCoroutine(cor_tuto);
            cor_tuto = Tuto_Jump(); // ���� Ʃ�丮�� ����
            StartCoroutine(cor_tuto);
        }
        else if(current_tuto == tuto_progress[2])   // ȸ�� Ʃ�丮��
        {
            player.Player_Set_Position(1);  // �÷��̾ �ΰ��� ������ ��ġ
            tuto_theater.SetActive(false);
            StopCoroutine(cor_tuto);
            cor_tuto = Tuto_Avoid();    // ȸ�� Ʃ�丮�� ����
            StartCoroutine(cor_tuto);
        }
        else if(current_tuto == tuto_progress[3])   // �÷��̾ �ΰ������� ����
        {
            StopCoroutine(cor_tuto);
            cor_tuto = Tuto_UI();   // �ΰ��� UI ����
            StartCoroutine(cor_tuto);
        }
    }

    void Change_Anim()
    {   // �ִϸ��̼� ���� �Լ�. ���� �ִϸ��̼� �׸��� -1�� �ƴϸ� �ִϸ��̼� ����. -1�̸� ������ ȸ�� Ʃ�丮���� ����Ǳ⿡ �ִϸ��̼� â ��Ȱ��ȭ
        if(GameManager.gm.dataLoader.sinaData[sinaID[tuto_anim_index]][2] == "-1")
        {
            screen_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            screen_Text.gameObject.SetActive(true);
            tutorial_screen.gameObject.SetActive(false);
        }
        else
        {
            anim_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            tutorial_screen.sprite = animation_Images[tuto_anim_index];
            tutorial_screen.gameObject.SetActive(true);
            screen_Text.gameObject.SetActive(false);
        }
    }

    IEnumerator FadeOut()   // ���̵�ƿ� ����. alpha���� 1���� �ø�. (������ �̹���)
    {
        Debug.Log("���̵� �ƿ�" + sinaID[tuto_anim_index]);
        float fadetime = 0f;
        while (fadetime < 0.5f)
        {
            //Debug.Log(fadetime);
            fadetime += 0.01f;
            yield return new WaitForSeconds(0.01f);
            fade_image.color = new Color(0, 0, 0, (fadetime*2));
        }

        yield return new WaitForSeconds(2.0f);
        tuto_theater.SetActive(false);
        fade_image.color = new Color(0, 0, 0, 0);

        if(current_tuto == tuto_progress[0])    // �ִϸ��̼ǿ��� ���̵�ƿ��� �ʿ��� �� �����ϰ� ���̵�ƿ��� ���� �� �ٸ� Ʃ�丮�� ������ ����
        {   // �ִϸ��̼� -> ����, ���� -> �ִϸ��̼�, �ִϸ��̼� -> ȸ�� ��
            if(sinaID[tuto_anim_index] == "scnrid_07") Start_Tuto(tuto_progress[1]);
            else if(sinaID[tuto_anim_index] == "scnrid_18") Start_Tuto(tuto_progress[2]);
            else if(sinaID[tuto_anim_index] == "scnrid_28")
            {
                GameManager.gm.tutorial = true;
            
                InGameManager.inGameManager.MainMenuSet();
                InGameManager.inGameManager.fade_clear = true;
                InGameManager.inGameManager.FadeClear();

                this.gameObject.SetActive(false);
            }
        }
        else if(current_tuto == tuto_progress[1])
        {   // ���� Ʃ�丮���� ������ �÷��̾ ����ġ�� ������ �ִϸ��̼� Ʃ�丮�� �ٽ� ����
            player.Player_Set_Position(0);
            tuto_anim_index++;
            Start_Tuto(tuto_progress[0]);
        }
        else
        {   // ȸ��, �ΰ��� Ʃ�丮�� ���� �ִϸ��̼� Ʃ�丮��� �ٽ� ����
            Start_Tuto(tuto_progress[0]);
        }
    }

    IEnumerator Tuto_Anim()
    {   // �ִϸ��̼� Ʃ�丮�󿡼� ����, ȸ��, �ΰ������� �Ѿ�� �������� ��ŵ�� ����. ��ŵ�� �Ұ����Ҷ� ���̵� �ƿ� ������ ���� ���� Ʃ�丮�� ����
        if(tuto_anim_index < 24)
        {
            if((sinaID[tuto_anim_index] == "scnrid_07" || sinaID[tuto_anim_index] == "scnrid_18") && isfadeOut == false )
            {
                Change_Anim();
                isfadeOut = true;
            }
            else if(isfadeOut)
            {
                canSkip = false;
                isfadeOut = false;
                StartCoroutine(FadeOut());
            }
            else
            {
                Change_Anim();
            }
        }
        else
        {
            if(sinaID[tuto_anim_index] == "scnrid_28")
            {
                canSkip = false;
                cookImage.SetActive(false);
                Change_Anim();
                new WaitForSeconds(2.0f);
                StartCoroutine(FadeOut());
            }
            else
            {
                Change_Anim();
                if(sinaID[tuto_anim_index] == "scnrid_27") cookImage.SetActive(true);
            }
        }

        yield return null;
    }

    IEnumerator Tuto_Avoid()
    {   // ȸ�� Ʃ�丮��. Ʃ�丮��� �Ѿ��� ���ͼ� ȸ�Ǹ� �׽�Ʈ �Ѵ�.
        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];

        yield return new WaitForSeconds(2.0f);

        tuto_bullet.SetActive(true);

        yield return null;
    }

    IEnumerator Tuto_Jump()
    {   // ���� Ʃ�丮��. 1�� ����, 2�� ���� ����. needJump�� üũ
        WaitForSeconds waitForSec = new WaitForSeconds(1.0f);
        yield return waitForSec;

        if(!needJump && (!one_Jump || !double_Jump))
        {
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            if(sinaID[tuto_anim_index] == "scnrid_09" || sinaID[tuto_anim_index] == "scnrid_11") 
            {
                needJump = true;
            }
            else Start_Tuto(current_tuto);
        }
        else if(sinaID[tuto_anim_index] == "scnrid_13")
        {
            StartCoroutine(StoreIn());
        }
        else
        {
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            Start_Tuto(current_tuto);
        }
    }

    IEnumerator StoreIn()
    {   // ���� ���� �ִϸ��̼�
        WaitForFixedUpdate move_store_frame = new WaitForFixedUpdate();

        yield return new WaitForSeconds(1.0f);

        RectTransform player_rect = player.Get_Player_Rect();
        float player_x = player_rect.anchoredPosition.x;
        float player_y = player_rect.anchoredPosition.y;

        while(player_x < 980)
        {
            player_x += 10.0f;
            player_rect.anchoredPosition = new Vector2(player_x, player_y);
            yield return move_store_frame;
        }

        Tuto_Text.text = "";

        StartCoroutine(FadeOut());
    }

    IEnumerator Tuto_UI()
    {   // �ΰ��� UI ���� Ʃ�丮��
        yield return new WaitForSeconds(2.0f);
        InGameManager.inGameManager.uiManager.Tuto_UI_ON();
        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
        yield return new WaitForSeconds(2.0f);

        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
        yield return new WaitForSeconds(2.0f);

        Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
        yield return new WaitForSeconds(2.0f);

        Tuto_Text.text = "";
        InGameManager.inGameManager.uiManager.Map_Change(0);
        tuto_anim_index++;
        this.gameObject.SetActive(false);
        InGameManager.inGameManager.StartGameSet();
    }
    
    public void OnPointerDown(PointerEventData data)
    {   // �÷��̾� ȭ�� ��ġ�� ���� �� �÷��̾� ������ ���ÿ� ���� Ʃ�丮�� üũ
        if(current_tuto == tuto_progress[0] && canSkip)
        {
            Debug.Log("�ִϸ��̼� ��ŵ!");
            if(isfadeOut == false) tuto_anim_index++;
            Start_Tuto(tuto_progress[0]);
            return;
        }
        if(needJump) player.Jump();

        if(!one_Jump && player.isJump && current_tuto == tuto_progress[1] && needJump)
        {
            one_Jump = true;
            needJump = false;
            Start_Tuto(current_tuto);
        }
        else if(!double_Jump && player.doubleJump && current_tuto == tuto_progress[1] && needJump)
        {
            double_Jump = true;
            Start_Tuto(current_tuto);
        }
        else if(!avoid && current_tuto == tuto_progress[2] && needJump)
        {
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            tuto_bullet_rigid.AddForce(Vector2.left*3.0f, ForceMode2D.Impulse);
            avoid = true;
            needJump = false;
            current_tuto = tuto_progress[3];
            Start_Tuto(current_tuto);
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {   // Ʃ�� �ҷ������� ���� ������ ������ ��� ���߾��ٰ� �÷��̾� ���� �� �ٽ� ������.
        if(obj.name == "Tuto_bullet")
        {
            tuto_bullet_rigid.velocity = Vector2.zero;
            col.enabled = false;
            Tuto_Text.text = GameManager.gm.dataLoader.localData[GameManager.gm.dataLoader.sinaData[sinaID[++tuto_anim_index]][1]][(int)GameManager.gm.countryLang];
            avoid = false;
            needJump = true;
        }
    }
}
