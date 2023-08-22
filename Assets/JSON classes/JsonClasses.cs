using System.Collections.Generic;
using System;
//original classes moved to C:\Code\Temp Unity Stuff

[System.Serializable]
public class MapObjectJson
{
    public int X;
    public int Y;
    public string Type;
}

[System.Serializable]
public class PickupJson
{
    public int Value;
    public int X;
    public int Y;
    public string Type;
}

[System.Serializable]
public class PlayerObjectJson
{
    public int Health;
    public bool HasRevolver;
    public int RevolverAmmo;
    public bool HasShotgun;
    public int ShotgunAmmo;
    public int X;
    public int Y;
    public string Type;
}

[System.Serializable]
public class MapNpcObjectJson
{
    public int Health;
    public int ProjectileDamage;
    public float FiringRate;
    public float PatrolRange;
    public float AttackRange;
    public float ChaseRange;
    public bool CanMove;
    public int X;
    public int Y;
    public string Type;
}

public class MapJson
{
    public MapObjectJson[] MapObjects;
    public MapNpcObjectJson[] MapNpcObjects;
    public PickupJson[] Pickups;
    public PlayerObjectJson PlayerGameObject;
    public string Name;
    public bool StoryTextSegment;    
    public string StoryText;
    public int WallTexture;
    public int StartX;
    public int StartY;
}
