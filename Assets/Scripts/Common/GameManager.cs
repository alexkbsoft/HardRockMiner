using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject EnemyPrefab;
    void Start()
    {
        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(15);

        var enemy = Instantiate(EnemyPrefab, new Vector3(64.01001f, 0.1128006f, -121.83f), Quaternion.identity);
        enemy.GetComponent<NavMeshAgent>().enabled = false;
        enemy.GetComponent<NavMeshAgent>().enabled = true;

        // foreach (GameObject e in Enemies)
        // {
        //     e.SetActive(true);
        //     e.GetComponent<NavMeshAgent>().isStopped = true;
        // }
    }

    void Update()
    {

    }
}
