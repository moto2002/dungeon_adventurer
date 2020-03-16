#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
public class SkillsEditor : EditorWindow
{
    SkillsManager _skillsManager;
    VisualElement _root;
    StyleSheet _styleSheet;
    VisualElement _collectionContainer;
    VisualElement _spellContainer;
    VisualElement _itemInfo;
    Label _title;
    Button _backToCollectionButton;
    Button _backToSkillsButton;
    Button _removeSkillButton;
    Skill _currentSelectedSkill;
    Sprite _defaultIcon;
    SkillsCollection _currentSelectedCollection;
    SerializedObject SerializedSkill;
    SerializedProperty SkillList;
    int ListSize;
    bool[] _possiblePositions;
    bool[] _possibleTargets;

    Editor componentEditor;

    [MenuItem("Window/SkillsEditor")]
    public static void ShowExample()
    {
        SkillsEditor wnd = GetWindow<SkillsEditor>();
        wnd.titleContent = new GUIContent("SkillsEditor");
    }

    public void OnGUI()
    {

        if (Event.current.commandName == "ObjectSelectorUpdated")
        {
            Debug.Log("here");
        }
    }

    public void OnEnable()
    {
        _skillsManager = AssetDatabase.LoadAssetAtPath<SkillsManager>("Assets/ScriptableObjects/Skills/SkillsManager.asset");
        _root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ScriptableObjects/Editor/SkillsEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        _root.Add(labelFromUXML);
        _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ScriptableObjects/Editor/SkillsEditor.uss");

        _defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Skill_PlaceHolder.png");

        if (_defaultIcon == null)
        {

            Debug.Log("Loaded");
        }

        _collectionContainer = _root.Q("CollectionContainer");
        _title = _root.Q("Title") as Label;

        _removeSkillButton = _root.Q("RemoveSkillButton") as Button;
        _removeSkillButton.clicked += RemoveSkill;
        _backToCollectionButton = _root.Q("BackButton") as Button;
        _backToCollectionButton.clicked += BackToCollection;
        _backToSkillsButton = _root.Q("BackToSkillsButton") as Button;
        _backToSkillsButton.clicked += BackToSkills;

        _collectionContainer.style.display = DisplayStyle.Flex;
        foreach (var collection in _skillsManager.Collections)
        {
            var button = new Button(() => SelectCollection(collection));
            button.text = collection.name;
            button.AddToClassList("CollectionButton");
            button.styleSheets.Add(_styleSheet);
            _collectionContainer.Add(button);
        }
    }

    void BackToCollection()
    {
        _collectionContainer.style.display = DisplayStyle.Flex;
        _spellContainer.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.None;
        _title.text = "SkillsEditor";
    }

    void BackToSkills()
    {
        _spellContainer.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.None;
        _backToSkillsButton.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.Flex;
        _removeSkillButton.style.display = DisplayStyle.None;

        SelectCollection(_currentSelectedCollection);
    }

    void SelectCollection(SkillsCollection selectedCollection)
    {
        _currentSelectedCollection = selectedCollection;
        _collectionContainer.style.display = DisplayStyle.None;

        _spellContainer = _root.Q("SpellsContainer");
        _spellContainer.Clear();
        _title.text = _currentSelectedCollection.name;

        foreach (var skill in _currentSelectedCollection.Skills)
        {
            var button = new Button(() => SelectSkill(skill));
            button.AddToClassList("SkillButton");
            button.styleSheets.Add(_styleSheet);

            var label = new Label(skill.displayName);
            label.AddToClassList("SkillButtonTitle");
            label.styleSheets.Add(_styleSheet);
            button.Add(label);

            var icon = new Image();
            icon.AddToClassList("SkillButtonIcon");
            icon.styleSheets.Add(_styleSheet);
            icon.image = skill.icon.texture;
            button.Add(icon);

            _spellContainer.Add(button);
        }

        var newButton = new Button(() => AddNewSkill());
        newButton.AddToClassList("SkillButton");
        newButton.styleSheets.Add(_styleSheet);

        var newLabel = new Label("Add Skill");
        newLabel.AddToClassList("SkillButtonTitle");
        newLabel.styleSheets.Add(_styleSheet);
        newButton.Add(newLabel);

        var newButtonicon = new Image();
        newButtonicon.AddToClassList("SkillButtonIcon");
        newButtonicon.styleSheets.Add(_styleSheet);
        newButtonicon.tintColor = Color.white;
        newButton.Add(newButtonicon);

        _spellContainer.Add(newButton);
        _spellContainer.style.display = DisplayStyle.Flex;
        _backToCollectionButton.style.display = DisplayStyle.Flex;
    }

    void AddNewSkill()
    {
        var newSkill = ScriptableObject.CreateInstance<Skill>();
        var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_currentSelectedCollection));
        newSkill.icon = _defaultIcon;
        newSkill.id = _currentSelectedCollection.Skills.Length != 0 ? _currentSelectedCollection.Skills.Max(skill => skill.id) + 1 : 1;

        AssetDatabase.CreateAsset(newSkill, assetPath + "/Skill" + newSkill.id + ".asset");
        if (_currentSelectedCollection.name.Equals("MonsterSpells"))
        {
            AssetDatabase.SetLabels(newSkill, new string[] { "MonsterSkill" });
        }
        _currentSelectedCollection.AddSkill(newSkill);

        SelectSkill(newSkill);
    }

    void RemoveSkill()
    {
        _currentSelectedCollection.RemoveSkill(_currentSelectedSkill);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_currentSelectedSkill));
        BackToSkills();
        SelectCollection(_currentSelectedCollection);
    }

    void SelectSkill(Skill selectedSkill)
    {
        _spellContainer.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.None;
        _removeSkillButton.style.display = DisplayStyle.Flex;

        _itemInfo = _root.Q("ItemInfo");
        _itemInfo.Clear();

        var iconPreview = new Image();
        iconPreview.style.width = 100;
        iconPreview.style.height = 100;
        // iconPreview.style.marginLeft = 10;
        iconPreview.image = selectedSkill.icon.texture;

        var icon = new ObjectField();
        icon.value = selectedSkill.icon;
        icon.objectType = typeof(Sprite);
        icon.RegisterValueChangedCallback(changeEvent =>
        {
            iconPreview.image = ((Sprite)changeEvent.newValue).texture;
            selectedSkill.icon = (Sprite)changeEvent.newValue;
            EditorUtility.SetDirty(selectedSkill);
        });

        _currentSelectedSkill = selectedSkill;
        SerializedSkill = new SerializedObject(_currentSelectedSkill);
        SkillList = SerializedSkill.FindProperty("SkillEffects");
        _possiblePositions = _currentSelectedSkill.possiblePositions;
        _possibleTargets = _currentSelectedSkill.possibleTargets;

        var costField = new IntegerField("Cooldown");
        costField.labelElement.style.minWidth = 60;
        costField.value = selectedSkill.cooldown;
        costField.RegisterValueChangedCallback(changeEvent =>
        {
            selectedSkill.cooldown = changeEvent.newValue;
            EditorUtility.SetDirty(selectedSkill);
        });

        var manaCostField = new IntegerField("Mana Cost");
        manaCostField.labelElement.style.minWidth = 60;
        manaCostField.value = selectedSkill.manaCost;
        manaCostField.RegisterValueChangedCallback(changeEvent =>
        {
            selectedSkill.manaCost = changeEvent.newValue;
            EditorUtility.SetDirty(selectedSkill);
        });

        var nameTextField = new TextField("Name");
        nameTextField.value = selectedSkill.displayName;
        nameTextField.labelElement.style.minWidth = 60;
        nameTextField.RegisterValueChangedCallback(changeEvent =>
        {
            selectedSkill.displayName = changeEvent.newValue;
            EditorUtility.SetDirty(selectedSkill);
        });

        var nameChangeButton = new Button(() =>
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedSkill), selectedSkill.displayName);
            EditorUtility.SetDirty(selectedSkill);
        });
        nameChangeButton.text = "Change GO Name";
        nameChangeButton.style.minWidth = 60;

        var descriptionField = new TextField("Description");
        descriptionField.value = selectedSkill.description;
        descriptionField.multiline = true;
        descriptionField.style.height = 80;
        descriptionField.name = "messageText";
        descriptionField.RegisterValueChangedCallback(changeEvent => { selectedSkill.description = changeEvent.newValue; });

        var passiveField = new Toggle("Passive Skill");
        passiveField.value = selectedSkill.isPassive;
        passiveField.RegisterValueChangedCallback(changeEvent => { selectedSkill.isPassive = changeEvent.newValue; });

        var targetTypeField = new EnumField("Target Type");
        targetTypeField.Init(selectedSkill.targetType);
        targetTypeField.RegisterValueChangedCallback(changeEvent => { selectedSkill.targetType = (TargetType)changeEvent.newValue; });

        var targetField = new EnumField("Target Site");
        targetField.Init(selectedSkill.targetSite);
        targetField.RegisterValueChangedCallback(changeEvent => { selectedSkill.targetSite = (TargetSite)changeEvent.newValue; });

        var amountField = new EnumField("Target Amount");
        amountField.Init(selectedSkill.targetCount);
        amountField.RegisterValueChangedCallback(changeEvent => { selectedSkill.targetCount = (TargetAmount)changeEvent.newValue; });

        var infoContainer = new VisualElement();
        var upperContainer = new VisualElement();
        var leftContainer = new VisualElement();
        leftContainer.style.width = 200;
        leftContainer.style.alignItems = Align.Center;
        leftContainer.Add(iconPreview);
        leftContainer.Add(icon);
        var rightContainer = new VisualElement();
        rightContainer.style.width = 200;
        rightContainer.Add(nameTextField);
        rightContainer.Add(nameChangeButton);
        rightContainer.Add(manaCostField);
        rightContainer.Add(costField);
        rightContainer.Add(descriptionField);

        upperContainer.Add(leftContainer);
        upperContainer.Add(rightContainer);
        upperContainer.style.flexDirection = FlexDirection.Row;

        var bottomContainer = new VisualElement();
        bottomContainer.style.marginLeft = 50;
        bottomContainer.style.marginTop = 25;
        bottomContainer.style.width = 300;
        bottomContainer.Add(passiveField);
        bottomContainer.Add(targetTypeField);
        bottomContainer.Add(targetField);
        bottomContainer.Add(amountField);

        infoContainer.Add(upperContainer);
        infoContainer.Add(bottomContainer);

        _itemInfo.Add(infoContainer);

        var positionBlock = new IMGUIContainer(() =>
        {
            GeneratePositionEditor();
        });

        var effectScroller = new ScrollView();
        effectScroller.style.minWidth = 350;
        effectScroller.style.maxHeight = 350;

        var effectBlock = new IMGUIContainer(() =>
        {
            GenerateEffectEditor();
        });

        effectScroller.Add(effectBlock);

        _itemInfo.Add(positionBlock);
        _itemInfo.Add(effectScroller);

        _backToSkillsButton.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.Flex;
    }

    //Old GUI Stuff I used in a CustomInspector for Skills
    void GeneratePositionEditor()
    {
        GUILayout.Space(25f);
        EditorGUILayout.LabelField("Possible Positions");

        var boxSize = 35f;
        MakeHorizontalField(ref _possiblePositions[2], ref _possiblePositions[1], ref _possiblePositions[0], ref _possibleTargets[0], ref _possibleTargets[1], ref _possibleTargets[2]);
        MakeHorizontalField(ref _possiblePositions[5], ref _possiblePositions[4], ref _possiblePositions[3], ref _possibleTargets[3], ref _possibleTargets[4], ref _possibleTargets[5], () => { GUILayout.Box("-->", new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, stretchHeight = true }, GUILayout.MaxHeight(boxSize), GUILayout.MinWidth(81f)); });
        MakeHorizontalField(ref _possiblePositions[8], ref _possiblePositions[7], ref _possiblePositions[6], ref _possibleTargets[6], ref _possibleTargets[7], ref _possibleTargets[8]);
        EditorGUILayout.Space();
        _currentSelectedSkill.possiblePositions = _possiblePositions;
        EditorUtility.SetDirty(_currentSelectedSkill);

        void MakeHorizontalField(ref bool firstPos, ref bool secondPos, ref bool thirdPos, ref bool firstTarget, ref bool secondTarget, ref bool thirdTarget, Action middlePart = null)
        {
            GUILayout.BeginHorizontal();
            firstPos = GUILayout.Toggle(firstPos, "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(boxSize), GUILayout.MinWidth(boxSize));
            secondPos = GUILayout.Toggle(secondPos, "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(boxSize), GUILayout.MinWidth(boxSize));
            thirdPos = GUILayout.Toggle(thirdPos, "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(boxSize), GUILayout.MinWidth(boxSize));

            if (middlePart != null)
            {
                middlePart?.Invoke();
            }
            else
            {
                GUILayout.Space(85);
            }

            if (_currentSelectedSkill.targetCount == TargetAmount.Single || _currentSelectedSkill.targetCount == TargetAmount.Multiple)
            {
                firstTarget = GUILayout.Toggle(firstTarget, "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(boxSize), GUILayout.MinWidth(boxSize));
                secondTarget = GUILayout.Toggle(secondTarget, "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(boxSize), GUILayout.MinWidth(boxSize));
                thirdTarget = GUILayout.Toggle(thirdTarget, "", GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.MinHeight(boxSize), GUILayout.MinWidth(boxSize));
            }
            GUILayout.EndHorizontal();
        }
    }

    void GenerateEffectEditor()
    {
        EditorGUILayout.Space();
        for (int i = 0; i < SkillList.arraySize; i++)
        {
            SerializedProperty MyListRef = SkillList.GetArrayElementAtIndex(i);
            SerializedProperty MyEffect = MyListRef.FindPropertyRelative("effect");

            _currentSelectedSkill.SkillEffects[i].effect = ModuleField("Skill Effect", _currentSelectedSkill.SkillEffects[i].effect);

            EditorGUILayout.Space();

            if (GUILayout.Button("Remove This Skill Effect"))
            {
                SkillList.DeleteArrayElementAtIndex(i);
                DestroyExistingModule(_currentSelectedSkill.SkillEffects[i].effect);
                EditorUtility.SetDirty(_currentSelectedSkill);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_currentSelectedSkill));
            }
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add New Skill Effect"))
        {
            _currentSelectedSkill.SkillEffects.Add(new SkillEffect());
            SelectSkill(_currentSelectedSkill);
        }
        SerializedSkill.ApplyModifiedProperties();
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
            Editor.CreateCachedEditor(result, null, ref componentEditor);
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
        clone.name = clone.name.Replace("Default", "");

        Undo.RegisterCreatedObjectUndo(clone, "Add Component");

        AssetDatabase.AddObjectToAsset(clone, _currentSelectedSkill);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_currentSelectedSkill));

        EditorUtility.SetDirty(_currentSelectedSkill);
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
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_currentSelectedSkill));
                    EditorUtility.SetDirty(_currentSelectedSkill);
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