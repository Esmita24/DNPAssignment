using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentDto request)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync("Comments", request);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<CommentDto>())!;
    }

    public async Task<List<CommentDto>> GetCommentsAsync()
    {
        return await client.GetFromJsonAsync<List<CommentDto>>("Comments")
               ?? new List<CommentDto>();
    }
}