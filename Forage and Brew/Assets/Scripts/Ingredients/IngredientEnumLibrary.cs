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
    OnTreeTrunk = 1 << 2,
    DeadTreeTrunk = 1 << 3,
    Stump = 1 << 4,
    Bush = 1 << 5,
    RiverBank = 1 << 6,
    RiverBed = 1 << 7,
    Pond = 1 << 8,
    ImmergedLand = 1 << 9,
    BaseOfBigTree = 1 << 10,
    MagicTreeStump = 1 << 11,
    MagicDeadTrunk = 1 << 12,
    SpikyBush = 1 << 13,
    RockBig = 1 << 14,
    RockMedium = 1 << 15,
    RockSmall = 1 << 16,
   
    
}
