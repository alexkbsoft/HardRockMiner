using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public static FollowCamera Instance => _instance;

    public GameObject player;
    public float cameraDist = 1;

    [SerializeField] private float speed = 2f;

    private Vector3 playerPosition;

    [SerializeField] private Vector3 _offset;

    private static FollowCamera _instance;

    void Awake() {
        _instance = this;
    }
    void Start()
    {
        RememberOffset();
    }

    public void SetPosition(Vector3 pos) {
        transform.position = pos + _offset * cameraDist;
    }

    public void RememberOffset()
    {
        _offset = transform.position - player.transform.position;        
    }

    public void SetDistance(float dist)
    {
        cameraDist = dist;
    }

    private void Update()
    {
        if (player)
        {
            playerPosition = player.transform.position + _offset * cameraDist;
            Vector3 currentPosition = Vector3.Lerp(transform.position, playerPosition, speed * Time.deltaTime);
            transform.position = currentPosition;
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Equals))
        {
            if (Input.GetKeyDown(KeyCode.Equals)) cameraDist -= 0.1f;
            if (Input.GetKeyDown(KeyCode.Minus)) cameraDist += 0.1f;
            cameraDist = Math.Clamp(cameraDist, 1, 4);
        }
    }
}
