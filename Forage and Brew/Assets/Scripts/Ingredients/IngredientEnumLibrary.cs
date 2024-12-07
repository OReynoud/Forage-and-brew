public enum IngredientType
{
    Mushroom = 0,
    Herb = 1,
    Moss = 2,
    Berry = 3
}

public enum Biome
{
    None = 0,
    Forest = 1 << 0,
    Swamp = 1 << 1
}

public enum SpawnLocation
{
    None = 0,
    FlatLand = 1 << 0,
    BaseOfTree = 1 << 1,
    TreeTrunk = 1 << 2,
    DeadTreeTrunk = 1 << 3,
    Bush = 1 << 4,
    RiverBank = 1 << 5,
    RiverBed = 1 << 6,
    Pond = 1 << 7,
    Mud = 1 << 8,
    Rock = 1 << 9,
    RockWall = 1 << 10,
    Cliff = 1 << 11
}
