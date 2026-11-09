using UnityEngine;

public enum GrowthKind { LevelUp, Evolved }

public readonly struct GrowthEvent
{
    public readonly GrowthKind Kind;
    public readonly string Message;

    public GrowthEvent(GrowthKind kind, string message)
    {
        Kind = kind;
        Message = message;
    }
}