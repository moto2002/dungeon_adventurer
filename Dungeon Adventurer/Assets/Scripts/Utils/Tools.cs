#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class Tools : MonoBehaviour
{

    const string Bootstrappath = "Assets/Scenes/Bootstrap.unity";
    const string Trainingpath = "Assets/AI_Training/Basic/Scenes/AITrainingGround.unity";
    const string DataKey = "SavedScenes";

    static Tools()
    {
        EditorApplication.playModeStateChanged -= RestoreScenes;
        EditorApplication.playModeStateChanged += RestoreScenes;
    }

    [MenuItem("Tools/Start Game %#o")]
    static void StartGame()
    {
        if (!SaveOpenScenes()) return;

        EditorSceneManager.OpenScene(Bootstrappath);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Tools/Start Training %#t")]
    static void StartTraining()
    {
        if (!SaveOpenScenes()) return;

        EditorSceneManager.OpenScene(Trainingpath);
        EditorApplication.isPlaying = true;
    }

    static bool SaveOpenScenes()
    {
        var scenePaths = new string[SceneManager.sceneCount];

        for (var i = 0; i < scenePaths.Length; i++)
        {

            var scene = SceneManager.GetSceneAt(i);
            scenePaths[i] = scene.path;
            if (!scene.isLoaded)
            {
                scenePaths[i] += "#unloaded";
            }
        }

        PlayerPrefs.SetString(DataKey, string.Join(";", scenePaths));

        return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }

    static void RestoreScenes(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredEditMode)
        {
            return;
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString(DataKey))) return;

        var joinedPaths = PlayerPrefs.GetString(DataKey);
        var scenePaths = joinedPaths.Split(';');
        for (var i = 0; i < scenePaths.Length; i++)
        {

            var pathData = scenePaths[i].Split('#');
            var path = pathData[0];
            var openSceneMode = pathData.Length > 1 ? OpenSceneMode.AdditiveWithoutLoading : OpenSceneMode.Additive;
            EditorSceneManager.OpenScene(path, i == 0 ? OpenSceneMode.Single : openSceneMode);
        }

    }
}
#endif
