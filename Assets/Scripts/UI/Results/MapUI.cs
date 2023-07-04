using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [SerializeField] private MinerState _minerState;
    [SerializeField] private GameObject LevelButtonPrefab;

    void Start()
    {
        var dataManager = new DataManager();

        int x = 230;

        foreach (string fName in dataManager.AsteroidNames())
        {
            var levelBtn = Instantiate(LevelButtonPrefab, transform, true);
            levelBtn.GetComponent<LevelButton>().SetTitle(fName);
            levelBtn.GetComponent<Button>().onClick.AddListener(() => LoadLevel(fName));
            levelBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
            
            x += 210;
        }
    }
    
    public void LoadLevel(string levelName)
    {
        _minerState.AsteroidName = levelName;

        SceneManager.LoadScene("AsteroidInternals");
    }
}
