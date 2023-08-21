using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UniversalMobileController;

public class MechController : MonoBehaviour
{
    public bool IsActive = true;
    public float CollectDistance = 2.0f;
    [SerializeField] private FloatingJoyStick joystick;
    [SerializeField] private FloatingJoyStick rightJoystick;
    [SerializeField] private GameObject _bodyAim;
    [SerializeField] private AudioSource _stepAudio;


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
    private Damagable _damagable;
    private bool _isDead = false;
    private Vector2 _localPos;

    private static MechController _Instance;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _chController = GetComponent<CharacterController>();
        _damagable = GetComponent<Damagable>();

        _fixedY = transform.position.y;

        _newDir = transform.position + transform.forward;
        _curAimRotation = Quaternion.identity;

        _damagable.OnDamaged.AddListener(OnDamaged);
        _damagable.OnDestroyed.AddListener(OnDead);

        _Instance = this;
    }

    void Start()
    {
        var pos = Constants.LevelOrigin;
        pos.y = transform.position.y;

        transform.position = pos;
    }

    public void SetInitialPlace(float x, float z)
    {
        var pos = Constants.LevelOrigin;
        pos.y = transform.position.y;
        pos.x = x;
        pos.z = z;

        transform.position = pos;
        FollowCamera.Instance.SetPosition(pos);
    }


    void OnDestroy()
    {
        _damagable.OnDamaged.RemoveAllListeners();
        _damagable.OnDestroyed.RemoveAllListeners();
    }


    void FixedUpdate()
    {
        FixYPos();

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

        SetBodyAim();
    }

    public static MechController Instance => _Instance;
    public static Damagable InstanceDamagable => _Instance._damagable;

    public void OnDamaged(float lifeRemained)
    {

    }

    public void OnDead()
    {
        IsActive = false;
        _isDead = true;
        _animator.SetTrigger("dead");
        GetComponentInChildren<Gun>().enabled = false;
        GetComponentInChildren<AnimatedGun>().Fire(false);

        GameObject.Find("MechLights").GetComponent<Light>().enabled = false;
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

        if (_isDead)
        {
            curPos += Vector3.down * 1.5f;
        }

        transform.position = curPos;
    }

    void OnAnimatorMove()
    {
        if (!IsActive)
        {
            return;
        }

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

        if (!_isDead)
        {
            _localPos = new Vector2(horizontal, vertical);
        }
        _newDir = transform.position + transform.forward * 5;

        if (_localPos != Vector2.zero)
        {
            var lookToWorld = DirectionTransform.MoveToSpecifiedDir(_localPos, Camera.main.transform);
            _newDir = transform.position + lookToWorld * 5;

            if (_isDead)
            {
                _newDir += Vector3.down * 2;
            }
        }

        _bodyAim.transform.position = Vector3.Slerp(_bodyAim.transform.position, _newDir, 5 * Time.fixedDeltaTime);
    }

    public void OnStep()
    {
        _stepAudio.Play();
    }
}
