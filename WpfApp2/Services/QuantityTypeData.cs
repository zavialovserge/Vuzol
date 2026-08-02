using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class QuantityTypeData
    {
        private const string GET_ALL_QuantityTypeS_SQL =
                   @"SELECT ""Id"", ""Description"" FROM ""QuantityType""";

        private const string UPDATE_QuantityType_SQL =
                   @"UPDATE ""QuantityType"" SET ""Description"" = @Description WHERE ""Id"" = @Id";

        private const string INSERT_QuantityType_SQL =
                  @"INSERT INTO ""QuantityType"" (""Description"") VALUES (@Description)";

        private const string DELETE_QuantityType_SQL =
                  @"DELETE FROM ""QuantityType"" WHERE ""Id"" = @Id";

        public static IEnumerable<QuantityType> GetAllQuantityTypes()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<QuantityTypeDTO> QuantityTypeDTOs =
                database.Query<QuantityTypeDTO>(GET_ALL_QuantityTypeS_SQL);
            return QuantityTypeDTOs.Select(ToQuantityType).ToList();
        }

        public static bool DeleteFromDb(QuantityType QuantityType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            QuantityTypeDTO QuantityTypeDTO = ToQuantityTypeDTO(QuantityType);
            var result = database.Execute(DELETE_QuantityType_SQL, new { Id = QuantityTypeDTO.Id });
            return result == 1;
        }

        public static bool EditIEank(QuantityType QuantityType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            QuantityTypeDTO QuantityTypeDTO = ToQuantityTypeDTO(QuantityType);
            var escapedDesc = EscapePostgresString(QuantityTypeDTO.Description);
            var result = database.Execute(UPDATE_QuantityType_SQL, new { Description = escapedDesc, ID = QuantityTypeDTO.Id });
            return result == 1;
        }

        public static bool InsertQuantityType(QuantityType QuantityType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            QuantityTypeDTO QuantityTypeDTO = ToQuantityTypeDTO(QuantityType);
            var result = database.Execute(INSERT_QuantityType_SQL, new { Description = EscapePostgresString(QuantityTypeDTO.Description) });
            return result == 1;
        }

        private static string EscapePostgresString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.Replace("'", "''");
        }

        private static QuantityType ToQuantityType(QuantityTypeDTO dto) =>
                 new QuantityType(dto.Id, dto.Description);

        private static QuantityTypeDTO ToQuantityTypeDTO(QuantityType dto) =>
                 new QuantityTypeDTO() { Id = dto.Id, Description = dto.Description };
    }
}
