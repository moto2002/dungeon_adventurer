using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class EncounterResultController : UIBehaviour, IPointerClickHandler
{
    public struct EncounterResult
    {
        public bool wasSucceded;
        public string endText;
    }

    [SerializeField] Button continueButton;
    [SerializeField] Button closeButton;

    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] TextMeshProUGUI descText;

    Action _closeCallback;

    protected override void Awake()
    {
        continueButton.onClick.AddListener(CloseResult);
        closeButton.onClick.AddListener(CloseResult);
    }

    public void ShowResult(EncounterResult result, Action closeCallback)
    {
        _closeCallback = closeCallback;

        headerText.text = result.wasSucceded ? "Success!" : "Failed ...";
        descText.text = result.endText;

        gameObject.SetActive(true);
    }

    void CloseResult()
    {
        gameObject.SetActive(false);
        _closeCallback?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CloseResult();
    }
}
