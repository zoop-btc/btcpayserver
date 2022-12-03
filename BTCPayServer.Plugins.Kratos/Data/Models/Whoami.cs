using System;
using System.Text.Json.Serialization;

namespace BTCPayServer.Plugins.Kratos.Models
{
    public class Whoami
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("authenticated_at")]
        public DateTime AuthenticatedAt { get; set; }

        [JsonPropertyName("issued_at")]
        public DateTime IssuedAt { get; set; }

        [JsonPropertyName("identity")]
        public KratosIdentity Identity { get; set; }
    }

    public class KratosIdentity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("traits")]
        public Traits Traits { get; set; }
    }

    public class Traits
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}