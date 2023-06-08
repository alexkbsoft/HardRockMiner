using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
    [SerializeField] private MinerState _minerState;
    [SerializeField] private TextMeshProUGUI _lifeText;
    [SerializeField] private GameObject _resourcePrefab;
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private Image _fireImg;
    private EventBus _eventBus;
    private List<GameObject> _resourcePanels = new();

    private bool _fireSelected = false;

    void Start()
    {
        UpdateUI();

        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.ResourceCollected?.AddListener(UpdateRes);

        MechController.InstanceDamagable.OnDamaged.AddListener(OnMechDamaged);
    }

    public void FirePressed() {
        _fireSelected = !_fireSelected;
        _eventBus.FireEnabled?.Invoke(_fireSelected);
        UpdateUI();
    }

    private void UpdateRes(Resource _)
    {
        foreach (GameObject panel in _resourcePanels)
        {
            Destroy(panel);
        }
        _resourcePanels.Clear();

        int pos = -300;
        foreach (MinerState.StoredResource res in _minerState.resources)
        {
            var newPanel = Instantiate(_resourcePrefab, _infoPanel.transform);

            var resPanel = newPanel.GetComponent<ResourcePanel>();
            resPanel.UpdateUI(res.name, res.count, pos);
            pos -= 303;
            _resourcePanels.Add(newPanel);
        }
    }

    private void OnMechDamaged(float _) {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _lifeText.text = $"{MechController.InstanceDamagable.CurrentLife}";
        UpdateRes(null);

        Color c = _fireSelected ? Color.red : Color.white;

        _fireImg.color = c;
    }
    void OnDestroy()
    {
        _eventBus.ResourceCollected?.RemoveListener(UpdateRes);
        MechController.InstanceDamagable.OnDamaged?.RemoveListener(OnMechDamaged);
    }
}
