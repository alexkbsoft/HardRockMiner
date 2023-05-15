using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceModeCameraConrtoller : MonoBehaviour
{
    [SerializeField] private float zoom=30;
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x,zoom,player.transform.position.z);

        if (Input.mouseScrollDelta.magnitude > 0)
        {
            zoom += -Input.mouseScrollDelta.y;
        }
    }
}
