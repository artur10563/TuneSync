using Application.DTOs.Albums;
using Application.DTOs.Songs;

namespace Application.DTOs.Artists;

/// <summary>
/// Artist Info and list of albums + separate songs
/// </summary>
public record ArtistSummaryDTO(
    ArtistInfoDTO ArtistInfo, 
    ICollection<AlbumSummaryDTO> Albums, 
    ICollection<SongDTO> Songs)
{
}