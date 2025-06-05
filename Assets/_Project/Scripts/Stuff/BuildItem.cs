using UnityEngine;

public class BuildItem : Stuff
{
    [SerializeField, Space(5), Header("Build Item")]
    private string token;
    public string Token => token;
}