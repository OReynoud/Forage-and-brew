using System;

public enum IngredientType
{
    Mushroom,
    Herb,
    Moss,
    Berry
}

[Flags]
public enum Biome
{
    Forest = 1 << 0
}

[Flags]
public enum SpawnLocation
{
    FlatLand = 1 << 0,
    BaseOfTree = 1 << 1,
    TreeTrunk = 1 << 2,
    DeadTreeTrunk = 1 << 3,
    Brush = 1 << 4,
    RiverBank = 1 << 5,
    RiverBed = 1 << 6,
    Pond = 1 << 7,
    Mud = 1 << 8,
    Rock = 1 << 9,
    RockWall = 1 << 10,
    Cliff = 1 << 11
}
