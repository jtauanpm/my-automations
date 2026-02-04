using Automations.Instagram.OperationControl;
using Microsoft.Extensions.Configuration;

namespace Automations.Instagram;

public static class Configuration
{
    public static IConfiguration Global { get; set; } = null!;

    public static OperationControllerOptions GetOperationControlOptions()
    {
        var result = new OperationControllerOptions();
        Global.GetSection(OperationControllerOptions.SectionName).Bind(result);
        return result;
    }
}