using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsResourcePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private RectTransform _rect;

    public void UpdateUI(string resourceId, float count, int yPos) {
        _countText.text = $"{count}";
        _rect.anchoredPosition = new Vector2(0, yPos);

        transform.Find(resourceId).gameObject.SetActive(true);
    }   
}
