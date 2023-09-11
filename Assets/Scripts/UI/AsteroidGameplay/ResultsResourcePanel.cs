using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsResourcePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _image;

    public void UpdateUI(Sprite sprite, float count, int yPos) {
        _countText.text = $"{count}";
        _rect.anchoredPosition = new Vector2(0, yPos);
        _image.sprite = sprite;
    }   
}
