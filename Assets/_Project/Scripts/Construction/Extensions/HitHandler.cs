using UnityEngine;
using System;

public class HitHandler : MonoBehaviour
{
    [SerializeField] private string tagTarget;

    public event Action<GameObject, bool> hitTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagTarget) hitTarget?.Invoke(collision.gameObject, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == tagTarget) hitTarget?.Invoke(collision.gameObject, false);
    }
}
