using System.Net;
using System.Net.Http.Headers;
using System.Text;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Persistence.ExternalsApi.KeycloakExternalApi.ModelsKeycloakExternalApi;
using TCG.CatalogService.Application.Keycloak.DTO.Request;
using TCG.Common.Middlewares.MiddlewareException;
using TCG.Common.Settings;

namespace TCG.AuthenticationService.Persistence.ExternalsApi.KeycloakExternalApi.RepositoriesKeycloakExternalApi;

public class KeycloakRepository : IKeycloakRepository
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly KeycloakSetting _keycloakSetting;

    public KeycloakRepository(IHttpClientFactory clientFactory, IConfiguration configuration, IMapper mapper, IOptions<KeycloakSetting> keycloakSetting)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _keycloakSetting = keycloakSetting.Value;
        _mapper = mapper;
    }
    
    public async Task<string> GetAdminAccessTokenAsync()
    {
        var httpClient = GetConfigHttpClient();
        var requestBody = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _keycloakSetting.ClientId),
            new KeyValuePair<string, string>("client_secret", _keycloakSetting.ClientSecret)
        });

        var response = await httpClient.PostAsync(_keycloakSetting.TokenEndpoint, requestBody);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        dynamic tokenResponse = JsonConvert.DeserializeObject(content);

        return tokenResponse.access_token;
    }
    
    public async Task<string> AuthenticateUserAsync(UserLogin userLogin)
    {
        var httpClient = GetConfigHttpClient();
        var requestBody = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", _keycloakSetting.ClientId),
            new KeyValuePair<string, string>("client_secret", _keycloakSetting.ClientSecret),
            new KeyValuePair<string, string>("scope", "openid"),
            new KeyValuePair<string, string>("username", userLogin.Email),
            new KeyValuePair<string, string>("password", userLogin.Password)
        });

        var response = await httpClient.PostAsync(_keycloakSetting.TokenEndpoint, requestBody);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        dynamic tokenResponse = JsonConvert.DeserializeObject(content);

        return tokenResponse.access_token;
    }

    public async Task<Guid> GetUserInfoAsync(string accessToken)
    {
        var httpClient = GetConfigHttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync(_keycloakSetting.UserInfoEndpoint);
        if (response.ReasonPhrase == "Unauthorized")
        {
            throw new UnAuthorizedException();
        }
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JsonConvert.DeserializeObject<KeycloakUser>(content);
        var item = Guid.Parse(userInfo.Sub);
        return item;
    }

    public async Task CreateUserAsync(string accessToken, UserRegistration userRegistration)
    {
        var httpClient = GetConfigHttpClient();
        try
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var user = new
            {
                username = userRegistration.Username,
                email = userRegistration.Email,
                firstName = userRegistration.Firstname,
                lastName = userRegistration.Lastname,
                attributes = new Dictionary<string, string>
                {
                    { "address", userRegistration.Adress },
                    { "dateOfBirth", userRegistration.BirthDate.ToString("yyyy-MM-dd")},
                    { "city", userRegistration.City },
                    { "postalCode", userRegistration.PostalCode },
                },
                enabled = true,
                emailVerified = false,
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = userRegistration.Password,
                        temporary = false
                    }
                }
            };

            var requestBody = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(_keycloakSetting.AdminUsersEndpoint, requestBody);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erreur 400 (Bad Request) : " + jsonResponse);
            }
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new UserAlreadyExistsException("User already exists");
            }
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<string> GetUserIdAsync(string accessToken, string username)
    {
        try
        {
            var httpClient = GetConfigHttpClient();

            var searchUserEndpoint = $"{_keycloakSetting.AdminUsersEndpoint}?username={username}";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(searchUserEndpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<KeycloakUser>>(content);

            if (users.Count == 0)
            {
                throw new Exception("Aucun utilisateur trouvé avec ce nom d'utilisateur.");
            }
            return users[0].Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private HttpClient GetConfigHttpClient()
    {
        var httpClient = _clientFactory.CreateClient();
        var baseUrl = _keycloakSetting.BaseUrl;
        httpClient.BaseAddress = new Uri(baseUrl);
        return httpClient;
    }
}