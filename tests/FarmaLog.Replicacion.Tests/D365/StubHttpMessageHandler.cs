using System.Net;
using System.Text;

namespace FarmaLog.Replicacion.Tests.D365
{
    public sealed class StubHttpMessageHandler(
        HttpStatusCode statusCode, string responseBody = "") : HttpMessageHandler
    {
        private readonly List<ReceivedRequest> _received = [];

        public IReadOnlyList<ReceivedRequest> Received => _received;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? body = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            _received.Add(new ReceivedRequest(request.Method, request.RequestUri, body));

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        }
    }

    public sealed record ReceivedRequest(HttpMethod Method, Uri? Uri, string? Body);
}
