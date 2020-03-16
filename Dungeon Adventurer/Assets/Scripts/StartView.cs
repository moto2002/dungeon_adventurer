using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class StartView : View
{
    [SerializeField] Button shopButton;
    [SerializeField] Button taverneButton;
    [SerializeField] Button dungeonButton;
    [SerializeField] Button characterButton;
    [SerializeField] Button smithButton;

    [SerializeField] Button startButton;
    [SerializeField] GameObject loadingContent;
    [SerializeField] GameObject mainContent;
    [SerializeField] PlayableDirector startDirector;
    [SerializeField] CinemachineVirtualCamera mainCamera;
    [SerializeField] CinemachineVirtualCamera startCamera;

    protected override void Awake()
    {
        shopButton.onClick.AddListener(SceneManagement._manager.ShowShopView);
        taverneButton.onClick.AddListener(SceneManagement._manager.ShowTaverneView);
        dungeonButton.onClick.AddListener(SceneManagement._manager.ShowBattleOrderView);
        characterButton.onClick.AddListener(SceneManagement._manager.ShowInventoryView);
        smithButton.onClick.AddListener(SceneManagement._manager.ShowSmithView);

        startButton.onClick.AddListener(ShowStartCutscene);
    }

    void ShowStartCutscene()
    {
        loadingContent.SetActive(false);
        startCamera.gameObject.SetActive(false);
        startDirector.Play();
        startDirector.stopped += director => {
            mainContent.SetActive(true);
            mainCamera.gameObject.SetActive(true);
        };
    }
}
