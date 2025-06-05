using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector2[] points;
    [SerializeField] private float speed;

    private int goalPoint;
    private Vector2 startPos;
    public Vector2 velocity = Vector2.zero;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position = Vector2.SmoothDamp(transform.position, startPos + points[goalPoint], ref velocity, speed);
        if (Vector2.Distance(transform.position, startPos + points[goalPoint]) < 0.06f)
        {
            if (goalPoint < points.Length - 1) goalPoint++;
            else goalPoint = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (points.Length == 0) return;
        Gizmos.color = Color.red;

        foreach (Vector2 point in points)
        {
            if (!Application.isPlaying)
                Gizmos.DrawSphere((Vector2)transform.position + point, 0.35f);
            else
                Gizmos.DrawSphere(startPos + point, 0.35f);
        }
    }
}
