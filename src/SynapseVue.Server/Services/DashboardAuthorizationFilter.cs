using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace SynapseVue.Server.Services;

public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
{

    private readonly AppSettings _appSettings;
    private readonly TokenValidationParameters _validationParameters;
    private HttpContext _httpContext;

    public DashboardAuthorizationFilter(/*IOptions<*/AppSettings/*>*/ appSettings)
    {
        _appSettings = appSettings;//.Value;


        var certificatePath = Path.Combine(Directory.GetCurrentDirectory(), "IdentityCertificate.pfx");
        var certificate = new X509Certificate2(certificatePath, appSettings.IdentitySettings.IdentityCertificatePassword, OperatingSystem.IsWindows() ? X509KeyStorageFlags.EphemeralKeySet : X509KeyStorageFlags.DefaultKeySet);
        _validationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            RequireSignedTokens = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new X509SecurityKey(certificate),

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ValidateAudience = true,
            ValidAudience = _appSettings.IdentitySettings.Audience,

            ValidateIssuer = true,
            ValidIssuer = _appSettings.IdentitySettings.Issuer,

            AuthenticationType = IdentityConstants.BearerScheme
        };
    }

    private void SetCookie(string jwtToken)
    {
        _httpContext.Response.Cookies.Append("_hangfireCookie",
                jwtToken,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(30)
                });
    }


    public bool Authorize(DashboardContext context)
    {

        //HttpContext.Request.Cookies.TryGetValue("access_token", out string? token);

        //_httpContext.Request.Cookies.TryGetValue("access_token", out string? token);

        //return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("HangfireAdmin");
        //AppSecureJwtDataFormat appSecureJwtDataFormat = new AppSecureJwtDataFormat(_appSettings, _validationParameters);
        //appSecureJwtDataFormat.Unprotect("access_token");
        _httpContext = context.GetHttpContext();
        string jwtToken = null ;
        if (_httpContext.Request.Query.ContainsKey("access_token"))
        {
            jwtToken = _httpContext.Request.Query["access_token"].FirstOrDefault();
            SetCookie(jwtToken);
        }
        else
        {
            jwtToken = _httpContext.Request.Cookies["_hangfireCookie"];
        }

        if (String.IsNullOrEmpty(jwtToken))
        {
            return false;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(jwtToken);

        try
        {
            // Only authenticated users who have the required claim (AzureAD group in this demo) can access the dashboard.
            bool authenticate = jwtSecurityToken.Claims.Any(t => t.Type == "iss" && t.Value == "SynapseVue") && jwtSecurityToken.Claims.Any(t => t.Type == "aud" && t.Value == "SynapseVue");
            return authenticate;
        }
        catch (Exception exception)
        {
            throw exception;
        }
    }
}
