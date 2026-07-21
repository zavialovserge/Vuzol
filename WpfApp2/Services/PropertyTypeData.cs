using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class PropertyTypeData
    {
        private const string GET_ALL_PROPERTYTYPE_SQL =
                   @"SELECT ""Id"", ""Name"" FROM ""PropertyType""";

        private const string UPDATE_PROPERTYTYPE_SQL =
                   @"UPDATE ""PropertyType"" SET ""Name"" = ";

        private const string INSERT_PROPERTYTYPE_SQL =
                  @"INSERT INTO ""PropertyType"" (""Name"") VALUES ";

        private const string DELETE_PROPERTYTYPE_SQL =
                  @"DELETE FROM ""PropertyType"" WHERE ""Id"" = ";

        public static IEnumerable<PropertyType> GetAllPropertyType()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<PropertyTypeDTO> propertyTypeDTOs =
                database.Query<PropertyTypeDTO>(GET_ALL_PROPERTYTYPE_SQL);
            return propertyTypeDTOs.Select(ToPropetyType).ToList();
        }

        public static bool InsertPropertyType(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var strInsert = INSERT_PROPERTYTYPE_SQL + $"('{EscapePostgresString(propertyTypeDTO.Name)}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }

        public static bool EditPropertyType(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var escapedName = EscapePostgresString(propertyTypeDTO.Name);
            var idValue = propertyTypeDTO.Id;
            var strUpdate = UPDATE_PROPERTYTYPE_SQL + $"'{escapedName}' WHERE \"Id\" = {idValue}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }

        public static bool DeleteFromDb(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var strDelete = DELETE_PROPERTYTYPE_SQL + $"{propertyTypeDTO.Id}";
            var result = database.Execute(strDelete);
            return result == 1;
        }

        private static string EscapePostgresString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.Replace("'", "''");
        }

        private static PropertyType ToPropetyType(PropertyTypeDTO dto)
                => new PropertyType(dto.Id, dto.Name);

        private static PropertyTypeDTO ToPropetyTypeDTO(PropertyType propertyType)
               => new PropertyTypeDTO()
               {
                   Id = propertyType.Id,
                   Name = propertyType.Name
               };
    }
}
