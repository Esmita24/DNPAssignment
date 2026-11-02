using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync("Users", request);

        response.EnsureSuccessStatusCode();

        UserDto? user =
            await response.Content.ReadFromJsonAsync<UserDto>();

        return user ?? throw new Exception(
            "Could not create user.");
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        List<UserDto>? users =
            await client.GetFromJsonAsync<List<UserDto>>("Users");

        return users ?? new List<UserDto>();
    }
}