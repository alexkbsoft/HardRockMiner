using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadControl : MonoBehaviour
{
    private float _direction = 1;
    private float _speed = 1;
    private float _scale = 1;
    void Start()
    {
        StartCoroutine(ChangeDir());
    }

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * _scale * _direction * 10.0f / _speed);
    }

    private IEnumerator ChangeDir() {
        while(true) {
            var range = Random.Range(0.5f, 2.0f);
            _scale = 1;
            _speed = range;
            _direction *= -1;
            yield return new WaitForSeconds(range);
            _scale = 0;
            yield return new WaitForSeconds(Random.Range(1.5f, 3.0f));
        }
    }
}
