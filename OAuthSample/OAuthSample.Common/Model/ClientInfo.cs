namespace OAuthSample.Common.Model
{
    public class ClientInfo
    {
        public AuthType AuthType { get; set; }

        public string ClientId { get; set; }

        public string RedirectUrI { get; set; }

        public string ClientSecret { get; set; }
    }
}