using BayatGames.SaveGameFree;
using System.IO;
using UnityEngine;

public class Initialiser : MonoBehaviour
{
    void Start()
    {
        Debug.LogWarning("RRRR Init");
        ServiceRegistry.Init();

        var init = PlayerPrefs.GetInt("Init");
        if (init == 0)
        {
            ServiceRegistry.Currency.CreditCurrency(Currency.Coins, 100000);
            PlayerPrefs.SetInt("Init", 1);
        }

        SceneManagement._manager.ShowStart();
    }

    private T LoadJson<T>(string jsonPath)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonPath);
        string dataAsJson = File.ReadAllText(filePath);
        T loadedData = JsonUtility.FromJson<T>(dataAsJson);
        return loadedData;
    }
}
