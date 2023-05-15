using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalMobileController;

public class MechController : MonoBehaviour
{
    public bool IsActive = true;
    [SerializeField] private FloatingJoyStick joystick;


    private Animator _animator;
    private Touch theTouch;
    private Vector2 touchStartPos, touchEndPos;
    private string direction;
    private Vector2 result = Vector2.zero;
    private CharacterController _chController;
    private Vector3 _move;
    private float _fixedY;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _chController = GetComponent<CharacterController>();
        _fixedY = transform.position.y;
    }


    void FixedUpdate()
    {
        if (!IsActive)
        {
            _animator.SetBool("move", false);

            return;
        }

        var _move = MovementFromCameraPoint();

        bool isMoving = _move != Vector3.zero;

        _animator.SetBool("move", isMoving);

        if (_move != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_move), Time.deltaTime * 10);
        }

        FixYPos();
    }

    private Vector3 MovementFromCameraPoint() {
        var vector2Move = LeftJoystickInput();

        var cam = Camera.main.transform;
        var forward = cam.forward;
        forward.y = 0;

        return cam.right * vector2Move.x  + forward * vector2Move.y;
    }

    private void FixYPos()
    {
        var curPos = transform.position;
        curPos.y = _fixedY;
        transform.position = curPos;
    }

    void OnAnimatorMove()
    {
        _chController.Move(_animator.deltaPosition + Vector3.down * Time.fixedDeltaTime);
        FixYPos();
    }

    private Vector2 LeftJoystickInput()
    {
        var horizontal = joystick.GetHorizontalValue();
        var vertical = joystick.GetVerticalValue();
        

        return new Vector2(horizontal, vertical);
    }
}
