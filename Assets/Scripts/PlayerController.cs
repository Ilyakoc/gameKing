using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speedRun;
    public float speedJump;
    public float largeResSpeed;
    public bool FaceRight = true;
    public bool jump = false;

    public Transform legs;
    public LayerMask groundLayer;

    public bool hold;
    public float distance = 2f;
    RaycastHit2D hit;
    public Transform holdPoint;
    public float throwObject = 5;

    public Rigidbody2D rb;
    private Vector2 moveVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speedRun;

        if (Input.GetAxis("Horizontal") > 0)
        {
            FaceRight = true;

            Quaternion rot = transform.rotation;
            rot.y = 0;
            transform.rotation = rot;
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            FaceRight = false;

            Quaternion rot = transform.rotation;
            rot.y = 180;
            transform.rotation = rot;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(IsGrounded());
            //Vector2 movement = new Vector2(rb.velosity.x, speedJump);
            //rb.linearVelocity = movement;
        }

        if (!IsGrounded() && !jump) {
            ReloadScene();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!hold)
            {
                Physics2D.queriesStartInColliders = false;
                hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance);

                if (hit.collider != null && hit.collider.tag == "ResourceType_1")
                {
                    hold = true;
                    speedRun /= largeResSpeed;
                    speedJump /= largeResSpeed;
                }
                else if (hit.collider != null && hit.collider.tag == "ResourceType_2")
                {
                    hold = true;
                }
            }
            else
            {
                //TODO
                hold = false;
                speedRun = 2;
                speedJump = 2;
                //������

                //if (hit.collider.gameObject.GetComponent<RigidBody2D>() != null)
                //{
                //    hit.collider.gameObject.GetComponent<RigidBody2D>().velocity = new Vector2(transform.localScale.x, 1) * throwObject;
                //}

            }
        }

        if (hold)
        {
            hit.collider.gameObject.transform.position = holdPoint.position;

            if (holdPoint.position.x > transform.position.x && hold == true)
            {
                hit.collider.gameObject.transform.localScale = new Vector2(transform.localScale.x * 2, transform.localScale.y * 2);
            }
            else if (holdPoint.position.x < transform.position.x && hold == true)
            {
                hit.collider.gameObject.transform.localScale = new Vector2(transform.localScale.x * -2, transform.localScale.y * 2);
            }
        }
    }


    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * speedRun * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * transform.localScale.x * distance);
    }

    public bool IsGrounded()
    {
        Collider2D checkGround = Physics2D.OverlapCircle(legs.position, 0.5f, groundLayer);

        if (checkGround != null)
        {
            return true;
        }

        return false;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}