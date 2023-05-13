using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [SerializeField] private float orbitalSpeed;
    private Orbitals orbit;
    private PlayerSpaceController player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerSpaceController>();
        orbit = GetComponent<Orbitals>();
        orbitalSpeed = player.orbitalSpeedMultiplier/orbit.xradius;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, orbitalSpeed * Time.deltaTime, 0,Space.World);
    }
}
