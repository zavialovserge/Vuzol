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
                   @"SELECT  [ID]
                            ,[Name]
                            FROM [PropertyDb].[dbo].[Unit]";
        private const string UPDATE_UNIT_SQL  =
                   @"UPDATE [dbo].[Unit] SET [Name] =";
        private const string INSERT_UNIT_SQL =
                  @"INSERT INTO [dbo].[Unit]
                            ([Name])
                            VALUES ";
        private const string DELETE_UNIT_SQL =
                  @"DELETE FROM [dbo].[Unit] ";
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
            var strInsert = INSERT_UNIT_SQL + $"(N'{unitDTO.Name}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }
        public static bool EditIUnit(Unit unit)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            UnitDTO unitDTO = ToUnitDTO(unit);
            var strInsert = UPDATE_UNIT_SQL + $"N'{unitDTO.Name}' " +
                " WHERE [Id] =" + $"{unitDTO.Id}";
            var result = database.Execute(strInsert);
            return result == 1;
        }
        public static bool DeleteFromDb(Unit unit)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            UnitDTO unitDTO = ToUnitDTO(unit);
            var strUpdate = DELETE_UNIT_SQL +
                " WHERE [Id] =" + $"{unitDTO.Id}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        private static Unit ToUnit(UnitDTO dto) =>
         new Unit(dto.Id, dto.Name);
        private static UnitDTO ToUnitDTO(Unit unit) =>
         new UnitDTO()
         {
             Id = unit.Id,
             Name = unit.Name,
         };
    }
}
