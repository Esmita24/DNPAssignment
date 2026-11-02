using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync("Posts", request);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<PostDto>())!;
    }

    public async Task<List<PostDto>> GetPostsAsync()
    {
        return await client.GetFromJsonAsync<List<PostDto>>("Posts")
               ?? new List<PostDto>();
    }
}