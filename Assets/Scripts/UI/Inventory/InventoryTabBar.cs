using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTabBar : MonoBehaviour
{
    [SerializeField] private GameObject[] inactiveBtns;
    [SerializeField] private GameObject[] selectedBtns;

    public Vector3[] _cameraPoints;
    public Vector3[] _cameraRotations;

    private EventBus _eventBus;

    private void Start()
    {
        _eventBus = EventBus.Instance;
    }

    public void SelectTab(int index)
    {
        void DeactivateTabs (GameObject oneBtn) => oneBtn.SetActive(false);
        
        Array.ForEach(inactiveBtns, DeactivateTabs);
        Array.ForEach(selectedBtns, DeactivateTabs);

        for (int i = 0; i < inactiveBtns.Length; i++)
        {
            inactiveBtns[i].SetActive(i != index);
        }

        for (int j = 0; j < selectedBtns.Length; j++)
        {
            selectedBtns[j].SetActive(j == index);
        }
        
        _eventBus.InventoryTabSelected?.Invoke(index);
    }
}
