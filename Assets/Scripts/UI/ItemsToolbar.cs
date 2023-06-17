using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemsToolbar : MonoBehaviour
{
    [SerializeField] private RectTransform MovingContainer;
    [SerializeField] private CanvasGroup PanelTransparentGroup;

    private EventBus _eventBus;
    private bool visible = false;
    private bool animating = false;
    void Start()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
    }

    public void SpawnItem(string id)
    {
        if (animating) {
            return;
        }

        RevealOrHide();
        _eventBus?.ItemSpawn.Invoke(id);
    }

    public void RevealOrHide()
    {
        if (animating)
        {
            return;
        }

        animating = true;
        float toAnimate = visible ? 300 : -290;
        float toAlpha =  visible ? 0 : 1;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(MovingContainer.DOAnchorPosX(toAnimate, 0.3f, true));
        mySequence.Insert(0, PanelTransparentGroup.DOFade(toAlpha, 0.3f));
        mySequence.OnComplete(() =>
                {
                    animating = false;
                    visible = !visible;
                });
    }
}
