using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Storage;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private MainStorage _mainStorage;
    [SerializeField] private MinerState _minerState;
    [SerializeField] private GameObject _resourcePanelPrefab;
    [SerializeField] private GameObject _failText;

    void Start()
    {
        if (_minerState.IsDead) {
            _failText.SetActive(true);
            return;
        }

        SaveResults();

        int pos = -400;
        foreach (MinerState.StoredResource res in _minerState.resources)
        {
            var newPanel = Instantiate(_resourcePanelPrefab, transform);

            var resPanel = newPanel.GetComponent<ResultsResourcePanel>();
            resPanel.UpdateUI(_mainStorage.ResSprites[res.name], res.count, pos);
            pos -= 105;
        }

        var allIcons = Resources.LoadAll<Sprite>("Dropables");

        foreach (var oneSprite in allIcons)
        {
            var rgx = new Regex("-icon");
            var name = rgx.Replace(oneSprite.name, "");

            _mainStorage.ResSprites[name] = oneSprite;
        }
    }

    private void SaveResults()
    {
        var dataManager = new DataManager();

        foreach (MinerState.StoredResource res in _minerState.resources)
        {
            _mainStorage.Add(res);
        }

        dataManager.SaveMainStorage(_mainStorage);
    }

    public void OnGoToBase()
    {
        SceneManager.LoadScene("Base");
    }
}
