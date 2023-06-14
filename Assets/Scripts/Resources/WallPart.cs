using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPart : MonoBehaviour
{
    public float DiveSpeed = 1.0f;
    public float LifeTime = 1.5f;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.position += Vector3.down * Time.deltaTime * DiveSpeed;
    }

    public void Activate()
    {
        var forceDir = Random.onUnitSphere;
        forceDir.y = 0;

        _rb.isKinematic = false;
        _rb.AddForce(forceDir * Random.Range(5f, 10.0f), ForceMode.Impulse);

        StartCoroutine(DelayedActivate());
    }

    private IEnumerator DelayedActivate() {
        yield return new WaitForSeconds(LifeTime);

        gameObject.layer = LayerMask.NameToLayer("DestroyedCube");
        Destroy(_rb);
        
        this.enabled = true;

        Destroy(gameObject, LifeTime);
    }
}
