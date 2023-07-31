using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraTabModes : MonoBehaviour
{
    private EventBus _eventBus;
    private InventoryTabBar _mainTabs;
    private void Start()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.InventoryTabSelected.AddListener(OnTabSelected);
        
        _mainTabs = GameObject.FindObjectOfType<InventoryTabBar>();

    }

    private void OnTabSelected(int index)
    {
        var moveToPoint = _mainTabs._cameraPoints[index];
        var rotateToPoint = _mainTabs._cameraRotations[index];
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DORotate(rotateToPoint, 0.5f));
        mySequence.Insert(0, transform.DOMove(moveToPoint, 0.5f));
    }

    private void OnDestroy()
    {
        _eventBus.InventoryTabSelected.AddListener(OnTabSelected);
    }
}
