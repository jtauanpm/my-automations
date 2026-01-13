using System.Net.Http.Headers;
using System.Text.Json;
using Automations.Strava.Entities;

namespace Automations.Strava.ExternalServices;

/// <summary>
/// https://www.strava.com/settings/api
/// https://developers.strava.com/docs/getting-started/
/// </summary>
public static class StravaApi
{
    public static async Task<List<Activity>?> ObterAtividades(string accessToken, DateTime startDate)
    {
        var client = GetHttpClient(accessToken);

        var afterDate = new DateTimeOffset(startDate).ToUnixTimeSeconds();

        var response = await client.GetAsync($"athlete/activities?after={afterDate}&per_page=200");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

        var json = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<List<Activity>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task AtualizarTenisParaAtividade(long activityId, string gearId, string accessToken)
    {
        var client = GetHttpClient(accessToken);

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("gear_id", gearId)
        ]);

        var response = await client.PutAsync($"activities/{activityId}", content);

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar atividade {activityId}: {response.StatusCode} - {body}");

        Console.WriteLine($"Atividade {activityId} atualizada com sucesso.");
    }
    
    private static HttpClient GetHttpClient(string accessToken)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://www.strava.com/api/v3/")
        };

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }
}