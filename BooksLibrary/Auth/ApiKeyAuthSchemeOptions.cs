using Microsoft.AspNetCore.Authentication;

namespace MinimalApi.Auth
{
    public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public string ApiKey { get; set; } = "VerySecret";
    }
}
