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
                   @"SELECT  [ID]
                            ,[Description]
                            FROM [PropertyDb].[dbo].[Rank]";
 
        private const string UPDATE_RANK_SQL =
                   @"UPDATE [dbo].[Rank] SET [Description] =";
        private const string INSERT_RANK_SQL =
                  @"INSERT INTO [dbo].[Rank]
                            ([Description])
                            VALUES ";
        private const string DELETE_RANK_SQL =
                  @"DELETE FROM [dbo].[Rank]";
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
            var strDelete = DELETE_RANK_SQL +
                " WHERE [Id] =" + $"{rankDTO.Id}";
            var result = database.Execute(strDelete);
            return result == 1;
        }

        public static bool EditIEank(Rank rank)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            RankDTO rankDTO = ToRankDTO(rank);
            var strUpdate = UPDATE_RANK_SQL + $"N'{rankDTO.Description}' " +
                " WHERE [Id] =" + $"{rankDTO.Id}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool InsertRank(Rank rank)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            RankDTO rankDTO = ToRankDTO(rank);
            var strInsert = INSERT_RANK_SQL + $"(N'{rankDTO.Description}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }
        private static Rank ToRank(RankDTO dto) =>
         new Rank(dto.Id,dto.Description);
        private static RankDTO ToRankDTO(Rank dto) =>
         new RankDTO() { Id = dto.Id,Description = dto.RankDescription};

       
    }
}
