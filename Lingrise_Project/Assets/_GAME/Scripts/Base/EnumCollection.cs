namespace AgeOfWar
{
    public enum GameState : byte
    {
        Setup,
        Playing,
        Booster,
        Pause,
        End,
        ShowResult,
    }

    public enum GridState : byte
    {
        Init,
        Select,
        Align,
        Swap,
        Discard,
        Activate,
        Match,
        Drop,
    }

    public enum GemEnum : byte
    {
        Blank = 0,
        Omni,
        Sword = 10,
        Bow,
        Dagger,
        Spear,
        Shield,
    }

    public enum GemChangeType : byte
    {
        ChangeToBlank,
        ChangeToOmni,
        LevelUp,
    }

    // public enum GemState : byte
    // {
    //     Default,
    //     Charged,
    // }

    // public enum GemTrigger : byte
    // {
    //     None,
    //     Firework,
    //     TNT,
    //     Paper,
    // }

    // public enum TouchEnum : byte
    // {
    //     Touch,
    //     Drag,
    // }
}