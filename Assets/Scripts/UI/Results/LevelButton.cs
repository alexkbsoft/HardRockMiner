using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleBox;

    public void SetTitle(string title)
    {
        titleBox.text = title;
    }

    
}
