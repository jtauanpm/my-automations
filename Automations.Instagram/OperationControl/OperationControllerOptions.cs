namespace Automations.Instagram.OperationControl;

public class OperationControllerOptions
{
    public const string SectionName = "Automations:Instagram:OperationControlOptions";
    public int InteractionPageSize { get; set; } = 25;
    public RangeOptions SecondsBetweenOperation { get; set; } = new()
    {
        Min = 5,
        Max = 10
    };

    public RangeOptions MinutesBetweenInteractions { get; set; } = new()
    {
        Min = 5,
        Max = 10
    };
}

public class RangeOptions
{
    public int Min { get; set; }
    public int Max { get; set; }
}