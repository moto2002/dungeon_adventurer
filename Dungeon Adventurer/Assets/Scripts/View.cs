using System.Collections;
using UnityEngine;

public abstract class View : MonoBehaviour {

    public string _sceneName;
    public Animator _animator;
    public GameObject content;

    private Controller _controller;
    public Controller Controller {
        get { return _controller; }
        set
        {
            _controller = value;
            OnControllerChanged(value);
        }
    }
    public virtual void AfterShow() {
    }

    public virtual void OnControllerChanged(Controller newController) {
        Debug.Log("here");
    }

    public void Show() {
        content.SetActive(true);
        _animator.SetBool("Active", true);
        AfterShow();
    }

    public virtual void Close() {
        _animator.SetTrigger("Hide");
        StartCoroutine(HideView());
    }

    IEnumerator HideView() {
        yield return new WaitUntil(() => !_animator.GetBool("Visible"));
        UIManager._instance.RemoveScene(_sceneName);
    }
}
