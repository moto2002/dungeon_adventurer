#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Skill))]
public class CustomListEditor : Editor
{
    public Texture2D myGUITexture;
    Skill t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;

    bool[] pos;
    bool[] aim;
    Editor componentEditor;

    void OnEnable()
    {
        t = (Skill)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("SkillEffects"); // Find the List in our script and create a refrence of it
        pos = t.possiblePositions;
        aim = t.possibleTargets;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GetTarget.Update();

      /*  EditorGUILayout.Space();
        t.isPassive = EditorGUILayout.Toggle("Is Passive?", t.isPassive);
        EditorGUILayout.Space();
        t.targetType = (TargetType)EditorGUILayout.EnumPopup("Who to target?", t.targetType);
        EditorGUILayout.Space();
        t.targetCount = (TargetAmount)EditorGUILayout.EnumPopup("How many targets?", t.targetCount);
        EditorGUILayout.Space();
        t.icon = (Sprite)EditorGUILayout.ObjectField("Icon", t.icon, typeof(Sprite));
        */
        GUILayout.Space(25f);
        EditorGUILayout.LabelField("Possible Positions");

        EditorGUILayout.BeginHorizontal();
        pos[0] = GUILayout.Toggle(pos[0], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        pos[1] = GUILayout.Toggle(pos[1], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        pos[2] = GUILayout.Toggle(pos[2], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        GUILayout.Space(85);
        if (t.targetCount == TargetAmount.Single || t.targetCount == TargetAmount.Multiple)
        {
            aim[0] = GUILayout.Toggle(aim[0], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
            aim[1] = GUILayout.Toggle(aim[1], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
            aim[2] = GUILayout.Toggle(aim[2], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        pos[3] = GUILayout.Toggle(pos[3], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        pos[4] = GUILayout.Toggle(pos[4], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        pos[5] = GUILayout.Toggle(pos[5], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        var style = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, stretchHeight = true };
        if (t.targetCount == TargetAmount.Single || t.targetCount == TargetAmount.Multiple)
        {
            GUILayout.Box("-->", style, GUILayout.MaxHeight(50f), GUILayout.MinWidth(81f));
            aim[3] = GUILayout.Toggle(aim[3], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
            aim[4] = GUILayout.Toggle(aim[4], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
            aim[5] = GUILayout.Toggle(aim[5], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        pos[6] = GUILayout.Toggle(pos[6], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        pos[7] = GUILayout.Toggle(pos[7], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        pos[8] = GUILayout.Toggle(pos[8], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        GUILayout.Space(85);
        if (t.targetCount == TargetAmount.Single || t.targetCount == TargetAmount.Multiple)
        {
            aim[6] = GUILayout.Toggle(aim[6], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
            aim[7] = GUILayout.Toggle(aim[7], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
            aim[8] = GUILayout.Toggle(aim[8], "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(50f), GUILayout.MinWidth(50f));
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        t.possiblePositions = pos;
        for (int i = 0; i < ThisList.arraySize; i++)
        {
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty MyEffect = MyListRef.FindPropertyRelative("effect");

            t.SkillEffects[i].effect = ModuleField("Skill Effect", t.SkillEffects[i].effect);

            EditorGUILayout.Space();

            if (GUILayout.Button("Remove This Skill Effect"))
            {
                ThisList.DeleteArrayElementAtIndex(i);
                DestroyExistingModule(t.SkillEffects[i].effect);
            }
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add New Skill Effect"))
        {
            t.SkillEffects.Add(new SkillEffect());
        }

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }

    private T ModuleField<T>(string label, T current) where T : Effect
    {
        T result;
        EditorGUI.BeginChangeCheck();
        var module = (T)EditorGUILayout.ObjectField(label, current, typeof(T), false);
        if (EditorGUI.EndChangeCheck())
        {
            if (module != null && module != current)
            {
                if (DestroyExistingModule(current))
                {
                    result = CreateModuleFromTemplate(module);
                }
                else
                {
                    result = current;
                }
            }
            else
            {
                if (DestroyExistingModule(current))
                {
                    result = null;
                }
                else
                {
                    result = current;
                }
            }
        }
        else
        {
            result = current;
        }

        if (result != null)
        {
            CreateCachedEditor(result, null, ref componentEditor);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            componentEditor.OnInspectorGUI();

            GUILayout.EndVertical();
        }

        return result;
    }

    private T CreateModuleFromTemplate<T>(T template) where T : Effect
    {
        var clone = Instantiate(template);
        clone.name = clone.name.Replace("(Clone)", "");

        Undo.RegisterCreatedObjectUndo(clone, "Add Component");

        AssetDatabase.AddObjectToAsset(clone, t);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(t));

        EditorUtility.SetDirty(t);
        return clone;
    }

    private bool DestroyExistingModule(Effect module)
    {
        if (module != null)
        {
            if (AssetDatabase.IsSubAsset(module))
            {
                if (EditorUtility.DisplayDialog("Delete Module", "Are you sure you want to remove the existing module? This operation cannot be undone.", "Continue", "Cancel"))
                {
                    DestroyImmediate(module, true);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(t));
                    EditorUtility.SetDirty(t);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
}

#endif
