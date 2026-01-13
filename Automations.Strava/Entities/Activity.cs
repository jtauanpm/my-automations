using System.Text.Json.Serialization;

namespace Automations.Strava.Entities;

public class Activity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Start_Date { get; set; }
    [JsonPropertyName("gear_id")]
    public string IdTenis { get; set; }
}