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
    BaseOfTreeWithSmallBushes = 1 << 1,
    DeadTreeTrunk = 1 << 2,
    TreeStump = 1 << 3,
    Bush = 1 << 4,
    MuddyBank = 1 << 5,
    SubmergedLand = 1 << 6,
    BaseOfGiantTree = 1 << 7,
    MagicTreeStump = 1 << 8,
    MagicDeadTreeTrunk = 1 << 9,
    Rock = 1 << 10,
    GrassyPatch = 1 << 11,
    ExposedRoots = 1 << 12,
}
