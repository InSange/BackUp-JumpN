using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatusManager : MonoBehaviour
{
    public bool dataSetReady = false;
    [Header("Map")]
    [SerializeField]
    public Map[] maps;  // 맵 기본 데이터

    [Header("Character")]
    [SerializeField]
    public Character[] characters;  // 캐릭터 기본 데이터

    [Header("Cook")]
    [SerializeField]
    public Cook[] cooks;    // 요리 기본 데이터

    [Header("Ingredient")]
    [SerializeField]
    public Ingredient[] ingredients;    // 재료 기본 데이터
    [SerializeField]
    public Dictionary<string, Ingredient> ingredient_dic = new Dictionary<string, Ingredient>();    // 재료 딕셔너리 {키, 재료}
    [SerializeField]
    public Dictionary<string, int> character_num_dic = new Dictionary<string, int>();   // 캐릭터 번호 {키, 번호}

    public static ItemStatusManager itemStatusManager = null;

    void Start()
    {
        if (itemStatusManager != null && itemStatusManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            itemStatusManager = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void ItemDataLoad()
    {
        Set_Item_Info();
        dataSetReady = true;
    }

    public void Set_Item_Info()
    {   // 재료, 캐릭터, 맵, 요리 데이터들을 구글시트 데이터들로 채운다.
        Dictionary<string, List<string>> LangData = GameManager.gm.dataLoader.localData;

        Dictionary<string, List<string>> stuffData = GameManager.gm.dataLoader.stuffData;
        Dictionary<string, List<string>> stuffGradeData = GameManager.gm.dataLoader.stuffGradeData;
        string dataKey;

        for (int i = 0; i < ingredients.Length; i++)
        {
            dataKey = ingredients[i].GetkeyID();
            //InGameManager.inGameManager.uiManager.testTEXT[0].text = "?? " + i + " ??? ? " + dataKey;
            ingredients[i].SetName(LangData[stuffData[dataKey][3]][(int)GameManager.gm.countryLang]);
            //InGameManager.inGameManager.uiManager.testTEXT[1].text = "?? " + i + " ??? ?? " + ingredients[i].GetName();
            ingredients[i].SetProbability(float.Parse(stuffGradeData[stuffData[dataKey][4]][1]));
            ingredients[i].Cost = int.Parse(stuffData[dataKey][5]);
            //InGameManager.inGameManager.uiManager.testTEXT[2].text = "?? " + i + " ??? ?? " + ingredients[i].Cost;
            if (!ingredient_dic.ContainsKey(ingredients[i].GetName())) ingredient_dic.Add(ingredients[i].GetkeyID(), ingredients[i]);
            //ingredients[i].isUnlocked = dataList[i].isLock;
        }

        Dictionary<string, List<string>> catData = GameManager.gm.dataLoader.skinData;
        for (int i = 0; i < characters.Length; i++)
        {
            dataKey = characters[i].GetkeyID();

            characters[i].character_name = LangData[catData[dataKey][2]][(int)GameManager.gm.countryLang];
            characters[i].character_des = LangData[catData[dataKey][3]][(int)GameManager.gm.countryLang];
            characters[i].Cost = int.Parse(catData[dataKey][4]);
            if (character_num_dic.ContainsKey(characters[i].character_name)) character_num_dic.Remove(characters[i].character_name);
            character_num_dic.Add(characters[i].character_name, i);//catDataList[i].name, i);
        }
        Dictionary<string, List<string>> mapData = GameManager.gm.dataLoader.backGroundData;

        for (int i = 0; i < ItemStatusManager.itemStatusManager.maps.Length; i++)
        {
            dataKey = maps[i].GetkeyID();

            maps[i].stageName = LangData[mapData[dataKey][2]][(int)GameManager.gm.countryLang];
            maps[i].price = int.Parse(mapData[dataKey][3]);
        }
        //List<CookText> cookDataList = GameManager.gm.dataLoader.GetCookData();
        Dictionary<string, List<string>> cookData = GameManager.gm.dataLoader.cookData;

        for (int i = 0; i < cooks.Length; i++)
        {
            dataKey = cooks[i].GetkeyID();

            cooks[i].SetName(LangData[cookData[dataKey][3]][(int)GameManager.gm.countryLang]);
            cooks[i].SetDescription(LangData[cookData[dataKey][11]][(int)GameManager.gm.countryLang]);
            cooks[i].needIngredients.Clear();
            cooks[i].needIngredientsNum.Clear();
            for (int j = 4; j <= 9; j++)
            {
                if (cookData[dataKey][j] == "-1") break;
                var needInfo = cookData[dataKey][j].Split('-');

                cooks[i].needIngredients.Add(ingredient_dic[needInfo[0]]);
                cooks[i].needIngredientsNum.Add(int.Parse(needInfo[1]));
            }
        }
    }

    // Update is called once per frame
    public void Load_ItemStatus(ItemInfo info)
    {   // 재료, 맵, 상점, 요리에 관한 데이터들을 세이브 파일에서 불러와 로드시킨다.
        for (int i = 0; i < maps.Length; i++)
        {
            if (i == 0)
            {
                maps[0].isPurchased = true;
                maps[0].isUnlocked = true;
                continue;
            }
            if (info.maps[i])
            {
                maps[i].isPurchased = true;
                maps[i].isUnlocked = true;
            }
        }

        for (int i = 0; i < characters.Length; i++)
        {
            if (i == 0)
            {
                characters[0].isPurchased = true;
                continue;
            }
            if (info.characters[i])
            {
                characters[i].isPurchased = true;
            }
        }

        for (int i = 0; i < cooks.Length; i++)
        {
            if (info.cooks[i])
            {
                cooks[i].isPurchased = true;
            }
        }

        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i].isPurchased = true;
            ingredients[i].SetAmount(info.ingredients[i]);
        }
    }

    public ItemInfo Get_ItemStatus()
    {   // 재료, 맵, 요리, 캐릭터에 대한 정보들을 세이브 파일에 저장하도록 전달해준다.
        bool[] mapsData = new bool[11];
        for (int i = 0; i < maps.Length; i++)
        {
            mapsData[i] = maps[i].isPurchased;
        }

        bool[] cooksData = new bool[21];
        for (int i = 0; i < cooks.Length; i++)
        {
            cooksData[i] = cooks[i].isPurchased;
        }

        bool[] charactersData = new bool[15];
        for (int i = 0; i < characters.Length; i++)
        {
            charactersData[i] = characters[i].isPurchased;
        }

        int[] ingredientsData = new int[20];
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredientsData[i] = ingredients[i].GetAmount();
        }

        ItemInfo itemInfo = new ItemInfo(mapsData,
                                        charactersData,
                                        cooksData,
                                        ingredientsData);
        return itemInfo;
    }

    public Map getStage(int n)
    {
        return maps[n - 1];
    }
}