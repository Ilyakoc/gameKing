using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    [Header("INPUT")]
    public bool SnapInput = true;
    public float VerticalDeadZoneThreshold = 0.1f;
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("MOVEMENT")]
    public float MaxSpeed = 14;
    public float Acceleration = 120;
    public float Deceleration = 60;
}