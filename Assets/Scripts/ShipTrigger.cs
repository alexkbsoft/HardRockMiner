using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // FindObjectOfType<MechController>().IsActive = false;
            // var flyer = FindObjectOfType<FlyMover>();
            // flyer.enabled = true;
            // var cam = FindObjectOfType<FollowCamera>();
            // cam.player = flyer.gameObject;
            // cam.SetDistance(4.0f);
        }
    }
}
