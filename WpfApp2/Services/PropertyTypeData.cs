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
                   @"UPDATE ""PropertyType"" SET ""Name"" = @Name WHERE ""Id"" = @Id";

        private const string INSERT_PROPERTYTYPE_SQL =
                  @"INSERT INTO ""PropertyType"" (""Name"") VALUES (@Name)";

        private const string DELETE_PROPERTYTYPE_SQL =
                  @"DELETE FROM ""PropertyType"" WHERE ""Id"" = @Id";

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
            var result = database.Execute(INSERT_PROPERTYTYPE_SQL, new { Name = EscapePostgresString(propertyTypeDTO.Name) });
            return result == 1;
        }

        public static bool EditPropertyType(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var escapedName = EscapePostgresString(propertyTypeDTO.Name);
            var result = database.Execute(UPDATE_PROPERTYTYPE_SQL, new { Name = escapedName, Id = propertyTypeDTO.Id });
            return result == 1;
        }

        public static bool DeleteFromDb(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var result = database.Execute(DELETE_PROPERTYTYPE_SQL, new { Id = propertyTypeDTO.Id });
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
