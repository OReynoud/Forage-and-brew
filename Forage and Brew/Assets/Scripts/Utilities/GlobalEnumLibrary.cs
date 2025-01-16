public enum Scene
{
    House,
    Outdoor,
    Biome1,
    Biome2
}

public enum TimeOfDay
{
    Daytime,
    Nighttime
}

public enum Temperature
{
    None,
    LowHeat,
    MediumHeat,
    HighHeat
}

public enum TutorialTriggerConditions
{
    ZoneTrigger = 1 << 0,
    TimeOfTheDay = 1 << 1,
    ObtainCommand = 1 << 2,
    ObtainIngredient = 1 << 3,
    ObtainPotion = 1 << 4,
    IsCarryingObject = 1 << 5
}

public enum LetterType
{
    Orders,
    Thanks,
    ShippingError
}

public enum JoystickDirection
{
    Up,
    Down,
    Left,
    Right,
    Clockwise,
    CounterClockwise,
    ArcOfACircleDownCounterClockwise
}
