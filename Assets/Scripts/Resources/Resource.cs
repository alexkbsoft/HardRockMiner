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
    private Rigidbody _rb;
    private Transform _internalGeometry;

    void Start()
    {
        _player = MechController.Instance;
        _rb = GetComponent<Rigidbody>();

        StartCoroutine(StartFollowing());
    }

    
    public void SetType(string name)
    {
        this.Name = name;
        _internalGeometry = transform.Find(this.Name);
        _internalGeometry.gameObject.SetActive(true);
    }

    private IEnumerator StartFollowing()
    {
        yield return new WaitForSeconds(2.0f);

        _isFollowing = true;
        gameObject.layer = LayerMask.NameToLayer("ResourceFollowing");
        _internalGeometry.gameObject.layer = LayerMask.NameToLayer("ResourceFollowing");
    }

    void Update()
    {
        var curPos = transform.position;
        curPos.y = 0;
        var playerPos= _player.transform.position;
        playerPos.y = 0;

        if (_isFollowing && Vector3.Distance(curPos, playerPos) <= _player.CollectDistance)
        {
            // Vector3 speed = (_player.transform.position - transform.position).normalized * Speed;

            // // _rb.MovePosition(transform.position + speed * Time.deltaTime);

            // _rb.AddForce

            transform.position = Vector3.MoveTowards(
                transform.position,
                _player.transform.position + Vector3.up,
                Time.deltaTime * Speed);
        }
    }
}
