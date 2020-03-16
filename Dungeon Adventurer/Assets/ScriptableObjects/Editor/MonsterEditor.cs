#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MonsterEditor : EditorWindow
{
    MonsterCollection _monsterCollection;
    VisualElement _root;
    StyleSheet _styleSheet;
    VisualElement _collectionContainer;
    VisualElement _spellContainer;
    VisualElement _itemInfo;
    Label _title;
    Button _backToCollectionButton;
    Button _backToSkillsButton;
    Button _removeSkillButton;
    Monster _currentSelectedMonster;
    Sprite _defaultIcon;
    SerializedObject SerializedSkill;
    SerializedProperty SkillList;
    int ListSize;
    bool[] _possiblePositions;
    bool[] _possibleTargets;

    Skill _newAddedSkill;

    Editor componentEditor;

    [MenuItem("Window/MonsterEditor")]
    public static void ShowExample()
    {
        var wnd = GetWindow<MonsterEditor>();
        wnd.titleContent = new GUIContent("MonsterEditor");
    }

    public void OnEnable()
    {
        _monsterCollection = AssetDatabase.LoadAssetAtPath<MonsterCollection>("Assets/ScriptableObjects/Monster/MonsterCollection.asset");
        _root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ScriptableObjects/Editor/MonsterEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        _root.Add(labelFromUXML);
        _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ScriptableObjects/Editor/MonsterEditor.uss");

        _defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Skill_PlaceHolder.png");

        _title = _root.Q("Title") as Label;

        _removeSkillButton = _root.Q("RemoveSkillButton") as Button;
        _removeSkillButton.clicked += RemoveSkill;

        _backToSkillsButton = _root.Q("BackToSkillsButton") as Button;
        _backToSkillsButton.clicked += BackToSkills;

        RefreshCollection();
    }

    void RefreshCollection()
    {
        _spellContainer = _root.Q("SpellsContainer");
        _spellContainer.Clear();
        _title.text = _monsterCollection.name;

        foreach (var type in Enum.GetValues(typeof(MonsterType)))
        {
            var ty = (MonsterType)type;
            var monsters = _monsterCollection.Monsters.Where(monster => monster.type == ty).OrderByDescending(monster => monster.isBoss).ToArray();

            if (monsters.Length == 0) continue;

            var typeLabel = new Label(ty.ToString());
            typeLabel.AddToClassList("MonsterTypeTitle");
            typeLabel.styleSheets.Add(_styleSheet);
            _spellContainer.Add(typeLabel);

            var content = new ScrollView();
            content.AddToClassList("Container");
            content.styleSheets.Add(_styleSheet);
            content.contentViewport.style.flexDirection = FlexDirection.Row;
            content.contentContainer.style.flexDirection = FlexDirection.Row;

            foreach (var monster in monsters)
            {
                var button = new Button(() => SelectMonster(monster));
                button.AddToClassList(monster.isBoss ? "BossButton" : "MonsterButton");
                button.styleSheets.Add(_styleSheet);

                var label = new Label(monster.displayName);
                label.AddToClassList("SkillButtonTitle");
                label.styleSheets.Add(_styleSheet);
                button.Add(label);

                var icon = new Image();
                icon.AddToClassList("SkillButtonIcon");
                icon.styleSheets.Add(_styleSheet);
                icon.image = monster.icon.texture;
                button.Add(icon);

                content.Add(button);
            }

            var newButton = new Button(() => AddNewMonster(ty));
            newButton.AddToClassList("MonsterButton");
            newButton.styleSheets.Add(_styleSheet);

            var newLabel = new Label("Add " + ty.ToString());
            newLabel.AddToClassList("SkillButtonTitle");
            newLabel.styleSheets.Add(_styleSheet);
            newButton.Add(newLabel);

            var newButtonicon = new Image();
            newButtonicon.AddToClassList("SkillButtonIcon");
            newButtonicon.styleSheets.Add(_styleSheet);
            newButtonicon.tintColor = Color.white;
            newButton.Add(newButtonicon);
            content.Add(newButton);
            _spellContainer.Add(content);
        }

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

    void AddNewMonster(MonsterType startType)
    {
        var newMonster = ScriptableObject.CreateInstance<Monster>();
        var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_monsterCollection));

        newMonster.icon = _defaultIcon;
        newMonster.id = _monsterCollection.Monsters.Max(skill => skill.id) + 1;
        newMonster.type = startType;

        AssetDatabase.CreateAsset(newMonster, assetPath + "/Monsters/Monster" + newMonster.id + ".asset");
        AssetDatabase.SetLabels(newMonster, new string[] { "Monster" });

        _monsterCollection.AddMonster(newMonster);

        SelectMonster(newMonster);
    }

    void RemoveSkill()
    {
        _monsterCollection.RemoveMonster(_currentSelectedMonster);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_currentSelectedMonster));
        BackToSkills();
        RefreshCollection();
    }

    void SelectMonster(Monster selectedMonster)
    {
        _currentSelectedMonster = selectedMonster;
        _spellContainer.style.display = DisplayStyle.None;
        _removeSkillButton.style.display = DisplayStyle.Flex;

        _itemInfo = _root.Q("ItemInfo");
        _itemInfo.Clear();

        var iconPreview = new Image();
        iconPreview.style.width = 100;
        iconPreview.style.height = 100;
        // iconPreview.style.marginLeft = 10;
        iconPreview.image = _currentSelectedMonster.icon.texture;

        var icon = new ObjectField();
        icon.value = _currentSelectedMonster.icon;
        icon.objectType = typeof(Sprite);
        icon.RegisterValueChangedCallback(changeEvent =>
        {
            iconPreview.image = ((Sprite)changeEvent.newValue).texture;
            _currentSelectedMonster.icon = (Sprite)changeEvent.newValue;
        });

        var nameTextField = new TextField("Name");
        nameTextField.value = _currentSelectedMonster.displayName;
        nameTextField.labelElement.style.minWidth = 60;
        nameTextField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.displayName = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });

        var nameChangeButton = new Button(() =>
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_currentSelectedMonster), _currentSelectedMonster.displayName);
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        nameChangeButton.text = "Change GO Name";
        nameChangeButton.style.minWidth = 60;

        var experienceField = new IntegerField("Exp/Level");
        experienceField.labelElement.style.minWidth = 60;
        experienceField.value = _currentSelectedMonster.experienceGain;
        experienceField.RegisterValueChangedCallback(changeEvent => { _currentSelectedMonster.experienceGain = changeEvent.newValue; });

        var bossToggle = new Toggle("Is Boss?");
        bossToggle.value = _currentSelectedMonster.isBoss;
        bossToggle.labelElement.style.minWidth = 60;
        bossToggle.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.isBoss = changeEvent.newValue;
            AssetDatabase.SetLabels(_currentSelectedMonster, _currentSelectedMonster.isBoss ? new string[] { "MonsterBoss" } : null);
        });

        var typeField = new EnumField("Monster Type");
        typeField.Init(_currentSelectedMonster.type);
        typeField.labelElement.style.minWidth = 90;
        typeField.style.minWidth = 200;
        typeField.RegisterValueChangedCallback(changeEvent => { _currentSelectedMonster.type = (MonsterType)changeEvent.newValue; });

        var styleField = new EnumField("Fighting Style");
        styleField.Init(_currentSelectedMonster.fightingStyle);
        styleField.labelElement.style.minWidth = 90;
        styleField.style.minWidth = 200;
        styleField.RegisterValueChangedCallback(changeEvent => { _currentSelectedMonster.fightingStyle = (FightingStyle)changeEvent.newValue; });

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
        rightContainer.Add(bossToggle);
        rightContainer.Add(experienceField);
        rightContainer.Add(typeField);
        rightContainer.Add(styleField);

        upperContainer.Add(leftContainer);
        upperContainer.Add(rightContainer);
        upperContainer.style.flexDirection = FlexDirection.Row;

        var leftBotContainer = new VisualElement();
        leftBotContainer.style.width = 200;

        var strField = new IntegerField("Strength");
        strField.labelElement.style.minWidth = 80;
        strField.value = _currentSelectedMonster.coreMain.str;
        strField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreMain.str = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        leftBotContainer.Add(strField);

        var conField = new IntegerField("Constitution");
        conField.labelElement.style.minWidth = 80;
        conField.value = _currentSelectedMonster.coreMain.con;
        conField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreMain.con = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        leftBotContainer.Add(conField);

        var dexField = new IntegerField("Dexterity");
        dexField.labelElement.style.minWidth = 80;
        dexField.value = _currentSelectedMonster.coreMain.dex;
        dexField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreMain.dex = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        leftBotContainer.Add(dexField);

        var intField = new IntegerField("Intelligence");
        intField.labelElement.style.minWidth = 80;
        intField.value = _currentSelectedMonster.coreMain.intel;
        intField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreMain.intel = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        leftBotContainer.Add(intField);

        var lckField = new IntegerField("Luck");
        lckField.labelElement.style.minWidth = 80;
        lckField.value = _currentSelectedMonster.coreMain.lck;
        lckField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreMain.lck = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        leftBotContainer.Add(lckField);

        var rightBotContainer = new VisualElement();
        rightBotContainer.style.width = 200;

        var hpField = new IntegerField("Health");
        hpField.labelElement.style.minWidth = 120;
        hpField.value = _currentSelectedMonster.coreSub.hp;
        hpField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.hp = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(hpField);

        var armorField = new IntegerField("Armor");
        armorField.labelElement.style.minWidth = 120;
        armorField.value = _currentSelectedMonster.coreSub.armor;
        armorField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.armor = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(armorField);

        var dodgeField = new IntegerField("Dodge");
        dodgeField.labelElement.style.minWidth = 120;
        dodgeField.value = _currentSelectedMonster.coreSub.dodge;
        dodgeField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.dodge = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(dodgeField);

        var speedField = new IntegerField("Speed");
        speedField.labelElement.style.minWidth = 120;
        speedField.value = _currentSelectedMonster.coreSub.speed;
        speedField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.speed = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(speedField);

        var physDmgField = new IntegerField("Physical Damage");
        physDmgField.labelElement.style.minWidth = 120;
        physDmgField.value = _currentSelectedMonster.coreSub.physDmg;
        physDmgField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.physDmg = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(physDmgField);

        var physCritField = new IntegerField("Phys Crit Damage");
        physCritField.labelElement.style.minWidth = 120;
        physCritField.value = _currentSelectedMonster.coreSub.critDmg;
        physCritField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.critDmg = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(physCritField);

        var magicDmgField = new IntegerField("Magic Damage");
        magicDmgField.labelElement.style.minWidth = 120;
        magicDmgField.value = _currentSelectedMonster.coreSub.magicDmg;
        magicDmgField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.magicDmg = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(magicDmgField);

        var magicCritField = new IntegerField("Magic Crit Damage");
        magicCritField.labelElement.style.minWidth = 120;
        magicCritField.value = _currentSelectedMonster.coreSub.critMagic;
        magicCritField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.critMagic = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(magicCritField);

        var critChanceField = new IntegerField("Critical Chance");
        critChanceField.labelElement.style.minWidth = 120;
        critChanceField.value = _currentSelectedMonster.coreSub.critChance;
        critChanceField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.critChance = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(critChanceField);

        var fireResField = new IntegerField("Fire Resistance");
        fireResField.labelElement.style.minWidth = 120;
        fireResField.value = _currentSelectedMonster.coreSub.fireRes;
        fireResField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.fireRes = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(fireResField);

        var iceResField = new IntegerField("Ice Resistance");
        iceResField.labelElement.style.minWidth = 120;
        iceResField.value = _currentSelectedMonster.coreSub.iceRes;
        iceResField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.iceRes = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(iceResField);

        var lightResField = new IntegerField("Lightning Resistance");
        lightResField.labelElement.style.minWidth = 120;
        lightResField.value = _currentSelectedMonster.coreSub.lightRes;
        lightResField.RegisterValueChangedCallback(changeEvent =>
        {
            _currentSelectedMonster.coreSub.lightRes = changeEvent.newValue;
            EditorUtility.SetDirty(_currentSelectedMonster);
        });
        rightBotContainer.Add(lightResField);

        var middleContainer = new VisualElement();
        middleContainer.Add(leftBotContainer);
        middleContainer.Add(rightBotContainer);
        middleContainer.style.flexDirection = FlexDirection.Row;
        middleContainer.style.marginLeft = 35;

        infoContainer.Add(upperContainer);

        var dividerLabel = new Label("Monster Stats");
        dividerLabel.AddToClassList("MonsterTypeTitle");
        dividerLabel.styleSheets.Add(_styleSheet);
        dividerLabel.style.marginTop = 25;
        infoContainer.Add(dividerLabel);

        infoContainer.Add(middleContainer);

        var skillsLabel = new Label("Skills");
        skillsLabel.AddToClassList("MonsterTypeTitle");
        skillsLabel.styleSheets.Add(_styleSheet);
        skillsLabel.style.marginTop = 25;
        infoContainer.Add(skillsLabel);

        var botContainer = new VisualElement();
        botContainer.style.flexDirection = FlexDirection.Row;
        botContainer.style.flexWrap = Wrap.Wrap;
        botContainer.style.marginLeft = 35;

        if (_currentSelectedMonster.coreSkills != null && _currentSelectedMonster.coreSkills.Length != 0)
        {
            foreach (var skill in _currentSelectedMonster.coreSkills)
            {
                var button = new Button();
                button.AddToClassList("SkillButton");
                button.styleSheets.Add(_styleSheet);

                var skillLabel = new Label(skill.displayName);
                skillLabel.AddToClassList("SkillButtonTitle");
                skillLabel.styleSheets.Add(_styleSheet);
                button.Add(skillLabel);

                var skillIcon = new Image();
                skillIcon.AddToClassList("SkillButtonIcon");
                skillIcon.styleSheets.Add(_styleSheet);
                skillIcon.image = skill.icon.texture;
                button.Add(skillIcon);

                botContainer.Add(button);
            }
        }

        var newButton = new Button(() => ShowSkillPicker());
        newButton.AddToClassList("SkillButton");
        newButton.styleSheets.Add(_styleSheet);

        var newLabel = new Label("Add Skill");
        newLabel.AddToClassList("SkillButtonTitle");
        newLabel.styleSheets.Add(_styleSheet);
        newButton.Add(newLabel);

        botContainer.Add(newButton);
        infoContainer.Add(botContainer);

        _itemInfo.Add(infoContainer);

        _backToSkillsButton.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.Flex;
    }

    bool listening = false;

    void ShowSkillPicker()
    {
        listening = true;
        EditorGUIUtility.ShowObjectPicker<Skill>(null, false, "l:MonsterSkill", 0);
    }

    void AddNewSkill()
    {
        _currentSelectedMonster.AddSkill(_newAddedSkill);
        SelectMonster(_currentSelectedMonster);
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
                if (source == null || typeof(Skill) != source.GetType()) return;
                _newAddedSkill = (Skill)source;
                AddNewSkill();
            }
        }
    }
}
#endif