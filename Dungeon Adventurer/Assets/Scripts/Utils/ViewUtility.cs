using System;
using RSG;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ViewUtility
{
    public static event Action<View> OnViewChange;

    public static IPromise<TShow> HideThenShow<THide, TShow>() where THide : View where TShow : View
    {
        return Hide<THide>().Then(() => Show<TShow>());
    }

    public static IPromise<TShow> HideThenShow<THide, TShow>(Action<TShow> beforeShow) where THide : View where TShow : View
    {
        return Hide<THide>().Then(() => Show(beforeShow));
    }

    public static IPromise<TShow> ShowThenHide<TShow, THide>() where TShow : View where THide : View
    {
        return Show<TShow>().Then(view => Hide<THide>().Then(() => Promise<TShow>.Resolved(view)));
    }

    public static IPromise<TShow> ShowThenHide<TShow, THide>(Action<TShow> beforeShow) where TShow : View where THide : View
    {
        return Show(beforeShow).Then(view => Hide<THide>().Then(() => Promise<TShow>.Resolved(view)));
    }

    public static IPromise<T> Show<T>(Action<T> beforeShow = null) where T : View
    {
        return Load<T>()
            .Then(view =>
                {
                    beforeShow?.Invoke(view);
                    return DoShow(view);
                }
            );
    }

    public static IPromise Hide<T>() where T : View
    {
        var sceneName = typeof(T).Name;
        var scene = SceneManager.GetSceneByName(sceneName);

        if (scene.isLoaded)
        {
            return SceneLoader.GetRoot<T>(scene).Then(root => root.Hide());
        }

        throw new Exception($"Scene of type {sceneName} not loaded! Load first!");
    }

    public static IPromise<T> Load<T>() where T : View
    {
        return SceneLoader.GetRootObject<T>();
    }

    static IPromise<T> DoShow<T>(T view) where T : View
    {
        return view.Show().Then(() =>
        {
            OnViewChange?.Invoke(view);
            return Promise<T>.Resolved(view);
        });
    }
}

