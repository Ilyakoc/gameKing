using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private IPlayerController _player;

    private void Awake()
    {
        _player = GetComponentInParent<IPlayerController>();
    }

    private void OnEnable()
    {
        // Подписка на функции, активирующие trigger переменные
    }

    private void OnDisable()
    {
        // Отписка
    }

    private void Update()
    {
        // Постоянное изменение boolean переменных
    }

    // private static readonly int DefaultKey = Animator.StringToHash("Default");
}