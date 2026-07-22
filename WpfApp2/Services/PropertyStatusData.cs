using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class PropertyStatusData
    {
        private const string GET_ALL_PROPERTYSTATUS_SQL =
                   @"SELECT ""Id"", ""Name"" FROM ""PropertyStatus""";

        private const string UPDATE_PROPERTYSTATUS_SQL =
                   @"UPDATE ""PropertyStatus"" SET ""Name"" = ";

        private const string INSERT_PROPERTYSTATUS_SQL =
                  @"INSERT INTO ""PropertyStatus"" (""Name"") VALUES ";

        private const string DELETE_PROPERTYSTATUS_SQL =
                  @"DELETE FROM ""PropertyStatus"" WHERE ""Id"" = ";

        public static IEnumerable<PropertyStatus> GetAllPropertyStatus()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<PropertyStatusDTO> propertyStatusDTOs =
                database.Query<PropertyStatusDTO>(GET_ALL_PROPERTYSTATUS_SQL);
            return propertyStatusDTOs.Select(ToPropertyStatus).ToList();
        }
        public static bool InsertPropertyType(PropertyStatus propertyStatus)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyStatusDTO propertyStatusDTO = ToPropertyStatusDTO(propertyStatus);
            var strInsert = INSERT_PROPERTYSTATUS_SQL + $"(N'{propertyStatusDTO.Name}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }
        public static bool EditPropertyType(PropertyStatus propertyStatus)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyStatusDTO propertyStatusDTO = ToPropertyStatusDTO(propertyStatus);
            var strUpdate = UPDATE_PROPERTYSTATUS_SQL + $"N'{propertyStatusDTO.Name}' " +
                " WHERE \"Id\" =" + $"{propertyStatusDTO.Id}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool DeleteFromDb(PropertyStatus propertyStatus)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyStatusDTO propertyStatusDTO = ToPropertyStatusDTO(propertyStatus);
            var strDelete = DELETE_PROPERTYSTATUS_SQL + $"{propertyStatusDTO.Id}";
            var result = database.Execute(strDelete);
            return result == 1;
        }
        private static PropertyStatus ToPropertyStatus(PropertyStatusDTO dTO)
        => new PropertyStatus(dTO.Id, dTO.Name);
        private static PropertyStatusDTO ToPropertyStatusDTO(PropertyStatus propertyType)
       => new PropertyStatusDTO()
       {
           Id = propertyType.Id,
           Name = propertyType.Name,
       };
    }
}
