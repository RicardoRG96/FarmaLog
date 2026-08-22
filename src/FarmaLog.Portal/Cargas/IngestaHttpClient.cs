using System.Net.Http.Headers;

namespace FarmaLog.Portal.Cargas
{
    internal sealed class IngestaHttpClient(HttpClient http) : ICargaProcessor
    {
        public async Task<ResultadoDeCarga> ProcesarAsync(ArchivoDeCarga archivo, CancellationToken ct)
        {
            using MultipartFormDataContent content = new();
            using StreamContent streamContent = new(archivo.Content);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            content.Add(streamContent, "archivo", archivo.Name);

            using HttpResponseMessage response = await http.PostAsync("/cargas", content, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ResultadoDeCarga>(ct)
                ?? throw new InvalidOperationException("Ingesta respondió un cuerpo vacío");
        }
    }
}
