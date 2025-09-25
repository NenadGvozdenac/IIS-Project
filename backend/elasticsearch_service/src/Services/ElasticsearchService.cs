using Nest;
using elasticsearch_service.src.Models;

namespace elasticsearch_service.src.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _playersIndex;
        private readonly string _sessionsIndex;

        public ElasticsearchService(IElasticClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            _playersIndex = _configuration["Elasticsearch:IndexNames:Players"] ?? "players";
            _sessionsIndex = _configuration["Elasticsearch:IndexNames:Sessions"] ?? "sessions";
        }

        #region Players

        public async Task<bool> CreatePlayerAsync(Player player)
        {
            var response = await _client.IndexAsync(player, i => i.Index(_playersIndex).Id(player.IdPlayer));
            return response.IsValid;
        }

        public async Task<Player?> GetPlayerAsync(int id)
        {
            var response = await _client.GetAsync<Player>(id, g => g.Index(_playersIndex));
            return response.IsValid ? response.Source : null;
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Player>(s => s
                .Index(_playersIndex)
                .From(skip)
                .Size(take)
                .Query(q => q.MatchAll()));

            return response.IsValid ? response.Documents : Enumerable.Empty<Player>();
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            var response = await _client.UpdateAsync<Player>(player.IdPlayer, u => u
                .Index(_playersIndex)
                .Doc(player)
                .DocAsUpsert(true));

            return response.IsValid;
        }

        public async Task<bool> DeletePlayerAsync(int id)
        {
            var response = await _client.DeleteAsync<Player>(id, d => d.Index(_playersIndex));
            return response.IsValid;
        }

        public async Task<IEnumerable<Player>> SearchPlayersAsync(string searchTerm, int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Player>(s => s
                .Index(_playersIndex)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(p => p.Name)
                            .Field(p => p.Surname)
                            .Field(p => p.FullName)
                            .Field(p => p.Nationality)
                            .Field(p => p.Position))
                        .Query(searchTerm)
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(Fuzziness.Auto))));

            return response.IsValid ? response.Documents : Enumerable.Empty<Player>();
        }

        #endregion

        #region Sessions

        public async Task<bool> CreateSessionAsync(Session session)
        {
            var response = await _client.IndexAsync(session, i => i.Index(_sessionsIndex).Id(session.IdSession));
            return response.IsValid;
        }

        public async Task<Session?> GetSessionAsync(int id)
        {
            var response = await _client.GetAsync<Session>(id, g => g.Index(_sessionsIndex));
            return response.IsValid ? response.Source : null;
        }

        public async Task<IEnumerable<Session>> GetSessionsAsync(int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .From(skip)
                .Size(take)
                .Query(q => q.MatchAll()));

            return response.IsValid ? response.Documents : Enumerable.Empty<Session>();
        }

        public async Task<bool> UpdateSessionAsync(Session session)
        {
            var response = await _client.UpdateAsync<Session>(session.IdSession, u => u
                .Index(_sessionsIndex)
                .Doc(session)
                .DocAsUpsert(true));

            return response.IsValid;
        }

        public async Task<bool> DeleteSessionAsync(int id)
        {
            var response = await _client.DeleteAsync<Session>(id, d => d.Index(_sessionsIndex));
            return response.IsValid;
        }

        public async Task<IEnumerable<Session>> SearchSessionsAsync(string searchTerm, int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(s => s.PlayerFullName)
                            .Field(s => s.SessionStatus)
                            .Field(s => s.SessionType))
                        .Query(searchTerm)
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(Fuzziness.Auto))));

            return response.IsValid ? response.Documents : Enumerable.Empty<Session>();
        }

        public async Task<IEnumerable<Session>> GetSessionsByPlayerAsync(int playerId, int skip = 0, int take = 10)
        {
            var response = await _client.SearchAsync<Session>(s => s
                .Index(_sessionsIndex)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.IdPlayer)
                        .Value(playerId))));

            return response.IsValid ? response.Documents : Enumerable.Empty<Session>();
        }

        #endregion

        #region Index Management

        public async Task<bool> CreateIndexesAsync()
        {
            var playersResult = await CreatePlayersIndexAsync();
            var sessionsResult = await CreateSessionsIndexAsync();
            return playersResult && sessionsResult;
        }

        public async Task<bool> DeleteIndexesAsync()
        {
            var playersResult = await _client.Indices.DeleteAsync(_playersIndex);
            var sessionsResult = await _client.Indices.DeleteAsync(_sessionsIndex);
            return playersResult.IsValid && sessionsResult.IsValid;
        }

        private async Task<bool> CreatePlayersIndexAsync()
        {
            var indexExists = await _client.Indices.ExistsAsync(_playersIndex);
            if (indexExists.Exists) return true;

            var response = await _client.Indices.CreateAsync(_playersIndex, c => c
                .Map<Player>(m => m.AutoMap()));

            return response.IsValid;
        }

        private async Task<bool> CreateSessionsIndexAsync()
        {
            var indexExists = await _client.Indices.ExistsAsync(_sessionsIndex);
            if (indexExists.Exists) return true;

            var response = await _client.Indices.CreateAsync(_sessionsIndex, c => c
                .Map<Session>(m => m.AutoMap()));

            return response.IsValid;
        }

        #endregion
    }
}