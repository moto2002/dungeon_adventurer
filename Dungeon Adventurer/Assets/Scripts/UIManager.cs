using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{


    public static UIManager _instance;

    private List<SceneInfo> _stackedScenes;


    private void Awake()
    {
        _instance = this;

        _stackedScenes = new List<SceneInfo>();
    }

    //public View LoadScene( string name, bool show = true)
    //{
    //    GameObject rootObject;

    //    SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    //    Scene loadedScene = SceneManager.GetSceneByName(name);

    //    Debug.Log(loadedScene.isLoaded);

    //    GameObject[] rootObjects = loadedScene.GetRootGameObjects();
    //    rootObject = rootObjects[0];

    //    //rootObject.SetActive(show);

    //    View loadedView = rootObject.GetComponent<View>();

    //    if (show)
    //        loadedView._animator.SetBool("Active", true);

    //    SceneInfo info = new SceneInfo();
    //    info._attachedScene = loadedScene;
    //    info._attachedView = loadedView;
    //    info._rootObject = rootObject;
    //    _stackedScenes.Push(info);

    //    Debug.Log("here");
    //}

    public IEnumerator LoadScene(string name, bool show = true)
    {
        GameObject rootObject;

        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        Scene loadedScene = SceneManager.GetSceneByName(name);

        yield return new WaitUntil(() => loadedScene.isLoaded);
        GameObject[] rootObjects = loadedScene.GetRootGameObjects();
        rootObject = rootObjects[0];

        //rootObject.SetActive(show);

        View loadedView = rootObject.GetComponent<View>();

        if (show)
            loadedView._animator.SetBool("Active", true);

        SceneInfo info = new SceneInfo();
        info._attachedScene = loadedScene;
        info._attachedView = loadedView;
        info._rootObject = rootObject;
        _stackedScenes.Add(info);

        yield return loadedView;
    }

    public void Show(string name)
    {
        foreach (SceneInfo scene in _stackedScenes.ToArray())
        {
            if (scene._attachedScene.name.Equals(name))
            {
                scene._rootObject.SetActive(true);
                return;
            }
        }
        Debug.Log("Something wrong");
    }

    public void RemoveLastScene() {

        var scene = _stackedScenes[_stackedScenes.Count - 1];
        RemoveScene(scene._attachedScene.name);
    }

    public void RemoveScene(string name)
    {
        try
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(name));
        }
        catch (Exception e)
        {
            return;
        }
        foreach (SceneInfo scene in _stackedScenes.ToArray())
        {
            if (scene._attachedScene.name.Equals(name))
            {
                _stackedScenes.Remove(scene);
                return;
            }
        }
    }

    public void CloseScene(string name)
    {
        foreach (SceneInfo s in _stackedScenes.ToArray())
        {
            if (s._attachedScene.name.Equals(name))
            {
                s._attachedView.Close();
                return;
            }
        }
    }
}

public struct SceneInfo
{
    public Scene _attachedScene;
    public View _attachedView;
    public GameObject _rootObject;
}

public class CoroutineWithData
{
    public Coroutine coroutine { get; private set; }
    public object result;
    private IEnumerator target;
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = (object)target.Current;
            yield return result;
        }
    }
}
