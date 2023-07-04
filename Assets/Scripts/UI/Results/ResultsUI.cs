using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private MainStorage _mainStorage;
    [SerializeField] private MinerState _minerState;
    [SerializeField] private GameObject _resourcePanelPrefab;
    void Start()
    {
        SaveResults();
        
        int pos = -400;
        foreach (MinerState.StoredResource res in _minerState.resources)
        {
            var newPanel = Instantiate(_resourcePanelPrefab, transform);

            var resPanel = newPanel.GetComponent<ResultsResourcePanel>();
            resPanel.UpdateUI(res.name, res.count, pos);
            pos -= 105;
        }
        
        pos = -700;
        foreach (MinerState.StoredResource res in _mainStorage.resources)
        {
            var newPanel = Instantiate(_resourcePanelPrefab, transform);

            var resPanel = newPanel.GetComponent<ResultsResourcePanel>();
            resPanel.UpdateUI(res.name, res.count, pos);
            pos -= 105;
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
}
