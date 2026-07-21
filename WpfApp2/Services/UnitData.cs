using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class UnitData
    {
        private const string GET_ALL_Untis_SQL =
                   @"SELECT ""Id"", ""Name"" FROM ""Unit""";

        private const string UPDATE_UNIT_SQL =
                   @"UPDATE ""Unit"" SET ""Name"" = ";

        private const string INSERT_UNIT_SQL =
                  @"INSERT INTO ""Unit"" (""Name"") VALUES ";

        private const string DELETE_UNIT_SQL =
                  @"DELETE FROM ""Unit"" WHERE ""Id"" = ";

        public static IEnumerable<Unit> getAllUnits()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<UnitDTO> unitDTOs =
                database.Query<UnitDTO>(GET_ALL_Untis_SQL);
            return unitDTOs.Select(ToUnit).ToList();
        }

        public static bool InsertUnit(Unit unit)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            UnitDTO unitDTO = ToUnitDTO(unit);
            var strInsert = INSERT_UNIT_SQL + $"('{EscapePostgresString(unitDTO.Name)}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }

        public static bool EditIUnit(Unit unit)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            UnitDTO unitDTO = ToUnitDTO(unit);
            var escapedName = EscapePostgresString(unitDTO.Name);
            var idValue = unitDTO.Id;
            var strUpdate = UPDATE_UNIT_SQL + $"'{escapedName}' WHERE \"Id\" = {idValue}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }

        public static bool DeleteFromDb(Unit unit)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            UnitDTO unitDTO = ToUnitDTO(unit);
            var strDelete = DELETE_UNIT_SQL + $"{unitDTO.Id}";
            var result = database.Execute(strDelete);
            return result == 1;
        }

        private static string EscapePostgresString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.Replace("'", "''");
        }

        private static Unit ToUnit(UnitDTO dto) =>
                 new Unit(dto.Id, dto.Name);

        private static UnitDTO ToUnitDTO(Unit unit) =>
                 new UnitDTO()
                 {
                     Id = unit.Id,
                     Name = unit.Name
                 };
    }
}
