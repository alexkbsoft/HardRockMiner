using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalMobileController;

public class MechController : MonoBehaviour
{
    public bool IsActive = true;
    [SerializeField] private FloatingJoyStick joystick;
    [SerializeField] private FloatingJoyStick rightJoystick;
    [SerializeField] private GameObject _bodyAim;


    private Animator _animator;
    private Touch theTouch;
    private Vector2 touchStartPos, touchEndPos;
    private string direction;
    private Vector2 result = Vector2.zero;
    private CharacterController _chController;
    private Vector3 _move;
    private Vector3 _newDir;
    private Quaternion _curAimRotation;
    private float _fixedY;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _chController = GetComponent<CharacterController>();
        _fixedY = transform.position.y;

        _newDir = transform.position + transform.forward;
        _curAimRotation = Quaternion.identity;
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
        SetBodyAim();
    }

    private Vector3 MovementFromCameraPoint()
    {
        var vector2Move = LeftJoystickInput();

        var cam = Camera.main.transform;
        return DirectionTransform.MoveToSpecifiedDir(vector2Move, cam);
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

    private void SetBodyAim()
    {
        var horizontal = rightJoystick.GetHorizontalValue();
        var vertical = rightJoystick.GetVerticalValue();

        var _localPos = new Vector2(horizontal, vertical);
        _newDir = transform.position + transform.forward * 5;

        if (_localPos != Vector2.zero)
        {
            var lookToWorld = DirectionTransform.MoveToSpecifiedDir(_localPos, Camera.main.transform);
            _newDir = transform.position + lookToWorld * 5;
        }

        _bodyAim.transform.position = Vector3.Slerp(_bodyAim.transform.position, _newDir, 0.1f);
    }
}
