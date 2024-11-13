using Api.Extensions;
using Application.CQ.Playlists;
using Application.Repositories.Shared;
using MediatR;

namespace Api.Endpoints
{
    public static class PlaylistEndpoints
    {
        public static async Task RegisterPlaylistEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/playlist");

            //Create new playlist
            group.MapPost("", async (ISender sender, HttpContext _httpContext, IUnitOfWork _uow,
                string playlistTitle) =>
            {
                var userId = _httpContext.GetExternalUserId();
                var user = await _uow.UserRepository.GetByExternalIdAsync(userId);

                var command = new CreatePlaylistCommand(playlistTitle, user!.Guid);

                var result = await sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);
                return Results.Created($"api/playlist/{result.Value}", result.Value);
            }).RequireAuthorization();

            //Get playlist by Guid
            group.MapGet("/{guid}", async (ISender sender, Guid guid) =>
            {
                //_uow.PlaylistRepository.FirstOrDefaultAsync(x => x.Guid == guid);
                return await Task.FromResult("");
            });
        }
    }
}
