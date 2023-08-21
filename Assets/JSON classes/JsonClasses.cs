using System.Collections.Generic;

//original classes moved to C:\Code\Temp Unity Stuff

public class MapObjectJson
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Type { get; set; }
}

public class PickupJson
{
    public int Value { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Type { get; set; }
}

public class PlayerObjectJson
{
    public int Health { get; set; }
    public bool HasRevolver { get; set; }
    public int RevolverAmmo { get; set; }
    public bool HasShotgun { get; set; }
    public int ShotgunAmmo { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Type { get; set; }
}

public class MapJson
{
    public List<MapObjectJson> MapObjects { get; set; }
    public List<MapObjectJson> MapNpcObjects { get; set; }
    public List<PickupJson> Pickups { get; set; }
    public PlayerObjectJson PlayerGameObject { get; set; }
    public string Name { get; set; }
    public bool StoryTextSegment { get; set; }
    public string StoryText { get; set; }
    public int WallTexture { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
}