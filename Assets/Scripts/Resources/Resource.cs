using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public string Name;
    public GameObject[] TypeObjs;
    public float Speed = 2;
    
    private GameObject _player;
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
        _player = GameObject.Find("Player");
        StartCoroutine(StartFollowing());
    }

    private IEnumerator StartFollowing() {
        yield return new WaitForSeconds(1.0f);

        _isFollowing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFollowing) {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Time.deltaTime * Speed);
        }

    }
}
