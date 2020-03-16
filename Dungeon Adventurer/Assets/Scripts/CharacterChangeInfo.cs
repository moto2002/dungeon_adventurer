using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterChangeInfo : MonoBehaviour
{
    [SerializeField] Image statusImage;
    [SerializeField] TextMeshProUGUI turnCounter;

    int _turnCount;
    public int TurnCount => _turnCount;

    public void SetData(Sprite image, int turnCount)
    {
        _turnCount = turnCount;
        statusImage.sprite = image;
        turnCounter.text = _turnCount + "";
    }
}

