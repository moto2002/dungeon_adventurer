#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
public class ItemEditor : EditorWindow
{
    static int[] ItemLevels => new int[] { 1, 15, 30, 45, 60};

    BaseItemManager _itemManager;
    VisualElement _root;
    StyleSheet _styleSheet;
    VisualElement _collectionContainer;
    ScrollView _itemContainer;
    VisualElement _itemInfo;
    Label _title;
    Button _backToCollectionButton;
    Button _backToSkillsButton;
    Button _removeSkillButton;
    Sprite _defaultIcon;
    BaseItem _currentSelectedItem;
    BaseItemCollection _currentSelectedCollection;
    SerializedObject SerializedItem;
    SerializedProperty SkillList;
    int ListSize;
    bool[] _possiblePositions;
    bool[] _possibleTargets;

    Editor componentEditor;

    [MenuItem("Window/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void OnEnable()
    {
        _itemManager = AssetDatabase.LoadAssetAtPath<BaseItemManager>("Assets/ScriptableObjects/BaseItems/BaseItemManager.asset");
        _root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ScriptableObjects/Editor/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        _root.Add(labelFromUXML);
        _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ScriptableObjects/Editor/ItemEditor.uss");

        _defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Skill_PlaceHolder.png");

        _collectionContainer = _root.Q("CollectionContainer");
        _title = _root.Q("Title") as Label;

        _backToCollectionButton = _root.Q("BackButton") as Button;
        _backToCollectionButton.clicked += BackToCollection;
        _backToSkillsButton = _root.Q("BackToSkillsButton") as Button;
        _backToSkillsButton.clicked += BackToSkills;

        _collectionContainer.style.display = DisplayStyle.Flex;
        foreach (var collection in _itemManager.Collections)
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
        _itemContainer.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.None;
        _title.text = "ItemEditor";
    }

    void BackToSkills()
    {
        _itemContainer.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.None;
        _backToSkillsButton.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.Flex;
        _removeSkillButton.style.display = DisplayStyle.None;

        SelectCollection(_currentSelectedCollection);
    }

    void SelectCollection(BaseItemCollection selectedCollection)
    {
        _currentSelectedCollection = selectedCollection;
        _collectionContainer.style.display = DisplayStyle.None;

        _itemContainer = _root.Q("SpellsContainer") as ScrollView;
        _itemContainer.Clear();
        _title.text = _currentSelectedCollection.name;

        var scrollViews = new VisualElement[ItemLevels.Length];

        for (var i = 0; i < ItemLevels.Length; i++)
        {
            var scrollView = new VisualElement();
            scrollView.AddToClassList("ScrollItemContainer");
            scrollView.styleSheets.Add(_styleSheet);
            scrollViews[i] = scrollView;

            var levelTitle = new Label("Level " + ItemLevels[i] + "+");
            levelTitle.AddToClassList("ItemLevelTitle");
            levelTitle.styleSheets.Add(_styleSheet);

            _itemContainer.Add(levelTitle);
            _itemContainer.Add(scrollView);
        }

        for (var u = 0; u < ItemLevels.Length; u++)
        {
            foreach (var item in _currentSelectedCollection.Items)
            {
                var button = new Button();
                button.AddToClassList("SkillButton");
                button.styleSheets.Add(_styleSheet);

                var label = new Label(item.name);
                label.AddToClassList("SkillButtonTitle");
                label.styleSheets.Add(_styleSheet);
                button.Add(label);

                var icon = new Image();
                icon.AddToClassList("SkillButtonIcon");
                icon.styleSheets.Add(_styleSheet);
                var itemicon = item.GetIcon(ItemLevels[u]);
                if (itemicon == null)
                {
                    itemicon = _defaultIcon;
                }
                icon.image = itemicon.texture;
                button.Add(icon);

                var field = new ObjectField();
                field.value = item.GetIcon(ItemLevels[u]) ?? _defaultIcon;
                field.objectType = typeof(Sprite);
                var id = u;
                field.RegisterValueChangedCallback(changeEvent =>
                {
                    icon.image = ((Sprite)changeEvent.newValue)?.texture;
                    item.SetIcon(id, (Sprite)changeEvent.newValue);
                });
                button.Add(field);

                scrollViews[u].Add(button);
            }
        }
        _itemContainer.style.display = DisplayStyle.Flex;
        _backToCollectionButton.style.display = DisplayStyle.Flex;
    }

    void SelectItem(BaseItem selectedItem)
    {
        _itemContainer.style.display = DisplayStyle.None;
        _backToCollectionButton.style.display = DisplayStyle.None;
        _removeSkillButton.style.display = DisplayStyle.Flex;

        _itemInfo = _root.Q("ItemInfo");
        _itemInfo.Clear();

        var iconPreview = new Image();
        iconPreview.style.width = 100;
        iconPreview.style.height = 100;
        // iconPreview.style.marginLeft = 10;

        _currentSelectedItem = selectedItem;

        _backToSkillsButton.style.display = DisplayStyle.Flex;
        _itemInfo.style.display = DisplayStyle.Flex;
    }
}
#endif