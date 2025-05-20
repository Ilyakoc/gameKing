using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;

    [Header("States")]
    public bool facingRight;
    public bool disableMove;

    #region Interface

    public float Vertical => _frameInput.Vertical;
    public float Horizontal => _frameInput.Horizontal;

    #endregion

    private void Awake()
    {
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GatherInput();
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
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
            _frameInput.Vertical = 0;
            _frameInput.Horizontal = 0;
        }
    }

    private void FixedUpdate()
    {
        HandleVertical();
        HandleHorizontal();

        ApplyMovement();
    }

    #region Vertical

    private void HandleVertical()
    {
        if (_frameInput.Vertical == 0)
        {
            float deceleration = _stats.Deceleration;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, _frameInput.Vertical * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Horizontal

    private void HandleHorizontal()
    {
        if (_frameInput.Horizontal == 0)
        {
            float deceleration = _stats.Deceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Horizontal * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }

        if (_frameInput.Horizontal > 0 && !facingRight) Flip();
        else if (_frameInput.Horizontal < 0 && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    #endregion

    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;
}

public struct FrameInput
{
    public float Vertical;
    public float Horizontal;
}

public interface IPlayerController
{
    public float Vertical { get; }
    public float Horizontal { get; }
}