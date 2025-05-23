using UnityEngine;

public class Stuff : MonoBehaviour
{
    [SerializeField] private CargoWeight cargoWeight;
    public enum CargoWeight { Null, Low, Average, High };
    [SerializeField] private Vector2 offset;

    [SerializeField, Space(5)] private Material selected;
    private Material def;

    private bool referTarget;

    [SerializeField, Space(5)] private LayerMask groundLayer;
    [SerializeField] private float groundDistance;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private MovingPlatform followPlatform;
    private bool _cachedQueryStartInColliders;

    private void Start()
    {
        def = GetComponent<SpriteRenderer>().material;

        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        ApplyMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && !referTarget) GetComponent<SpriteRenderer>().material = selected;
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && !referTarget) GetComponent<SpriteRenderer>().material = def;
    }

    public bool SwitchBindStuff(Transform par, out CargoWeight modifier, out Stuff refer)
    {
        referTarget = !referTarget;
        if (referTarget)
        {
            transform.parent = par;
            transform.localPosition = offset;
            refer = this;
            modifier = cargoWeight;
            _col.enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().material = def;
            transform.parent = null;
            modifier = CargoWeight.Null;
            refer = null;
            _col.enabled = true;
        }
        return referTarget;
    }

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = true;

        Vector2 scale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        RaycastHit2D groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size * scale, _col.direction, 0, Vector2.down, groundDistance, groundLayer);
        if (groundHit && groundHit.collider.tag == "Platform") followPlatform = groundHit.transform.GetComponent<MovingPlatform>();
        else followPlatform = null;

        if (!groundHit)
        {
            FellOffPlatform();
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    protected virtual void FellOffPlatform()
    {
        if (referTarget) return;
        transform.position = new Vector2(0, 0);
    }

    private void ApplyMovement()
    {
        _rb.linearVelocity = new Vector2(0, 0);
        if (followPlatform && !referTarget) _rb.linearVelocity += followPlatform.velocity;
    }
}