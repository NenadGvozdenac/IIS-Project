using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces;

public interface IAutomaticRecommendationRepository
{
    AutomaticRecommendation Create(AutomaticRecommendation automaticRecommendation);
    AutomaticRecommendation? GetById(int id);
    IEnumerable<AutomaticRecommendation> GetByMatchId(int matchId);
    AutomaticRecommendation Update(AutomaticRecommendation automaticRecommendation);
    void Delete(int id);
}