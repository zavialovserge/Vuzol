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
                   @"SELECT  [ID]
                            ,[Name]
                            FROM [dbo].[PropertyType]";
        private const string UPDATE_PROPERTYTYPE_SQL =
                   @"UPDATE [dbo].[PropertyType] SET [Name] =";
        private const string INSERT_PROPERTYTYPE_SQL =
                  @"INSERT INTO [dbo].[PropertyType]
                            ([Name])
                            VALUES ";
        private const string DELETE_PROPERTYTYPE_SQL =
                  @"DELETE FROM [dbo].[PropertyType] ";
        public static IEnumerable<PropertyType> GetAllPropertyType()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<PropertyTypeDTO> unitDTOs =
                database.Query<PropertyTypeDTO>(GET_ALL_PROPERTYTYPE_SQL);
            return unitDTOs.Select(ToPropetyType).ToList();
        }
        public static bool InsertPropertyType(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var strInsert = INSERT_PROPERTYTYPE_SQL + $"(N'{propertyTypeDTO.Name}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }
        public static bool EditPropertyType(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var strUpdate = UPDATE_PROPERTYTYPE_SQL + $"N'{propertyTypeDTO.Name}' " +
                " WHERE [Id] =" + $"{propertyTypeDTO.Id}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool DeleteFromDb(PropertyType propertyType)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyTypeDTO propertyTypeDTO = ToPropetyTypeDTO(propertyType);
            var strDelete = DELETE_PROPERTYTYPE_SQL +
                " WHERE [Id] =" + $"{propertyTypeDTO.Id}";
            var result = database.Execute(strDelete);
            return result == 1;
        }
        private static PropertyType ToPropetyType(PropertyTypeDTO dTO)
        => new PropertyType(dTO.Id, dTO.Name);
        private static PropertyTypeDTO ToPropetyTypeDTO(PropertyType propertyType)
       => new PropertyTypeDTO()
       {
           Id = propertyType.Id,
           Name = propertyType.Name,
       };
    }
}
