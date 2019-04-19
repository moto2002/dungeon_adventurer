using BayatGames.SaveGameFree;
using System.IO;
using UnityEngine;

public class Initialiser : MonoBehaviour
{
    private CharacterData _characterData;

    [SerializeField] TextAsset json;

    void Awake()
    {
        RaceData racesData = LoadJson<RaceData>("races.json");
    }

    public void ShowShopView()
    {
        SceneManagement._manager.ShowShopView();
    }

    public void ShowInventoryView()
    {
        SceneManagement._manager.ShowInventoryView();
    }

    public void ShowSmithView()
    {
        SceneManagement._manager.ShowSmithView();
    }

    public void ShowTaverneView()
    {
        SceneManagement._manager.ShowTaverneView();
    }

    private T LoadJson<T>(string jsonPath)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonPath);
        string dataAsJson = File.ReadAllText(filePath);
        T loadedData = JsonUtility.FromJson<T>(dataAsJson);
        return loadedData;
    }
}
