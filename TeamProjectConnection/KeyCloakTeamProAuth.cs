using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TeamProjectConnection.Intefaces;
using TeamProjectConnection.Models;

namespace TeamProjectConnection;

public class KeyCloakTeamProAuth : ITeamProAuthManager
{
    private const string ClientId = "teampro";
    private const string RedirectUri = "https://teamproject.urfu.ru/#/?status=active&year=2025&semester=1";
    private const string Realm = "urfu-lk";
    private const string BaseUrl = "https://keys.urfu.ru/auth/realms/" + Realm;
    private const string TokenUrl = $"{BaseUrl}/protocol/openid-connect/token";
    
    private readonly ILogger<KeyCloakTeamProAuth> _logger;
    
    private TokenResponse? _tokenInfo;
    private DateTime? _tokenExpiresAt;

    public KeyCloakTeamProAuth(ILogger<KeyCloakTeamProAuth> logger)
    {
        _logger = logger;
    }
    
    public bool IsAuthorized()
    {
        return _tokenInfo != null && _tokenExpiresAt != null && _tokenExpiresAt > DateTime.Now;
    }

    public bool TryGetSavedToken(out TokenResponse? tokenInfo)
    {
        if (!IsAuthorized())
        {
            tokenInfo = null;
            return false;
        }

        tokenInfo = _tokenInfo;
        return true;
    }

    public async Task<bool> TryAuthAsync(string login, string password)
    {
        var codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GenerateCodeChallenge(codeVerifier);

        var state = Guid.NewGuid().ToString();
        var nonce = Guid.NewGuid().ToString();

        var authUrl = $"{BaseUrl}/protocol/openid-connect/auth?" +
                      $"client_id={ClientId}" +
                      $"&redirect_uri={HttpUtility.UrlEncode(RedirectUri)}" +
                      $"&state={state}" +
                      $"&response_mode=query" +
                      $"&response_type=code" +
                      $"&scope=openid" +
                      $"&nonce={nonce}" +
                      $"&code_challenge={codeChallenge}" +
                      $"&code_challenge_method=S256";

        var handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer(),
            AllowAutoRedirect = false,
            UseCookies = true
        };

        using var client = new HttpClient(handler);

        var loginPage = await client.GetAsync(authUrl);
        var loginHtml = await loginPage.Content.ReadAsStringAsync();

        var sessionCode = Extract(loginHtml, "session_code=([^&\"]+)");
        var execution = Extract(loginHtml, "execution=([^&\"]+)");
        var tabId = Extract(loginHtml, "tab_id=([^&\"]+)");

        if (string.IsNullOrEmpty(sessionCode) || string.IsNullOrEmpty(execution) || string.IsNullOrEmpty(tabId))
        {
            _logger.LogError("TeamProject authorization parameters could not be retrieved.");
            return false;
        }
        var loginUrl = $"{BaseUrl}/login-actions/authenticate?session_code={sessionCode}&execution={execution}&client_id={ClientId}&tab_id={tabId}";

        var loginContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string,string>("username", login),
            new KeyValuePair<string,string>("password", password),
            new KeyValuePair<string,string>("login", "Вход")
        });

        var loginResp = await client.PostAsync(loginUrl, loginContent);

        if (loginResp.StatusCode == HttpStatusCode.Found)
        {
            var location = loginResp.Headers.Location!.ToString();

            var uri = new Uri(location);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var code = query["code"]!;
            var returnedState = query["state"];

            if (state != returnedState)
            {
                _logger.LogError("Login to TeamProject failed: state mismatch. {state} != {returnedState}", state, returnedState);
                return false;
            }

            var tokenContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type", "authorization_code"),
                new KeyValuePair<string,string>("client_id", ClientId),
                new KeyValuePair<string,string>("code", code),
                new KeyValuePair<string,string>("redirect_uri", RedirectUri),
                new KeyValuePair<string,string>("code_verifier", codeVerifier)
            });

            var tokenResp = await client.PostAsync(TokenUrl, tokenContent);
            var tokenJson = await tokenResp.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            _tokenInfo = JsonConvert.DeserializeObject<TokenResponse>(tokenJson, settings);
            if (_tokenInfo == null)
            {
                _logger.LogError("Failed to deserialize token response from TeamProject. Response: {json}", tokenJson);
                return false;
            }
            _tokenExpiresAt = DateTime.Now.AddSeconds(_tokenInfo.ExpiresIn);
            return true;
        }
        
        var text = await loginResp.Content.ReadAsStringAsync();
        _logger.LogError("Login failed. Status: {status}. Content:\n{content}", loginResp.StatusCode, text);
        _tokenInfo = null;
        return false;
    }

    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string verifier)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(verifier));
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string Extract(string text, string pattern)
    {
        var m = System.Text.RegularExpressions.Regex.Match(text, pattern);
        return m.Success ? m.Groups[1].Value : "";
    }
}
