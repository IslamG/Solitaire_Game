public static class OptionsManager 
{
    public static DrawType DrawCount { get; private set; } = DrawType.Single;
    public static ScoringType Scoring { get; private set; } = ScoringType.Standard;
    public static bool TimedGame { get; private set; } = true;
    public static bool HasStatusBar { get; private set; } = true;
    //public static bool HasScoring { get; private set; } = false;
    public static int CardBackIndex { get; private set; } = 52;
    public static bool LimitDiscardResets { get; private set; } 

    public static void SetDrawType (this DrawType type) => DrawCount = type;
    public static void SetScoringType(this ScoringType type) => Scoring = type;
    public static void SetTimedGame(this bool isTimed) => TimedGame = isTimed;
    public static void SetHasStatusBar(this bool hasStatusBar) => HasStatusBar = hasStatusBar;
    public static void SetBackIndext(this int index) => CardBackIndex = index;
    public static void SetCardBack(this int index) => index.ChangeCardBack();
    public static int NumberOfAllowedResets() => Scoring is ScoringType.Vegas ? 3 : 3;
    public static int DrawCountAsValue(this DrawType drawType) => drawType is DrawType.Single ? 1 : 3;
}
public enum DrawType
{
    Single, Three
}
public enum ScoringType
{
    None, Vegas, Standard
}

