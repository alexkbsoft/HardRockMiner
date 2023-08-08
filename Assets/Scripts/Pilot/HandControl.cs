using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour
{
    private float _direction = 1;
    private float _axis = 1;
    private float _speed = 1;
    private float _scale = 1;
    void Start()
    {
        StartCoroutine(ChangeDir());
    }

    void Update()
    {
        if (_axis == 1)
        {
            transform.Rotate(0, 0, Time.deltaTime * _scale * _direction * 20.0f / _speed);
        }
        else if (_axis == 2)
        {
            transform.Rotate(0, Time.deltaTime * _scale * _direction * 20.0f / _speed, 0);
        }
        else if (_axis == 3)
        {
            transform.Rotate(Time.deltaTime * _scale * _direction * 20.0f / _speed, 0, 0);
        }
    }

    private IEnumerator ChangeDir()
    {
        while (true)
        {
            var range = Random.Range(0.5f, 2.0f);
            _scale = 1;
            _speed = range;
            _direction *= -1;
            _axis = Random.Range(1, 3);
            yield return new WaitForSeconds(1);
            _scale = 0;
            yield return new WaitForSeconds(Random.Range(3.5f, 5.0f));
        }
    }
}
