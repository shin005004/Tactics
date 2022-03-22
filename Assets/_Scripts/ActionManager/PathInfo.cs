using UnityEngine;

public struct PathInfo
{
    public Vector3 point;
    public PlayerAction action;
    public Vector3 viewDir;
    
    public PathInfo(Vector3 _point, PlayerAction _action = PlayerAction.Idle, Vector3 _viewDir = new Vector3())
    {
        point = _point;
        action = _action;
        viewDir = _viewDir;
    }
}

public enum PlayerAction
{
    Idle,
    Look,
    Chase,
    Attack
}