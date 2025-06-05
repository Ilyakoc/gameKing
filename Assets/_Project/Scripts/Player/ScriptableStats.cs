using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    [Header("LAYERS")]
    public LayerMask GroundLayer;
    public LayerMask StuffLayer;
    public float ReferDistance = 0.1f;
    public Vector3 StuffWeightModifier = new Vector3(0.8f, 0.6f, 0.35f);

    [Header("INPUT")]
    public bool SnapInput = true;
    public float VerticalDeadZoneThreshold = 0.1f;
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("MOVEMENT")]
    public float MaxSpeed = 14;
    public float Acceleration = 120;
    public float Deceleration = 60;
    public float GroundDistance = 0.05f;

    [Header("JUMP")]
    public float JumpPower = 36;
    public float CrossJumpModifier = 0.7f;
    public float JumpDeceleration = 85;
    public float JumpEndEarlyGravityModifier = 3;
    public float CoyoteTime = .15f;
    public float JumpBuffer = .2f;
}