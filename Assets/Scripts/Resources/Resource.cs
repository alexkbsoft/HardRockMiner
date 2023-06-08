using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public string Name;
    public GameObject[] TypeObjs;
    public float Speed = 2;

    private MechController _player;
    private bool _isFollowing = false;

    public void SetType(string name)
    {
        this.Name = name;

        switch (Name)
        {
            case "green":
                TypeObjs[0].SetActive(true);

                break;
            case "blue":
                TypeObjs[1].SetActive(true);

                break;
            case "yello":
                TypeObjs[2].SetActive(true);

                break;
            case "red":
                TypeObjs[3].SetActive(true);

                break;
            default:
                break;
        }
    }

    void Start()
    {
        _player = MechController.Instance;
        StartCoroutine(StartFollowing());
    }

    private IEnumerator StartFollowing()
    {
        yield return new WaitForSeconds(2.0f);

        _isFollowing = true;
    }

    void Update()
    {
        var curPos = transform.position;
        curPos.y = 0;
        var playerPos= _player.transform.position;
        playerPos.y = 0;

        if (_isFollowing && Vector3.Distance(curPos, playerPos) <= _player.CollectDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _player.transform.position,
                Time.deltaTime * Speed);
        }
    }
}
