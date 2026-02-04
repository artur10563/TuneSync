using Domain.Entities;
using Domain.Primitives;

namespace Application.Services;

public interface ISongService
{
    Task<Result> ReplaceAudioFileAsync(Song song, GlobalVariables.AudioSource audioSource, Stream audioStream);
}