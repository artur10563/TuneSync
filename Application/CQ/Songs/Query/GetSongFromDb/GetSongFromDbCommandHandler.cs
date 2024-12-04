using Application.DTOs.Songs;
using Application.Repositories.Shared;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    internal class GetSongFromDbCommandHandler : IRequestHandler<GetSongFromDbCommand, Result<List<SongDTO>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetSongFromDbCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<List<SongDTO>>> Handle(GetSongFromDbCommand request, CancellationToken cancellationToken)
        {
            var result = _uow.SongRepository.Where(x =>
                x.Title.ToLower().Contains(request.query),
                asNoTracking: true)
                .ProjectTo<SongDTO>(_mapper.ConfigurationProvider)
                .ToList();

            return Result.Success(result);
        }
    }
}
