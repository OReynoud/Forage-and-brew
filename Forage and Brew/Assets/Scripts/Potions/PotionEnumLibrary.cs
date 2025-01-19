using System;

public enum PotionTag
{
    None = 0,
    FullHeal = 1 << 3,
    Breakfast = 1 << 7,
    FullEffects = 1 << 4,
    Colds = 1 << 0,
    Illness = 1 << 1,
    ShallowCuts = 1 << 2,
    Beverage = 1 << 5,
    EnergyDrink = 1 << 6,
    ContainCatnip = 1 << 8,
    
}