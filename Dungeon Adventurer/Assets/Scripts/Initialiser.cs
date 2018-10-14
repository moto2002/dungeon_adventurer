using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class Initialiser : MonoBehaviour {


    public static Initialiser _instance;

    public MonsterData _monsterData;
    private CharacterData _characterData;

    [SerializeField]
    TextAsset json;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);




        //    TextAsset asset = Resources.Load("characters.json") as TextAsset;

        //var myObject = JsonUtility.FromJson<QrCodeResult>(asset.ToString());

        _characterData = LoadJson<CharacterData>("characters.json");
        ItemData itemdate = LoadJson<ItemData>("items.json");
        CharacterCreateData chardata = LoadJson<CharacterCreateData>("characterStats.json");
        _monsterData = LoadJson<MonsterData>("monsters.json");
        _monsterData.Instantiate();

        RaceData racesData = LoadJson<RaceData>("races.json");


        DungeonData dungeonData = LoadJson<DungeonData>("dungeons.json");
        foreach (Dungeon d in dungeonData.dungeons)
            d.Instantiate();

        //Debug.Log(racesData.races[(int)loadedData.characters[0].race].growth[2]);
    }

    private T LoadJson<T>(string jsonPath)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonPath);
        string dataAsJson = File.ReadAllText(filePath);
        T loadedData = JsonUtility.FromJson<T>(dataAsJson);
        return loadedData;
    }


    public void Show(string name)
    {
        //StartCoroutine(ShowView(name));       
    }

    public void ShowShopView()
    {
        StartCoroutine(ShowView("View_Shop", new CharactersController(_characterData)));
    }

    public void ShowInventoryView()
    {
        StartCoroutine(ShowView("View_Inventory", new CharactersController(_characterData)));
    }

    public void ShowSmithView()
    {
        StartCoroutine(ShowView("View_Smith", new CharactersController(_characterData)));
    }

    public void ShowTaverneView()
    {
        StartCoroutine(ShowView("View_Taverne", new CharactersController(_characterData)));
    }

    IEnumerator ShowView(string name, Controller controller)
    {
        CoroutineWithData cd = new CoroutineWithData(this, UIManager._instance.LoadScene(name, false));
        yield return cd.coroutine;
        View view = (View)cd.result;
        view.Controller = controller;
        view.Show();
    }
}
