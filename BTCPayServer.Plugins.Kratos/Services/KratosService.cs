using System.Threading.Tasks;
using System.Net.Http;
using System;
using BTCPayServer.Plugins.Kratos.Models;
using BTCPayServer.Plugins.Kratos.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using BTCPayServer.Abstractions.Contracts;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace BTCPayServer.Plugins.Kratos.Services
{
    public class KratosService
    {
        private string _kratosUrl;
        private readonly HttpClient _client;

        private readonly ISettingsRepository _settingsRepository;

        public KratosService(ISettingsRepository settingsRepository)
        {
            _client = new HttpClient();
            _settingsRepository = settingsRepository;
            RefreshKratosURL();
        }

        public void RefreshKratosURL()
        {
            var kratosConfig = _settingsRepository.GetSettingAsync<KratosConf>().Result;
            _kratosUrl = kratosConfig.KratosPublic;
        }

        public async Task<KratosIdentity> GetUserIdByToken(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_kratosUrl}/sessions/whoami");
            request.Headers.Add("Authorization", token);
            return await SendWhoamiRequestAsync(request);
        }

        public async Task<KratosIdentity> GetUserIdByCookie(string cookieName, string cookieContent)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_kratosUrl}/sessions/whoami");
            request.Headers.Add("Cookie", $"{cookieName}={cookieContent}");
            return await SendWhoamiRequestAsync(request);
        }

        private async Task<KratosIdentity> SendWhoamiRequestAsync(HttpRequestMessage request)
        {
            var res = await _client.SendAsync(request);
            // Console.WriteLine($"Kratos Response Code: {res.StatusCode}, From request for: {request.RequestUri}");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            // Console.WriteLine(JValue.Parse(json).ToString(Formatting.Indented));
            var whoami = System.Text.Json.JsonSerializer.Deserialize<Whoami>(json);
            if (!whoami.Active)
                throw new InvalidOperationException("Session is not active.");

            return whoami.Identity;
        }

    }
}