using Api.Extensions;
using Application.CQ.Songs.Command.CreateSong;
using Application.CQ.Songs.Query.GetSongFromDb;
using MediatR;
using Application.CQ.Songs.Query.GetSongsMix;
using Application.DTOs.Songs;
using Domain.Primitives;

namespace Api.Endpoints
{
    public static class SongEndpoints
    {
        public static IEndpointRouteBuilder RegisterSongsEndpoints(this IEndpointRouteBuilder app)
        {
            var songGroup = app.MapGroup("api/song").WithTags("Song");

            app.MapGet("api/search/{query}", async (ISender _sender, HttpContext _httpContext, string query, int page = 1) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();

                var command = new GetSongFromDbCommand(query, user?.Guid, page);
                var result = await _sender.Send(command);

                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : result.Value.Count == 0
                        ? Results.NoContent()
                        : Results.Ok(result.ToPaginatedResponse());
            }).WithName("GetSong").WithDescription("FTS").Produces<List<SongDTO>>();

            songGroup.MapPost("", async (
                IFormFile audioFile,
                Guid artistGuid,
                ISender sender,
                HttpContext _httpContext
            ) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();

                using var stream = audioFile.OpenReadStream();
                var command = new CreateSongCommand(audioFile.FileName, artistGuid, stream, user!.Guid);
                var result = await sender.Send(command);

                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : Results.CreatedAtRoute(routeName: "GetSong", routeValues: new { query = result.Value.Guid.ToString() }, value: result.Value);
            }).DisableAntiforgery().RequireAuthorization().WithDescription("Upload from file"); //TODO: Add Antiforgery


            songGroup.MapGet("", async (ISender _sender, HttpContext _httpContext,  
                string albumGuids = "", 
                string playlistGuids = "", 
                string artistGuids = "", 
                int page = 1, string? shuffleSeed = null) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();
                

                var command = new GetSongsMixCommand(
                    UserGuid: user?.Guid,
                    AlbumGuids: ExtractGuids(albumGuids),
                    PlaylistGuids: ExtractGuids(playlistGuids),
                    ArtistGuids: ExtractGuids(artistGuids),
                    Page: page,
                    ShuffleSeed: shuffleSeed);
                
                var result = await _sender.Send(command);
                
                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : result.Value.ToList().Count == 0
                        ? Results.NoContent()
                        : Results.Ok(result.ToPaginatedResponse());

            }).RequireAuthorization().WithDescription("Shuffle based on multiple albums or playlists or artists").Produces<PaginatedResponse<IEnumerable<SongDTO>>>();
            
            return app;
        }

        private static List<Guid> ExtractGuids(string commaSeparated)
        {
            return commaSeparated
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(g => Guid.TryParse(g, out var guid) ? guid : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g.Value)
                .ToList();
        }
    }
}