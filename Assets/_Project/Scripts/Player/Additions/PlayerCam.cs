using Unity.Cinemachine;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private IPlayerController _player;

    [SerializeField] private CinemachineCamera vCam;
    private float startLens;

    [SerializeField, Space(5)] private float rate;

    private void Start()
    {
        _player = GetComponentInParent<IPlayerController>();
        startLens = vCam.Lens.OrthographicSize;
    }

    private void Update()
    {
        float speed =  Mathf.Max(Mathf.Abs(_player.FrameInput.x), Mathf.Abs(_player.FrameInput.y));
        vCam.Lens.OrthographicSize = Mathf.MoveTowards(vCam.Lens.OrthographicSize, startLens + speed / 2, Time.deltaTime * rate);
    }
}