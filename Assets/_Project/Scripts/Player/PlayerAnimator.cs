using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private IPlayerController _player;

    [SerializeField, Space(5)] private float _maxTilt = 5;
    [SerializeField] private float _tiltSpeed = 20;

    private void Awake()
    {
        _player = GetComponentInParent<IPlayerController>();
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
        _player.FellOff += OnFelled;
        _player.ReferedChanged += OnReferedChanged;
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;
        _player.FellOff -= OnFelled;
        _player.ReferedChanged -= OnReferedChanged;
    }

    private void Update()
    {
        HandleMovement();
        HandleCharacterTilt();
    }

    private void HandleMovement()
    {
        float input = Mathf.Max(Mathf.Abs(_player.FrameInput.x), Mathf.Abs(_player.FrameInput.y));
        _anim.SetBool(MoveKey, input > 0);
    }

    private void HandleCharacterTilt()
    {
        float input = Mathf.Max(Mathf.Abs(_player.FrameInput.x), Mathf.Abs(_player.FrameInput.y));
        float rot = _maxTilt * input;
        Quaternion runningTilt = Quaternion.Euler(0, 0, rot);
        _anim.transform.up = Vector3.RotateTowards(_anim.transform.up, runningTilt * Vector2.up, _tiltSpeed * Time.deltaTime, 0f);
    }

    private void OnJumped()
    {
        _anim.SetTrigger(JumpKey);
        _anim.ResetTrigger(GroundedKey);
    }

    private void OnGroundedChanged(bool grounded)
    {
        if (grounded) _anim.SetTrigger(GroundedKey);
    }

    private void OnFelled()
    {
        _anim.SetTrigger(FellKey);
    }

    private void OnReferedChanged(bool refer)
    {
        if (refer) _anim.SetTrigger(ReferKey);
        else _anim.SetTrigger(UnreferKey);
    }

    private static readonly int MoveKey = Animator.StringToHash("Move");
    private static readonly int JumpKey = Animator.StringToHash("Jump");
    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int FellKey = Animator.StringToHash("Fell");
    private static readonly int ReferKey = Animator.StringToHash("Refer");
    private static readonly int UnreferKey = Animator.StringToHash("Unrefer");
}