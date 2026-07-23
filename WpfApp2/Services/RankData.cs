using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class RankData
    {
        private const string GET_ALL_RANKS_SQL =
                   @"SELECT ""ID"", ""Description"" FROM ""Rank""";

        private const string UPDATE_RANK_SQL =
                   @"UPDATE ""Rank"" SET ""Description"" = @Description WHERE ""ID"" = @Id";

        private const string INSERT_RANK_SQL =
                  @"INSERT INTO ""Rank"" (""Description"") VALUES (@Description)";

        private const string DELETE_RANK_SQL =
                  @"DELETE FROM ""Rank"" WHERE ""ID"" = @Id";

        public static IEnumerable<Rank> getAllRanks()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<RankDTO> rankDTOs =
                database.Query<RankDTO>(GET_ALL_RANKS_SQL);
            return rankDTOs.Select(ToRank).ToList();
        }

        public static bool DeleteFromDb(Rank rank)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            RankDTO rankDTO = ToRankDTO(rank);
            var result = database.Execute(DELETE_RANK_SQL, new { Id = rankDTO.Id });
            return result == 1;
        }

        public static bool EditIEank(Rank rank)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            RankDTO rankDTO = ToRankDTO(rank);
            var escapedDesc = EscapePostgresString(rankDTO.Description);
            var result = database.Execute(UPDATE_RANK_SQL, new { Description = escapedDesc, ID= rankDTO.Id });
            return result == 1;
        }

        public static bool InsertRank(Rank rank)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            RankDTO rankDTO = ToRankDTO(rank);
            var result = database.Execute(INSERT_RANK_SQL, new { Description = EscapePostgresString(rankDTO.Description) });
            return result == 1;
        }

        private static string EscapePostgresString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.Replace("'", "''");
        }

        private static Rank ToRank(RankDTO dto) =>
                 new Rank(dto.Id, dto.Description);

        private static RankDTO ToRankDTO(Rank dto) =>
                 new RankDTO() { Id = dto.Id, Description = dto.RankDescription };
    }
}
