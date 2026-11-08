using UnityEngine;

public enum GrowthType { LevelUp, Evolved }

public readonly struct GrowthEvent
{
    public readonly GrowthType Type;
    public readonly MonsterData From;   
    public readonly MonsterData To;     
    public readonly string Message;

    public GrowthEvent(GrowthType type, MonsterData from, MonsterData to, string message)
    {
        Type = type; From = from; To = to; Message = message;
    }
}