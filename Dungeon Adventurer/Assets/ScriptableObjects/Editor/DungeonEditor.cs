#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class DungeonEditor : EditorWindow
{
    DungeonCollection _dungeonCollection;
    VisualElement _root;
    StyleSheet _styleSheet;
    VisualElement _collectionContainer;
    VisualElement _spellContainer;
    VisualElement _itemInfo;
    Label _title;
    Button _backToCollectionButton;
    Button _backToSkillsButton;
    Button _removeSkillButton;
    Sprite _defaultIcon;
    SerializedObject SerializedSkill;
    SerializedProperty SkillList;
    int ListSize;
    bool[] _possiblePositions;
    bool[] _possibleTargets;

    Monster _newAddedBossMonster;

    Editor componentEditor;

    [MenuItem("Window/DungeonEditor")]
    public static void ShowExample()
    {
        var wnd = GetWindow<DungeonEditor>();
        wnd.titleContent = new GUIContent("DungeonEditor");
    }

    public void OnEnable()
    {
        _dungeonCollection = AssetDatabase.LoadAssetAtPath<DungeonCollection>("Assets/ScriptableObjects/Dungeon/DungeonCollection.asset");
        _root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ScriptableObjects/Editor/DungeonEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        _root.Add(labelFromUXML);
        _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ScriptableObjects/Editor/DungeonEditor.uss");

        _defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Skill_PlaceHolder.png");

        _title = _root.Q("Title") as Label;

        RefreshCollection();
    }

    void RefreshCollection()
    {
        _spellContainer = _root.Q("SpellsContainer");
        _spellContainer.Clear();
        _title.text = _dungeonCollection.name;

        var dungeons = _dungeonCollection.Dungeons.OrderBy(dungeon => dungeon.minLvl).ToArray();

        foreach (var dungeon in dungeons)
        {
            var typeLabel = new Label($"Level {dungeon.minLvl} - {dungeon.maxLvl}");
            typeLabel.AddToClassList("MonsterTypeTitle");
            typeLabel.styleSheets.Add(_styleSheet);
            _spellContainer.Add(typeLabel);

            var content = new VisualElement();
            content.AddToClassList("Container");
            content.styleSheets.Add(_styleSheet);
            content.contentContainer.style.flexDirection = FlexDirection.Column;

            var topContent = new VisualElement();
            topContent.style.flexDirection = FlexDirection.Row;
            topContent.style.marginTop = 10;
            var botContent = new VisualElement();
            botContent.style.flexDirection = FlexDirection.Row;

            var topRightContent = new VisualElement();
            topRightContent.style.minWidth = 150;
            var topLeftContent = new VisualElement();
            topLeftContent.style.minWidth = 150;

            var botRightContent = new VisualElement();
            botRightContent.style.flexDirection = FlexDirection.Column;

            var botLeftContent = new VisualElement();
            botLeftContent.style.minWidth = 150;
            botLeftContent.style.alignItems = Align.Center;
            botLeftContent.style.justifyContent = Justify.Center;
            botContent.style.height = 150;

            var nameTextField = new TextField("Name");
            nameTextField.value = dungeon.displayName;
            nameTextField.labelElement.style.minWidth = 60;
            nameTextField.RegisterValueChangedCallback(changeEvent =>
            {
                dungeon.displayName = changeEvent.newValue;
                EditorUtility.SetDirty(dungeon);
            });

            var minLevelField = new IntegerField("Min Level");
            minLevelField.labelElement.style.minWidth = 80;
            minLevelField.value = dungeon.minLvl;
            minLevelField.RegisterValueChangedCallback(changeEvent => { dungeon.minLvl = changeEvent.newValue; });

            var maxLevelField = new IntegerField("Max Level");
            maxLevelField.labelElement.style.minWidth = 80;
            maxLevelField.value = dungeon.maxLvl;
            maxLevelField.RegisterValueChangedCallback(changeEvent => { dungeon.maxLvl = changeEvent.newValue; });

            var bossLabel = new Label("Boss");
            bossLabel.AddToClassList("MonsterTypeTitle");
            bossLabel.styleSheets.Add(_styleSheet);
            botLeftContent.Add(bossLabel);

            var button = new Button(() => ShowBossPicker(dungeon));
            button.style.width = 90;

            var label = new Label(dungeon.boss != null ? dungeon.boss.displayName : string.Empty);
            label.AddToClassList("SkillButtonTitle");
            label.styleSheets.Add(_styleSheet);
            button.Add(label);

            var icon = new Image();
            icon.AddToClassList("SkillButtonIcon");
            icon.styleSheets.Add(_styleSheet);
            icon.image = dungeon.boss != null ? dungeon.boss.icon.texture : _defaultIcon.texture;
            button.Add(icon);

            var monsterTitleLabel = new Label("Possible Monsters");
            monsterTitleLabel.AddToClassList("MonsterTypeTitle");
            monsterTitleLabel.styleSheets.Add(_styleSheet);
            botRightContent.Add(monsterTitleLabel);

            var posMonsterContent = new ScrollView();
            posMonsterContent.style.flexDirection = FlexDirection.Row;
            posMonsterContent.contentContainer.style.flexDirection = FlexDirection.Row;
            posMonsterContent.contentViewport.style.flexDirection = FlexDirection.Row;

            if (dungeon.possibleMonsters != null && dungeon.possibleMonsters.Length != 0)
            {
                foreach (var monster in dungeon.possibleMonsters)
                {
                    var monsterButton = new Button(() =>
                    {
                        dungeon.RemovePosMonster(monster);
                        RefreshCollection();
                    });
                    monsterButton.style.width = 90;

                    var monsterLabel = new Label(monster.displayName);
                    monsterLabel.AddToClassList("SkillButtonTitle");
                    monsterLabel.styleSheets.Add(_styleSheet);
                    monsterButton.Add(monsterLabel);

                    var monsterIcon = new Image();
                    monsterIcon.AddToClassList("SkillButtonIcon");
                    monsterIcon.styleSheets.Add(_styleSheet);
                    monsterIcon.image = monster.icon.texture;
                    monsterButton.Add(monsterIcon);

                    posMonsterContent.Add(monsterButton);
                }
            }

            var newMonsterButton = new Button(() => ShowMonsterPicker(dungeon));
            newMonsterButton.AddToClassList("SkillButton");
            newMonsterButton.styleSheets.Add(_styleSheet);

            var newMonsterLabel = new Label("Add Skill");
            newMonsterLabel.AddToClassList("SkillButtonTitle");
            newMonsterLabel.styleSheets.Add(_styleSheet);
            newMonsterButton.Add(newMonsterLabel);
            posMonsterContent.Add(newMonsterButton);
            botRightContent.Add(posMonsterContent);

            topLeftContent.Add(nameTextField);
            topRightContent.Add(minLevelField);
            topRightContent.Add(maxLevelField);

            topContent.Add(topLeftContent);
            topContent.Add(topRightContent);

            botLeftContent.Add(button);

            botContent.Add(botLeftContent);
            botContent.Add(botRightContent);

            content.Add(topContent);
            content.Add(botContent);


            _spellContainer.Add(content);
        }

        var addContent = new ScrollView();
        addContent.AddToClassList("Container");
        addContent.styleSheets.Add(_styleSheet);
        addContent.contentViewport.style.flexDirection = FlexDirection.Row;
        addContent.contentContainer.style.flexDirection = FlexDirection.Row;

        var newButton = new Button();
        newButton.AddToClassList("MonsterButton");
        newButton.styleSheets.Add(_styleSheet);

        var newLabel = new Label("Add New Dungeon");
        newLabel.AddToClassList("SkillButtonTitle");
        newLabel.styleSheets.Add(_styleSheet);
        newButton.Add(newLabel);

        var newButtonicon = new Image();
        newButtonicon.AddToClassList("SkillButtonIcon");
        newButtonicon.styleSheets.Add(_styleSheet);
        newButtonicon.tintColor = Color.white;
        newButton.Add(newButtonicon);
        addContent.Add(newButton);
        _spellContainer.Add(addContent);


        _spellContainer.style.display = DisplayStyle.Flex;
    }

    void BackToSkills()
    {
        _spellContainer.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.None;
        _backToSkillsButton.style.display = DisplayStyle.None;
        _removeSkillButton.style.display = DisplayStyle.None;

        RefreshCollection();
    }

    void AddNewDungeon(string dungeonName)
    {
        var newDungeon = ScriptableObject.CreateInstance<Dungeon>();
        var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_dungeonCollection));

        newDungeon.id = _dungeonCollection.Dungeons.Max(skill => skill.id) + 1;
        newDungeon.name = dungeonName;
        newDungeon.displayName = dungeonName;

        AssetDatabase.CreateAsset(newDungeon, assetPath + "/Monsters/Monster" + newDungeon.id + ".asset");

        _dungeonCollection.AddDungeon(newDungeon);
    }

    void RemoveDungeon(Dungeon removedDungeon)
    {
        _dungeonCollection.RemoveDungeon(removedDungeon);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(removedDungeon));
        BackToSkills();
        RefreshCollection();
    }

    bool listening = false;
    bool pickingBoss = false;
    Dungeon _currentSelectedDungeon;

    void ShowBossPicker(Dungeon selectedDungeon)
    {
        listening = true;
        pickingBoss = true;
        _currentSelectedDungeon = selectedDungeon;
        EditorGUIUtility.ShowObjectPicker<Monster>(null, false, "l:MonsterBoss", 0);
    }

    void ShowMonsterPicker(Dungeon selectedDungeon)
    {
        listening = true;
        _currentSelectedDungeon = selectedDungeon;
        EditorGUIUtility.ShowObjectPicker<Monster>(null, false, "l:Monster", 0);
    }

    void AddNewBoss()
    {
        _currentSelectedDungeon.boss = _newAddedBossMonster;
        RefreshCollection();
    }

    void AddNewMonster()
    {
        _currentSelectedDungeon.AddPosMonster(_newAddedBossMonster);
        RefreshCollection();
    }

    void OnGUI()
    {
        if (listening && Event.current.type == EventType.ExecuteCommand)
        {
            Event e = Event.current;
            if (e.commandName == "ObjectSelectorClosed")
            {
                listening = false;
                var source = EditorGUIUtility.GetObjectPickerObject();
                if (source == null || typeof(Monster) != source.GetType()) return;
                _newAddedBossMonster = (Monster)source;
                if (pickingBoss)
                {
                    AddNewBoss();
                }
                else
                {
                    AddNewMonster();
                }
                pickingBoss = false;
            }
        }
    }
}
#endif