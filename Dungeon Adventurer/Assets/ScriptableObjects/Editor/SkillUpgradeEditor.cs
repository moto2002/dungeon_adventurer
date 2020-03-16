#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
public class SkillUpgradeEditor : EditorWindow
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
    Skill _currentSelectedSkill;
    Sprite _defaultIcon;
    SkillsCollection _currentSelectedCollection;
    SerializedObject SerializedSkill;
    SerializedProperty SkillList;
    int ListSize;
    bool[] _possiblePositions;
    bool[] _possibleTargets;

    int _shownLevel = 0;

    Editor componentEditor;

    [MenuItem("Window/SkillUpgradeEditor")]
    public static void ShowExample()
    {
        SkillUpgradeEditor wnd = GetWindow<SkillUpgradeEditor>();
        wnd.titleContent = new GUIContent("SkillUpgradeEditor");
    }

    public void OnEnable()
    {
        _skillsManager = AssetDatabase.LoadAssetAtPath<SkillsManager>("Assets/ScriptableObjects/Skills/SkillsManager.asset");
        _root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ScriptableObjects/Editor/SkillUpgradeEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        _root.Add(labelFromUXML);
        _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ScriptableObjects/Editor/SkillUpgradeEditor.uss");

        _defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Skill_PlaceHolder.png");

        _collectionContainer = _root.Q("CollectionContainer");
        _title = _root.Q("Title") as Label;

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

    void RefreshLevelButtons(VisualElement container)
    {
        for (var i = 0; i < 10; i++)
        {
            var o = i;
            var button = new Button(() => ChangeLevel(o));
            button.AddToClassList(o != _shownLevel ? "UpgradeLevelButton" : "UpgradeLevelButtonSelected");
            button.text = "" + (o + 1);
            button.styleSheets.Add(_styleSheet);
            container.Add(button);
        }
    }

    void ChangeLevel(int newLevel)
    {
        _shownLevel = newLevel;
        SelectSkill(_currentSelectedSkill);
    }

    void BackToCollection()
    {
        _collectionContainer.style.display = DisplayStyle.Flex;
        _spellContainer.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.None;
        _title.text = "SkillUpgradeEditor";
    }

    void BackToSkills()
    {
        _spellContainer.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.None;
        _backToSkillsButton.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.Flex;

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

        _spellContainer.style.display = DisplayStyle.Flex;
        _backToCollectionButton.style.display = DisplayStyle.Flex;
    }

    void SelectSkill(Skill selectedSkill)
    {
        _spellContainer.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.None;

        _itemInfo = _root.Q("ItemInfo");
        _itemInfo.Clear();

        var iconPreview = new Image();
        iconPreview.style.width = 100;
        iconPreview.style.height = 100;
        iconPreview.image = selectedSkill.icon.texture;

        _currentSelectedSkill = selectedSkill;
        SerializedSkill = new SerializedObject(_currentSelectedSkill);
        SkillList = SerializedSkill.FindProperty("SkillEffects");

        var nameTextField = new TextField("Name");
        nameTextField.value = selectedSkill.displayName;
        nameTextField.labelElement.style.minWidth = 60;
        nameTextField.RegisterValueChangedCallback(changeEvent => {
            selectedSkill.displayName = changeEvent.newValue;
            EditorUtility.SetDirty(selectedSkill);
        });

        var infoContainer = new VisualElement();
        var upperContainer = new VisualElement();
        var leftContainer = new VisualElement();
        leftContainer.style.width = 200;
        leftContainer.style.alignItems = Align.Center;
        leftContainer.Add(iconPreview);
        var rightContainer = new VisualElement();
        rightContainer.style.width = 200;
        rightContainer.Add(nameTextField);

        upperContainer.Add(leftContainer);
        upperContainer.Add(rightContainer);
        upperContainer.style.flexDirection = FlexDirection.Row;

        var bottomContainer = new VisualElement();
        bottomContainer.style.marginLeft = 50;
        bottomContainer.style.marginTop = 25;
        bottomContainer.style.marginBottom = 25;
        bottomContainer.style.width = 400;
        bottomContainer.style.flexDirection = FlexDirection.Row;
        bottomContainer.style.flexWrap = Wrap.Wrap;

        RefreshLevelButtons(bottomContainer);

        infoContainer.Add(upperContainer);
        infoContainer.Add(bottomContainer);

        _itemInfo.Add(infoContainer);

        var effectScroller = new ScrollView();
        effectScroller.style.minWidth = 350;
        //effectScroller.style.maxHeight = 450;

        SerializedProperty generalUpgrades = SerializedSkill.FindProperty("upgrades");
        if (generalUpgrades.arraySize != 0 && generalUpgrades.arraySize > _shownLevel)
        {
            var element = generalUpgrades.GetArrayElementAtIndex(_shownLevel);
            element.isExpanded = true;
            var upgradeProperty = new PropertyField(element);
            upgradeProperty.Bind(SerializedSkill);
            upgradeProperty.styleSheets.Add(_styleSheet);
            upgradeProperty.style.marginBottom = 25f;

            var title = new Label(GetPropertyType(element));
            title.AddToClassList("SkillButtonTitle");
            title.styleSheets.Add(_styleSheet);

            effectScroller.Add(title);
            effectScroller.Add(upgradeProperty);
        }

        for (int i = 0; i < SkillList.arraySize; i++)
        {
            SerializedProperty MyEffectRef = SkillList.GetArrayElementAtIndex(i);
            var effect = MyEffectRef.FindPropertyRelative("effect");
            if (effect.objectReferenceValue != null)
            {
                var testEffect = new SerializedObject(effect.objectReferenceValue);
                var upgradeArray = testEffect.FindProperty("upgrades");

                if (upgradeArray.arraySize == 0 || upgradeArray.arraySize <= _shownLevel) continue;

                var element = upgradeArray.GetArrayElementAtIndex(_shownLevel);
                element.isExpanded = true;
                var upgradeProperty = new PropertyField(element);
                upgradeProperty.Bind(testEffect);
                upgradeProperty.styleSheets.Add(_styleSheet);
                upgradeProperty.style.marginBottom = 25f;

                var tempId = i;
                upgradeProperty.RegisterCallback<ChangeEvent<int>>((e) => {
                    CopyEffect(tempId);
                });
                upgradeProperty.RegisterCallback<ChangeEvent<bool>>((e) => {
                    CopyEffect(tempId);
                });
                upgradeProperty.RegisterCallback<ChangeEvent<string>>((e) => {
                    CopyEffect(tempId);
                });
                upgradeProperty.RegisterCallback<ChangeEvent<float>>((e) => {
                    CopyEffect(tempId);
                });

                var title = new Label(GetPropertyType(element));
                title.AddToClassList("SkillButtonTitle");
                title.styleSheets.Add(_styleSheet);

                effectScroller.Add(title);
                effectScroller.Add(upgradeProperty);
            }
        }

        _itemInfo.Add(effectScroller);

        _backToSkillsButton.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.Flex;
    }

    void CopyEffect(int effectId)
    {
        _currentSelectedSkill.SkillEffects[effectId].effect.ApplyUpgrade(_shownLevel);
    }

    string GetPropertyType(SerializedProperty prop)
    {
        string[] slices = prop.propertyPath.Split('.');
        System.Type type = prop.serializedObject.targetObject.GetType();

        for (int u = 0; u < slices.Length; u++)
            if (slices[u] == "Array")
            {
                u++; //skips "data[x]"
                type = type.GetElementType(); //gets info on array elements
            } else type = type.GetField(slices[u], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;
        return type.ToString();
    }
}
#endif
