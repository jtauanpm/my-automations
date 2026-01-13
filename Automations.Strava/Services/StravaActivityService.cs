using Automations.Strava.ExternalServices;

namespace Automations.Strava.Services;

public static class StravaActivityService
{
    private const string IdTenisOlympikusMarte = "g27381848";
    
    public static async Task AdicionarTenisParaAtividadesOutubro2025(string accessToken)
    {
        var outubro2025 = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc);

        var atividades = await StravaApi.ObterAtividades(accessToken, outubro2025);
        if (atividades != null)
        {
            var atividadesDescalco = atividades.Where(a => a.IdTenis == null).ToList();

            Console.WriteLine($"Atividades descalço: {atividadesDescalco.Count}");

            foreach (var a in atividadesDescalco)
            {
                await StravaApi.AtualizarTenisParaAtividade(a.Id, IdTenisOlympikusMarte, accessToken);
            }
        }
    }
}