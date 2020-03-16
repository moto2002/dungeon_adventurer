using System;
using RSG;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public sealed class SceneLoader : UIBehaviour
{
    const float ScreenThreshold = 0.2f;

    static SceneLoader _instance;

    protected override void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("instance of view manager found - discarding...");
            Destroy(_instance.gameObject);
        }

        _instance = this;
    }

    public static IPromise<T> GetRootObject<T>() where T : UIBehaviour
    {
        var sceneName = typeof(T).Name;
        return GetOrLoadScene(sceneName).Then(GetRoot<T>);
    }

    public static IPromise<T> GetRoot<T>(Scene scene) where T : UIBehaviour
    {
        if (scene.rootCount > 1)
        {
            throw new Exception($"Scene \"{scene.name}\" must have exactly ONE root object (counted {scene.rootCount}) with the appropriate script attached!");
        }

        var go = scene.GetRootGameObjects()[0].GetComponent<T>();
        if (go == null)
        {
            return Promise<T>.Rejected(
                new InvalidOperationException($"GameObject not found in scene root 0 (type = {typeof(T).Name}"));
        }

        if (go.name != scene.name)
        {
            Debug.LogWarning($"Scene root objects should be named after their respective scenes (expected: {scene.name}, was: {go.name}");
        }

        return Promise<T>.Resolved(go);
    }

    static IPromise<Scene> GetOrLoadScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (scene.isLoaded)
        {
            return Promise<Scene>.Resolved(scene);
        }

        var returnPromise = new Promise();
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        op.completed += a =>
        {
            returnPromise.Resolve();
        };

        return returnPromise.Then(() =>
            {
                scene = SceneManager.GetSceneByName(sceneName);
                if (!scene.isLoaded)
                {
                    return Promise<Scene>.Rejected(
                        new InvalidOperationException("Scene not found (" + sceneName + ")"));
                }

                return Promise<Scene>.Resolved(scene);
            });
    }
}

