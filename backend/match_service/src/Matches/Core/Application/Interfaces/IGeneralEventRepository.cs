using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface IGeneralEventRepository
    {
        IEnumerable<GeneralEvent> GetByMatchId(int matchId);
        GeneralEvent? GetById(int eventId);
        GeneralEvent Create(GeneralEvent generalEvent);
        void Update(GeneralEvent generalEvent);
        bool Delete(int eventId);
        bool Exists(int eventId);
    }
}