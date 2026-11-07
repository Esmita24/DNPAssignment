
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Services;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
    }

    public async Task LoginAsync(string userName, string password)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequest(userName, password));

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

        string serialisedData = JsonSerializer.Serialize(userDto);

        await jsRuntime.InvokeVoidAsync(
            "sessionStorage.setItem",
            "currentUser",
            serialisedData);

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(principal)));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? userAsJson;

        try
        {
            userAsJson = await jsRuntime.InvokeAsync<string?>(
                "sessionStorage.getItem", "currentUser");
        }
        catch (InvalidOperationException)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(userAsJson)!;

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task Logout()
    {
        await jsRuntime.InvokeVoidAsync(
            "sessionStorage.removeItem", "currentUser");

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal())));
    }
}
