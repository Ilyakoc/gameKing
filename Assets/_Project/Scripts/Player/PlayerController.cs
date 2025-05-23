using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public static PlayerController Instance { get; private set; }
    public enum TargetFace { Top, Right, Down, Left }

    [SerializeField] private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    public Vector2 FrameVelocity => _frameVelocity;
    private bool _cachedQueryStartInColliders;

    [Header("States")]
    public TargetFace targetFace;
    public bool disableMove;

    #region Interface

    public event Action<bool> ReferedChanged;
    public event Action<bool> GroundedChanged;
    public event Action FellOff;
    public event Action Jumped;
    public Vector2 FrameInput => new Vector2(_frameInput.Horizontal, _frameInput.Vertical);

    #endregion

    private float _time;

    private void Awake()
    {
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        _time = 100;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            ReferDown = Input.GetKeyDown(KeyCode.F),
            JumpDown = Input.GetKeyDown(KeyCode.Space),
            JumpHeld = Input.GetKey(KeyCode.Space),
            Vertical = Input.GetAxisRaw("Vertical"),
            Horizontal = Input.GetAxisRaw("Horizontal")
        };

        if (_stats.SnapInput)
        {
            _frameInput.Vertical = Mathf.Abs(_frameInput.Vertical) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Vertical);
            _frameInput.Horizontal = Mathf.Abs(_frameInput.Horizontal) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Horizontal);
        }
        if (disableMove)
        {
            _frameInput.ReferDown = false;
            _frameInput.JumpDown = false;
            _frameInput.JumpHeld = false;
            _frameInput.Vertical = 0;
            _frameInput.Horizontal = 0;
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        if (_frameInput.ReferDown)
        {
            _referToConsume = true;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();

        HandleJump();
        HandleDirection();

        HandleRefer();

        ApplyMovement();
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private MovingPlatform followPlatform;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = true;

        Vector2 scale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        RaycastHit2D groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size * scale, _col.direction, 0, Vector2.down, _stats.GroundDistance, _stats.GroundLayer);
        if (groundHit && groundHit.collider.tag == "Platform") followPlatform = groundHit.transform.GetComponent<MovingPlatform>();
        else followPlatform = null;

        if (_grounded && !groundHit)
        {
            SetGround(false);
        }
        else if (!_grounded && groundHit && !jumpOccursX && !jumpOccursY)
        {
            SetGround(true);
        }

        if (!groundHit && !CanUseCoyote && !(jumpOccursX || jumpOccursY))
        {
            FellOffPlatform();
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    private void SetGround(bool ground)
    {
        if (ground)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(_grounded);
        }
        else
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(_grounded);
        }
    }

    private void FellOffPlatform()
    {
        FellOff?.Invoke();
        transform.position = new Vector2(0, 0);
    }

    #endregion

    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;
    private bool jumpOccursX;
    private bool jumpOccursY;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_frameInput.JumpHeld && Mathf.Max(Mathf.Abs(_rb.linearVelocity.x), Mathf.Abs(_rb.linearVelocity.y)) > 0)
            _endedJumpEarly = true;
        if (jumpOccursX && Mathf.Abs(_frameVelocity.x) <= 0) jumpOccursX = false;
        if (jumpOccursY && Mathf.Abs(_frameVelocity.y) <= 0) jumpOccursY = false;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote && !jumpOccursX && !jumpOccursY) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        int dirs = 0;
        if (targetFace == TargetFace.Right || _frameVelocity.x > 0 && dirs < 2)
        {
            _frameVelocity.x = _frameVelocity.x / 1.65f + _stats.JumpPower;
            jumpOccursX = true;
            dirs++;
        }
        if (targetFace == TargetFace.Left || _frameVelocity.x < 0 && dirs < 2)
        {
            _frameVelocity.x = _frameVelocity.x / 1.65f + -_stats.JumpPower;
            jumpOccursX = true;
            dirs++;
        }
        if (targetFace == TargetFace.Top || _frameVelocity.y > 0 && dirs < 2)
        {
            _frameVelocity.y = _frameVelocity.y / 1.65f + _stats.JumpPower;
            jumpOccursY = true;
            dirs++;
        }
        if (targetFace == TargetFace.Down || _frameVelocity.y < 0 && dirs < 2)
        {
            _frameVelocity.y = _frameVelocity.y / 1.65f + -_stats.JumpPower;
            jumpOccursY = true;
            dirs++;
        }
        if (dirs == 2)
        {
            _frameVelocity *= _stats.CrossJumpModifier;
        }
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        SetGround(false);
        Jumped?.Invoke();
    }

    #endregion

    #region Direction

    private void HandleDirection()
    {
        bool jumpOccurs = jumpOccursX || jumpOccursY;
        float deceleration = !jumpOccurs ? _stats.Deceleration : _stats.JumpDeceleration;
        float acceleration = _stats.Acceleration;
        if (_endedJumpEarly) deceleration *= _stats.JumpEndEarlyGravityModifier;

        if (_frameInput.Vertical == 0 || jumpOccursY)
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, 0, deceleration * Time.fixedDeltaTime);
        else
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, _frameInput.Vertical * _stats.MaxSpeed, acceleration * Time.fixedDeltaTime);
        if (_frameInput.Horizontal == 0 || jumpOccursX)
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        else
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Horizontal * _stats.MaxSpeed, acceleration * Time.fixedDeltaTime);

        if (_frameInput.Horizontal > 0 && targetFace != TargetFace.Right) Flip(TargetFace.Right);
        else if (_frameInput.Horizontal < 0 && targetFace != TargetFace.Left) Flip(TargetFace.Left);
        else if (_frameInput.Vertical > 0 && targetFace != TargetFace.Top) Flip(TargetFace.Top);
        else if (_frameInput.Vertical < 0 && targetFace != TargetFace.Down) Flip(TargetFace.Down);
    }

    private void Flip(TargetFace face)
    {
        targetFace = face;
        if (face == TargetFace.Right) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        else if (face == TargetFace.Left) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
    }

    #endregion

    #region Reference

    private bool _referToConsume;
    private bool _inRefer;
    private Stuff referStuff;
    private Stuff.CargoWeight cargoModifier;

    private void HandleRefer()
    {
        Physics2D.queriesStartInColliders = true;
        Vector2 scale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        if (_inRefer) scale = Vector2.zero;
        RaycastHit2D stuffHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size * scale, _col.direction, 0, Vector2.right * Mathf.Sign(transform.localScale.x), _stats.ReferDistance, _stats.StuffLayer);
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;

        if (!_referToConsume) return;

        if (stuffHit || _inRefer) ExecuteRefer(stuffHit.transform?.GetComponent<Stuff>());

        _referToConsume = false;
    }

    private void ExecuteRefer(Stuff hit)
    {
        if (!hit) hit = referStuff;
        _inRefer = hit.SwitchBindStuff(transform, out cargoModifier, out referStuff);
        ReferedChanged?.Invoke(_inRefer);
    }

    #endregion

    private void ApplyMovement()
    {
        float cargo;
        switch (cargoModifier)
        {
            case Stuff.CargoWeight.Low:
                cargo = _stats.StuffWeightModifier.x;
                break;
            case Stuff.CargoWeight.Average:
                cargo = _stats.StuffWeightModifier.y;
                break;
            case Stuff.CargoWeight.High:
                cargo = _stats.StuffWeightModifier.z;
                break;
            default:
                cargo = 1;
                break;
        }

        _rb.linearVelocity = _frameVelocity;
        if (followPlatform) _rb.linearVelocity += followPlatform.velocity;
        if (_inRefer) _rb.linearVelocity *= cargo;
    }
}

public struct FrameInput
{
    public bool ReferDown;
    public bool JumpDown;
    public bool JumpHeld;
    public float Vertical;
    public float Horizontal;
}

public interface IPlayerController
{
    public event Action<bool> ReferedChanged;

    public event Action<bool> GroundedChanged;
    public event Action FellOff;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
}