using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class View : MonoBehaviour {

    public string _sceneName;

    public Animator _animator;


    private Controller _controller;
    public Controller Controller 
    {
        get {return _controller;}
        set
        {
            _controller = value;
            OnControllerChanged(value);
        }
    }


    public virtual void OnControllerChanged(Controller newController) {

        Debug.Log("here");

    }

    public void Show()
    {
        gameObject.SetActive(true);
        _animator.SetBool("Active", true);

    }

    public void Close()
    {
        _animator.SetTrigger("Hide");
        StartCoroutine(HideView());
        
    }

    IEnumerator HideView()
    {
        yield return new WaitUntil(() => !_animator.GetBool("Visible"));
        UIManager._instance.RemoveScene(_sceneName);
    }
}
