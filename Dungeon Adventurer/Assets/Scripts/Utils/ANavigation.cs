using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public abstract class ANavigation<T> : UIBehaviour where T : ANavigationEntry
{
    class NavigationEntry
    {
        public T entry;
        public Button button;
        public bool isActive = true;
        public bool isSelected = false;

        public NavigationEntry(T data, Button newButton)
        {
            entry = data;
            button = newButton;
        }

        public void Select()
        {
            isSelected = true;
            button.enabled = false;
            entry.Select();
        }

        public void Deselect()
        {
            isSelected = false;
            button.enabled = true;
            entry.Deselect();
        }
    }

    [SerializeField] Transform container;
    [SerializeField] Button prefab;

    readonly List<NavigationEntry> tabs = new List<NavigationEntry>();

    public void SetData(List<T> data, T selectedEntry)
    {
        tabs.Clear();
        foreach (var entry in data)
        {
            var button = Instantiate(prefab, container);
            var navEntry = new NavigationEntry(entry, button);

            button.onClick.AddListener(() => Select(navEntry));
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.Title;
            tabs.Add(navEntry);

            if (entry == selectedEntry) Select(navEntry);
        }

        if (tabs.All(tabs => !tabs.isSelected))
        {
            throw new System.Exception("Navigation instantiated without selecting an entry!");
        }
    }

    public void Select(T selectedEntry)
    {
        Select(tabs.FirstOrDefault(entry => entry.entry == selectedEntry));
    }

    void Select(NavigationEntry selectedEntry)
    {
        tabs.Where(tab => tab.isSelected).ToList().ForEach(tab => tab.Deselect());
        selectedEntry.Select();
    }
}

public abstract class ANavigationEntry : UIBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] string title;

    public string Title => title;

    public virtual void Select()
    {
        content.SetActive(true);
    }

    public virtual void Deselect()
    {
        content.SetActive(false);
    }
}
