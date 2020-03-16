using UnityEngine;
using DG.Tweening;
using RSG;
using UnityEngine.SceneManagement;

public abstract class View : EventListener
{
    const float ShowTime = 0.5f;
    const float HideTime = 0.2f;
    public Animator _animator;
    const string ShowClip = "Show";
    const string HideClip = "Hide";

    [SerializeField] bool shouldBeSetActive;
    public GameObject content;

    public IPromise Hide()
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"Tried to hide inactive Scene of type {name}. You should show it first!");
        }

        content.SetActive(false);
        gameObject.SetActive(false);
        return Promise.Resolved();
    }

    public virtual void AfterShow()
    {
    }

    public virtual IPromise Show()
    {
        gameObject.SetActive(true);
        //content.transform.localPosition = new Vector3(2000, 0, 0);
        content.SetActive(true);
        //content.transform.DOLocalMoveX(0, ShowTime);

        if (shouldBeSetActive)
        {
            SceneManager.SetActiveScene(gameObject.scene);
        }

        return Promise.Resolved().Then(() => AfterShow());
    }
}
