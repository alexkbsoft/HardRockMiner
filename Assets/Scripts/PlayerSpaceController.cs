using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject pointer;
    [SerializeField] private Vector3 target;
    [SerializeField] private float shipSpeed;
    [SerializeField] private float fuel;
    [SerializeField] private float fuelBurn;
    [SerializeField] private Orbitals fuelRing;
    [SerializeField] private float orbitalSpeed;
    [SerializeField] public float orbitalSpeedMultiplier=160f;

    // Start is called before the first frame update
    void Start()
    {
        player.transform.LookAt(pointer.transform.position);
        DrawFuel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                target = hit.point;
                target.y = 0;
                pointer.transform.position = target;
            }
        }

        if (Vector3.Distance(transform.position, pointer.transform.position) > 0.01)
        {
            player.transform.LookAt(pointer.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, pointer.transform.position, Time.deltaTime*shipSpeed);
            fuel -= Time.deltaTime * shipSpeed* fuelBurn;
            DrawFuel();
        }

        orbitalSpeed = orbitalSpeedMultiplier/ Vector3.Distance(transform.position, Vector3.zero);
        transform.RotateAround(Vector3.zero, Vector3.up, orbitalSpeed * Time.deltaTime);
        pointer.transform.RotateAround(Vector3.zero, Vector3.up, orbitalSpeed * Time.deltaTime);
    }

    private void DrawFuel() {
        fuelRing.xradius = fuel;
        fuelRing.yradius = fuel;
        fuelRing.CreatePoints();
    }
}
