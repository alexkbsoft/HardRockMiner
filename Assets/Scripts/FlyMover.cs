using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMover : MonoBehaviour
{
    private Touch theTouch;
    private Vector2 touchStartPos, touchEndPos;
    Vector2 result = Vector2.zero;

    public float Speed = 3.0f;

    void Start()
    {
    }

    void Update()
    {
        var vecto2Move = VirtualPadInput();
        var move = new Vector3(vecto2Move.x, 0, vecto2Move.y);
        bool isMoving = move != Vector3.zero;

        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * 2);
            transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        }
    }

    private Vector2 VirtualPadInput()
    {
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == TouchPhase.Began)
            {
                touchStartPos = theTouch.position;
            }
            else if (theTouch.phase == TouchPhase.Moved)
            {
                touchEndPos = theTouch.position;

                result.Set(touchEndPos.x - touchStartPos.x, touchEndPos.y - touchStartPos.y);
            } else if (theTouch.phase == TouchPhase.Ended) {
                result = Vector2.zero;
            }
        }

        return result;
    }
}
