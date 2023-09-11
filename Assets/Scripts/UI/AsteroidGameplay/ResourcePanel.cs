using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _image;



    public void UpdateUI(Sprite sprite, float count, int xPos) {
        _countText.text = $"{count}";
        _rect.anchoredPosition = new Vector2(xPos, 0);
        _image.sprite = sprite;

        // transform.Find(resourceId).gameObject.SetActive(true);
    }   
}
