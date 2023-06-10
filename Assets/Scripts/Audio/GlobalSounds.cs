using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSounds : MonoBehaviour
{
    [SerializeField] private AudioSource BlockCrushAudio;
    [SerializeField] private AudioSource ResourceCollectedAudio;

    private EventBus _eventBus;
    void Start()
    {
        _eventBus = GameObject.Find("EventBus").GetComponent<EventBus>();
        _eventBus.BlockDamaged.AddListener(OnBlockDamaged);
        _eventBus.ResourceCollected.AddListener(OnResourceCollected);
    }

    private void OnBlockDamaged(ResourceBlock block) {
        if (!BlockCrushAudio.isPlaying) {
            
            BlockCrushAudio.Play();
        }
    }

    private void OnResourceCollected(Resource res) {
        Debug.Log("Collected");
        ResourceCollectedAudio.Play();
    }

    void OnDestroy()
    {
        _eventBus.BlockDamaged.RemoveListener(OnBlockDamaged);
        _eventBus.ResourceCollected.RemoveListener(OnResourceCollected);
    }
}
